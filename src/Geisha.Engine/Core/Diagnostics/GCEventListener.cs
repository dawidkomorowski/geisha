using NLog;
using System;
using System.Diagnostics.Tracing;

namespace Geisha.Engine.Core.Diagnostics;

// ReSharper disable once InconsistentNaming
internal sealed class GCEventListener : EventListener
{
    private const double WarnThresholdMs = 1.0;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static GCEventListener? _instance;
    private long _gcStartTimeStamp = 0;

    private GCEventListener()
    {
    }

    public static void Start()
    {
        if (_instance is not null)
        {
            throw new InvalidOperationException("GCEventListener is already started.");
        }

        Logger.Info("Starting GCEventListener.");

        _instance = new GCEventListener();
    }

    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        if (eventSource.Name.Equals("Microsoft-Windows-DotNETRuntime"))
        {
            EnableEvents(eventSource, EventLevel.Informational, (EventKeywords)0x1);
        }
    }

    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        if (eventData.EventName is null)
        {
            return;
        }

        if (eventData.EventName.Contains("GCStart"))
        {
            _gcStartTimeStamp = eventData.TimeStamp.Ticks;
        }
        else if (eventData.EventName.Contains("GCEnd"))
        {
            var gcEndTimeStamp = eventData.TimeStamp.Ticks;
            var gcDurationMs = (double)(gcEndTimeStamp - _gcStartTimeStamp) / TimeSpan.TicksPerMillisecond;
            var gcIndex = long.Parse(eventData.Payload?[0]?.ToString() ?? "-1");
            var gcGeneration = eventData.Payload?[1]?.ToString() ?? "-1";

            var logLevel = gcDurationMs > WarnThresholdMs ? LogLevel.Warn : LogLevel.Info;
            Logger.Log(logLevel, "GC#{0} took {1:f3}ms for generation {2}", gcIndex, gcDurationMs, gcGeneration);
        }
    }
}