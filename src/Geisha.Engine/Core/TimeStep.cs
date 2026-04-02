using System;

namespace Geisha.Engine.Core;

public readonly record struct TimeStep
{
    public TimeStep(TimeSpan unscaledDeltaTime, double timeScale)
    {
        DeltaTime = unscaledDeltaTime * timeScale;
        UnscaledDeltaTime = unscaledDeltaTime;
        TimeScale = timeScale;
    }

    public TimeSpan DeltaTime { get; init; }
    public TimeSpan UnscaledDeltaTime { get; init; }
    public double TimeScale { get; init; }

    public double DeltaTimeSeconds => DeltaTime.TotalSeconds;
    public double UnscaledDeltaTimeSeconds => UnscaledDeltaTime.TotalSeconds;

    public static TimeSpan FixedDeltaTime => TimeSystem.FixedDeltaTime;
    public static double FixedDeltaTimeSeconds => FixedDeltaTime.TotalSeconds;
}