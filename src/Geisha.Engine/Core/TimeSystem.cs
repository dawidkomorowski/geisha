using System;
using System.Diagnostics;

namespace Geisha.Engine.Core;

// TODO: Add missing docs for new APIs.

public interface ITimeSystem
{
    DateTime StartUpTime { get; }
    TimeSpan TimeSinceStartUp { get; }
    int FramesSinceStartUp { get; }
    TimeSpan FixedDeltaTime { get; }
    double TimeScale { get; set; }
}

internal interface ITimeSystemInternal : ITimeSystem
{
    TimeStep NextTimeStep();
}

internal sealed class TimeSystem : ITimeSystemInternal
{
    private readonly Stopwatch _stopwatch = new();

    // TODO: Once migrated to .NET 8 a TimeProvider abstraction can be used instead of custom approach.
    private readonly Func<DateTime> _now;

    public TimeSystem(CoreConfiguration configuration) : this(configuration, () => DateTime.Now)
    {
    }

    public TimeSystem(CoreConfiguration configuration, Func<DateTime> now)
    {
        _now = now;

        if (configuration.FixedUpdatesPerSecond <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(configuration), configuration.FixedUpdatesPerSecond,
                $"{nameof(CoreConfiguration)}.{nameof(CoreConfiguration.FixedUpdatesPerSecond)} must be greater than zero.");
        }

        StartUpTime = _now();
        FixedDeltaTime = TimeSpan.FromSeconds(1.0 / configuration.FixedUpdatesPerSecond);
        TimeStep.FixedDeltaTime = FixedDeltaTime;
    }

    #region Implementation of ITimeSystem

    public DateTime StartUpTime { get; }
    public TimeSpan TimeSinceStartUp => _now() - StartUpTime;
    public int FramesSinceStartUp { get; private set; }
    public TimeSpan FixedDeltaTime { get; }
    public double TimeScale { get; set; } = 1.0;

    #endregion

    #region Implementation of ITimeSystemInternal

    public TimeStep NextTimeStep()
    {
        FramesSinceStartUp++;

        var elapsed = _stopwatch.Elapsed;
        _stopwatch.Restart();
        return new TimeStep(elapsed, TimeScale);
    }

    #endregion
}