using System;

namespace Geisha.Engine.Core;

// TODO: Demo application needs fixing input after moving input processing out of fixed update.
// TODO: Demo application should use V-Sync to avoid unnecessary CPU/GPU usage.
// TODO: Profile input system as it now runs each frame - not only on simulation frames.

/// <summary>
///     Represents game time that is composed of delta time and provides other useful time related information.
/// </summary>
public readonly record struct GameTime
{
    /// <summary>
    ///     Internal date and time provider used for calculating certain time information.
    /// </summary>
    /// <remarks>This is mainly for testability purposes.</remarks>
    internal static IDateTimeProvider DateTimeProvider { get; set; } = new DateTimeProvider();

    /// <summary>
    ///     Time of engine start up.
    /// </summary>
    public static DateTime StartUpTime { get; internal set; }

    /// <summary>
    ///     Time span of fixed time step.
    /// </summary>
    /// <remarks>
    ///     This is amount of time that is processed in single fixed update for each system using fixed time step updates. It
    ///     is used for deterministic behavior and stability of certain systems like physical simulations.
    /// </remarks>
    public static TimeSpan FixedDeltaTime { get; internal set; }

    /// <summary>
    ///     Duration in seconds of fixed time step.
    /// </summary>
    /// <remarks>
    ///     This is amount of time that is processed in single fixed update for each system using fixed time step updates. It
    ///     is used for deterministic behavior and stability of certain systems like physical simulations.
    /// </remarks>
    public static double FixedDeltaTimeSeconds => FixedDeltaTime.TotalSeconds;

    /// <summary>
    ///     Time that has passed since engine start up till now.
    /// </summary>
    public static TimeSpan TimeSinceStartUp => DateTimeProvider.Now() - StartUpTime;

    /// <summary>
    ///     Number of frames executed since engine start up.
    /// </summary>
    public static int FramesSinceStartUp { get; internal set; }

    /// <summary>
    ///     Time that has passed since previous frame. It is a time span of variable time step.
    /// </summary>
    public TimeSpan DeltaTime { get; }

    /// <summary>
    ///     Time that has passed since previous frame in seconds. It is duration in seconds of variable time step.
    /// </summary>
    public double DeltaTimeSeconds => DeltaTime.TotalSeconds;

    /// <summary>
    ///     Initializes new instance of <see cref="GameTime" /> structure given a delta time.
    /// </summary>
    /// <param name="deltaTime">Delta time of this <see cref="GameTime" />.</param>
    public GameTime(TimeSpan deltaTime)
    {
        DeltaTime = deltaTime;
    }
}