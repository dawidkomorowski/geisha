using System;
using Geisha.Common;

namespace Geisha.Engine.Core
{
    /// <summary>
    ///     Represents game time that is composed of delta time and provides other useful time related information.
    /// </summary>
    public readonly struct GameTime : IEquatable<GameTime>
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
        ///     This is amount of time that is processed in single fixed update for each fixed time step system. It is used for
        ///     stability of certain systems like physical simulations.
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

        #region Equality members

        /// <inheritdoc />
        public bool Equals(GameTime other) => DeltaTime.Equals(other.DeltaTime);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is GameTime other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => DeltaTime.GetHashCode();

        /// <summary>
        ///     Determines whether two specified instances of <see cref="GameTime" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="GameTime" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(GameTime left, GameTime right) => left.Equals(right);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="GameTime" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="GameTime" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(GameTime left, GameTime right) => !left.Equals(right);

        #endregion
    }
}