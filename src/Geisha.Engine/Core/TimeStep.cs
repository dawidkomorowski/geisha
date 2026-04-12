using System;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.GameLoop;

namespace Geisha.Engine.Core;

/// <summary>
///     Represents the time information for a single variable time step update in the game loop.
/// </summary>
/// <remarks>
///     <para>
///         A <see cref="TimeStep" /> value is provided by the engine to variable time step update methods
///         such as <see cref="BehaviorComponent.OnUpdate" /> and
///         <see cref="ICustomGameLoopStep.ProcessUpdate" />. It provides both the scaled game time elapsed
///         since the previous frame (<see cref="DeltaTime" />) and the actual real elapsed time
///         (<see cref="UnscaledDeltaTime" />), enabling game code to independently implement
///         time-scale-aware and time-scale-independent logic.
///     </para>
///     <para>
///         For fixed time step update methods such as <see cref="BehaviorComponent.OnFixedUpdate" /> and
///         <see cref="ICustomGameLoopStep.ProcessFixedUpdate" />, the constant time step is available via
///         the static <see cref="FixedDeltaTime" /> property.
///     </para>
/// </remarks>
public readonly record struct TimeStep
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TimeStep" /> struct with the specified delta time
    ///     and a <see cref="TimeScale" /> of <c>1.0</c>.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the previous frame.</param>
    public TimeStep(TimeSpan deltaTime) : this(deltaTime, 1.0)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TimeStep" /> struct with the specified unscaled
    ///     delta time and time scale.
    /// </summary>
    /// <param name="unscaledDeltaTime">The actual real time elapsed since the previous frame.</param>
    /// <param name="timeScale">
    ///     The time scale multiplier applied to <paramref name="unscaledDeltaTime" /> to produce
    ///     <see cref="DeltaTime" />.
    /// </param>
    public TimeStep(TimeSpan unscaledDeltaTime, double timeScale)
    {
        DeltaTime = unscaledDeltaTime * timeScale;
        UnscaledDeltaTime = unscaledDeltaTime;
        TimeScale = timeScale;
    }

    /// <summary>
    ///     Gets the scaled time elapsed since the previous frame. This is <see cref="UnscaledDeltaTime" />
    ///     multiplied by <see cref="TimeScale" />.
    /// </summary>
    /// <remarks>
    ///     Use <see cref="DeltaTime" /> (or <see cref="DeltaTimeSeconds" />) to advance game state in a
    ///     way that respects the current time scale. When <see cref="TimeScale" /> is <c>0.0</c>, this
    ///     value is zero, effectively pausing all time-based game logic that relies on it.
    /// </remarks>
    public TimeSpan DeltaTime { get; }

    /// <summary>
    ///     Gets the actual real time elapsed since the previous frame, unaffected by
    ///     <see cref="TimeScale" />.
    /// </summary>
    /// <remarks>
    ///     Use <see cref="UnscaledDeltaTime" /> (or <see cref="UnscaledDeltaTimeSeconds" />) for game
    ///     logic that should continue ticking at real time regardless of the current time scale, such as
    ///     UI animations or pause menu elements.
    /// </remarks>
    public TimeSpan UnscaledDeltaTime { get; }

    /// <summary>
    ///     Gets the time scale value that was active when this <see cref="TimeStep" /> was created.
    /// </summary>
    /// <remarks>
    ///     This is a snapshot of <see cref="ITimeSystem.TimeScale" /> at the moment the time step was
    ///     generated for the current frame.
    /// </remarks>
    public double TimeScale { get; }

    /// <summary>
    ///     Gets <see cref="DeltaTime" /> expressed in seconds as a <see cref="double" /> value. This is a
    ///     convenience equivalent of <see cref="TimeSpan.TotalSeconds" /> called on
    ///     <see cref="DeltaTime" />.
    /// </summary>
    public double DeltaTimeSeconds => DeltaTime.TotalSeconds;

    /// <summary>
    ///     Gets <see cref="UnscaledDeltaTime" /> expressed in seconds as a <see cref="double" /> value.
    ///     This is a convenience equivalent of <see cref="TimeSpan.TotalSeconds" /> called on
    ///     <see cref="UnscaledDeltaTime" />.
    /// </summary>
    public double UnscaledDeltaTimeSeconds => UnscaledDeltaTime.TotalSeconds;

    /// <summary>
    ///     Gets the constant time step used for fixed time step updates. This static property is
    ///     initialized from <see cref="CoreConfiguration.FixedUpdatesPerSecond" /> when the engine starts
    ///     up.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Use <see cref="FixedDeltaTime" /> (or <see cref="FixedDeltaTimeSeconds" />) within fixed
    ///         update methods such as <see cref="BehaviorComponent.OnFixedUpdate" /> and
    ///         <see cref="ICustomGameLoopStep.ProcessFixedUpdate" /> to advance deterministic simulations
    ///         by a constant time step.
    ///     </para>
    ///     <para>
    ///         Unlike <see cref="DeltaTime" />, this value is constant and not affected by
    ///         <see cref="ITimeSystem.TimeScale" /> at the per-call level. Instead,
    ///         <see cref="ITimeSystem.TimeScale" /> controls the frequency at which fixed update methods
    ///         are called per real second.
    ///     </para>
    /// </remarks>
    public static TimeSpan FixedDeltaTime { get; internal set; }

    /// <summary>
    ///     Gets <see cref="FixedDeltaTime" /> expressed in seconds as a <see cref="double" /> value. This
    ///     is a convenience equivalent of <see cref="TimeSpan.TotalSeconds" /> called on
    ///     <see cref="FixedDeltaTime" />.
    /// </summary>
    public static double FixedDeltaTimeSeconds => FixedDeltaTime.TotalSeconds;
}