using System;

namespace Geisha.Engine.Core;

public readonly record struct TimeStep
{
    public TimeStep(TimeSpan deltaTime) : this(deltaTime, 1.0)
    {
    }

    public TimeStep(TimeSpan unscaledDeltaTime, double timeScale)
    {
        DeltaTime = unscaledDeltaTime * timeScale;
        UnscaledDeltaTime = unscaledDeltaTime;
        TimeScale = timeScale;
    }

    public TimeSpan DeltaTime { get; }
    public TimeSpan UnscaledDeltaTime { get; }
    public double TimeScale { get; }

    public double DeltaTimeSeconds => DeltaTime.TotalSeconds;
    public double UnscaledDeltaTimeSeconds => UnscaledDeltaTime.TotalSeconds;

    public static TimeSpan FixedDeltaTime { get; internal set; }
    public static double FixedDeltaTimeSeconds => FixedDeltaTime.TotalSeconds;
}