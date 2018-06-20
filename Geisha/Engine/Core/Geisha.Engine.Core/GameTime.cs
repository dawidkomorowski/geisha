using System;
using Geisha.Common;

namespace Geisha.Engine.Core
{
    // TODO Add documentation.
    public struct GameTime : IEquatable<GameTime>
    {
        internal static IDateTimeProvider DateTimeProvider { get; set; }

        public static DateTime StartUpTime { get; internal set; }
        public static TimeSpan FixedDeltaTime { get; internal set; }
        public static TimeSpan TimeSinceStartUp => DateTimeProvider.Now() - StartUpTime;
        public static int FramesSinceStartUp { get; internal set; }

        public TimeSpan DeltaTime { get; }

        public GameTime(TimeSpan deltaTime)
        {
            DeltaTime = deltaTime;
        }

        public bool Equals(GameTime other)
        {
            return DeltaTime.Equals(other.DeltaTime);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            return obj is GameTime time && Equals(time);
        }

        public override int GetHashCode()
        {
            return DeltaTime.GetHashCode();
        }

        public static bool operator ==(GameTime left, GameTime right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GameTime left, GameTime right)
        {
            return !left.Equals(right);
        }
    }
}