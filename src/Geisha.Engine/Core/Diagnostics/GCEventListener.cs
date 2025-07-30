using NLog;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.Tracing;

namespace Geisha.Engine.Core.Diagnostics;

// ReSharper disable once InconsistentNaming
// https://learn.microsoft.com/en-us/dotnet/fundamentals/diagnostics/runtime-garbage-collection-events
internal sealed class GCEventListener : EventListener
{
    private const EventKeywords GCKeyword = (EventKeywords)0x1;
    private const double WarnThresholdMs = 1.0;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static GCEventListener? _instance;
    private readonly ConcurrentDictionary<uint, long> _gcStartTimeStamp = new();

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
            EnableEvents(eventSource, EventLevel.Informational, GCKeyword);
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
            var gcIndex = (uint)(eventData.Payload?[0] ?? 0);
            _gcStartTimeStamp.TryAdd(gcIndex, eventData.TimeStamp.Ticks);
        }

        if (eventData.EventName.Contains("GCEnd"))
        {
            var gcIndex = (uint)(eventData.Payload?[0] ?? 0);

            if (_gcStartTimeStamp.TryRemove(gcIndex, out var gcStartTimeStamp))
            {
                var gcEndTimeStamp = eventData.TimeStamp.Ticks;
                var gcDurationMs = (double)(gcEndTimeStamp - gcStartTimeStamp) / TimeSpan.TicksPerMillisecond;
                var gcGeneration = eventData.Payload?[1];

                var logLevel = gcDurationMs > WarnThresholdMs ? LogLevel.Warn : LogLevel.Info;
                Logger.Log(logLevel, "GC#{0} took {1:f3}ms for generation {2}", gcIndex, gcDurationMs, gcGeneration);
            }
        }
    }
}