using System;
using System.Diagnostics;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core;

/// <summary>
///     Provides timing information about the running engine and controls for game simulation speed.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="ITimeSystem" /> exposes both diagnostic timing information (start-up time, time elapsed
///         since start-up, frames executed) and game time control through <see cref="TimeScale" />.
///     </para>
///     <para>
///         <see cref="ITimeSystem" /> is available for injection into game code such as
///         <see cref="BehaviorComponent" /> derived components, <see cref="ICustomSystem" />
///         implementations, or any other class registered in the dependency injection container.
///     </para>
/// </remarks>
public interface ITimeSystem
{
    /// <summary>
    ///     Gets the wall-clock date and time at which the engine started up.
    /// </summary>
    DateTime StartUpTime { get; }

    /// <summary>
    ///     Gets the wall-clock time elapsed since the engine started up. This value is not affected by
    ///     <see cref="TimeScale" />.
    /// </summary>
    TimeSpan TimeSinceStartUp { get; }

    /// <summary>
    ///     Gets the total number of game loop frames executed since the engine started up.
    /// </summary>
    /// <remarks>
    ///     This count corresponds to the number of variable time step updates performed. Each game loop
    ///     frame is counted once regardless of how many fixed time step updates occur within that frame.
    /// </remarks>
    int FramesSinceStartUp { get; }

    /// <summary>
    ///     Gets the constant time step used for fixed time step updates. The value is derived from
    ///     <see cref="CoreConfiguration.FixedUpdatesPerSecond" /> as one second divided by the number of
    ///     fixed updates per second.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A constant fixed time step decouples time-sensitive simulation logic from the frame rate.
    ///         This ensures that physics, collision detection, and other deterministic systems behave
    ///         consistently across different hardware and frame rates, making gameplay predictable and
    ///         reproducible.
    ///     </para>
    ///     <para>
    ///         Use this time step in fixed update methods such as <see cref="BehaviorComponent.OnFixedUpdate" />
    ///         and implementations of <see cref="ICustomSystem" /> (via
    ///         <see cref="ICustomGameLoopStep.ProcessFixedUpdate" />). For convenience in fixed update methods where
    ///         <see cref="ITimeSystem" /> is not injected, this value is also accessible as the static
    ///         <see cref="TimeStep.FixedDeltaTime" /> property.
    ///     </para>
    /// </remarks>
    TimeSpan FixedDeltaTime { get; }

    /// <summary>
    ///     Gets or sets a time scale factor that adjusts game simulation speed relative to real time. Default value is <c>1.0</c>.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Setting <see cref="TimeScale" /> to <c>0.0</c> effectively pauses the game simulation
    ///         because no game time accumulates. Values between <c>0.0</c> and <c>1.0</c> slow down the
    ///         simulation while values greater than <c>1.0</c> speed it up.
    ///     </para>
    ///     <para>
    ///         <see cref="TimeScale" /> affects both variable time step updates, through
    ///         <see cref="TimeStep.DeltaTime" />, and the frequency of fixed time step updates. It does
    ///         not affect <see cref="TimeSinceStartUp" /> or <see cref="TimeStep.UnscaledDeltaTime" />,
    ///         which always represent real elapsed time.
    ///     </para>
    /// </remarks>
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