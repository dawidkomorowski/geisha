using NLog;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.Tracing;

namespace Geisha.Engine.Core.Diagnostics;

// ReSharper disable once InconsistentNaming
// https://learn.microsoft.com/en-us/dotnet/framework/performance/clr-etw-keywords-and-levels
// https://learn.microsoft.com/en-us/dotnet/fundamentals/diagnostics/runtime-garbage-collection-events
internal sealed class GCEventListener : EventListener
{
    // ReSharper disable InconsistentNaming
    private const EventKeywords GCKeyword = (EventKeywords)0x1;
    private const int GCStartEventId = 1; // Corresponds to GCStart_V2
    private const int GCEndEventId = 2; // Corresponds to GCEnd_V1
    // ReSharper restore InconsistentNaming

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

    public static void Stop()
    {
        if (_instance is null)
        {
            return;
        }

        Logger.Info("Stopping GCEventListener.");

        _instance.Dispose();
        _instance = null;
    }

    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        if (eventSource.Name.Equals("Microsoft-Windows-DotNETRuntime", StringComparison.Ordinal))
        {
            EnableEvents(eventSource, EventLevel.Informational, GCKeyword);
        }
    }

    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        switch (eventData.EventId)
        {
            case GCStartEventId:
            {
                var gcIndex = (uint)eventData.Payload![0]!;
                if (!_gcStartTimeStamp.TryAdd(gcIndex, eventData.TimeStamp.Ticks))
                {
                    Logger.Warn("Received GCStart event for GC#{0} but it was already registered.", gcIndex);
                }

                break;
            }
            case GCEndEventId:
            {
                var gcIndex = (uint)eventData.Payload![0]!;

                if (_gcStartTimeStamp.TryRemove(gcIndex, out var gcStartTimeStamp))
                {
                    var gcEndTimeStamp = eventData.TimeStamp.Ticks;
                    var gcDurationMs = (double)(gcEndTimeStamp - gcStartTimeStamp) / TimeSpan.TicksPerMillisecond;
                    var gcGeneration = (uint)eventData.Payload[1]!;

                    var logLevel = gcDurationMs > WarnThresholdMs ? LogLevel.Warn : LogLevel.Debug;
                    Logger.Log(logLevel, "GC#{0} took {1:f3}ms for generation {2}", gcIndex, gcDurationMs, gcGeneration);
                }
                else
                {
                    Logger.Warn("Received GCEnd event for GC#{0} but no corresponding GCStart event was found.", gcIndex);
                }

                break;
            }
        }
    }
}