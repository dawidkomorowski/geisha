using System;

namespace Geisha.Engine.Core
{
    // TODO Add documentation.
    // TODO Add unit tests?
    // TODO Count number of frames?
    public struct GameTime : IEquatable<GameTime>
    {
        public static DateTime StartUpTime { get; internal set; }
        public static TimeSpan FixedDeltaTime { get; internal set; }
        public static TimeSpan TimeSinceStartUp => DateTime.Now - StartUpTime;
        public static int FramesSinceStartUp { get; internal set; } = 0;

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