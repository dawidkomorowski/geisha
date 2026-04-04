using System;

namespace Geisha.Engine.Core;

/// <summary>
///     Represents game time that is composed of delta time and provides other useful time related information.
/// </summary>
public readonly record struct GameTime
{
    /// <summary>
    ///     Time span of fixed time step.
    /// </summary>
    /// <remarks>
    ///     This is amount of time that is processed in single fixed update for each system using fixed time step updates. It
    ///     is used for deterministic behavior and stability of certain systems like physical simulations.
    /// </remarks>
    public static TimeSpan FixedDeltaTime { get; internal set; }

    /// <summary>
    ///     Time that has passed since previous frame. It is a time span of variable time step.
    /// </summary>
    public TimeSpan DeltaTime { get; init; }

    /// <summary>
    ///     Initializes new instance of <see cref="GameTime" /> structure given a delta time.
    /// </summary>
    /// <param name="deltaTime">Delta time of this <see cref="GameTime" />.</param>
    public GameTime(TimeSpan deltaTime)
    {
        DeltaTime = deltaTime;
    }
}