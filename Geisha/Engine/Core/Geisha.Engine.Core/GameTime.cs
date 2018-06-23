using System;
using Geisha.Common;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core
{
    /// <summary>
    ///     Represents game time that is composed of delta time and provides other useful time related information.
    /// </summary>
    public struct GameTime : IEquatable<GameTime>
    {
        /// <summary>
        ///     Internal date and time provider used for calculating certain time information.
        /// </summary>
        /// <remarks>This is mainly for testability purposes.</remarks>
        internal static IDateTimeProvider DateTimeProvider { get; set; }

        /// <summary>
        ///     Time of engine start up.
        /// </summary>
        public static DateTime StartUpTime { get; internal set; }

        /// <summary>
        ///     Time span of fixed time step.
        /// </summary>
        /// <remarks>
        ///     This is amount of time that is processed in single <see cref="IFixedTimeStepSystem.FixedUpdate" /> for each
        ///     <see cref="IFixedTimeStepSystem" />. It is used for stability of certain systems like physical simulations.
        /// </remarks>
        public static TimeSpan FixedDeltaTime { get; internal set; }

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
        ///     Initializes new instance of <see cref="GameTime" /> structure given a delta time.
        /// </summary>
        /// <param name="deltaTime">Delta time of this <see cref="GameTime" />.</param>
        public GameTime(TimeSpan deltaTime)
        {
            DeltaTime = deltaTime;
        }

        /// <inheritdoc />
        public bool Equals(GameTime other)
        {
            return DeltaTime.Equals(other.DeltaTime);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            return obj is GameTime time && Equals(time);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return DeltaTime.GetHashCode();
        }

        /// <summary>
        ///     Indicates whether two <see cref="GameTime" /> instances are equal.
        /// </summary>
        /// <param name="left">The first game time to compare.</param>
        /// <param name="right">The second game time to compare.</param>
        /// <returns>true if the values of <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, false.</returns>
        public static bool operator ==(GameTime left, GameTime right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Indicates whether two <see cref="GameTime" /> instances are not equal.
        /// </summary>
        /// <param name="left">The first game time to compare.</param>
        /// <param name="right">The second game time to compare.</param>
        /// <returns>true if the values of <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(GameTime left, GameTime right)
        {
            return !left.Equals(right);
        }
    }
}