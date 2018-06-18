﻿using System;

namespace Geisha.Engine.Core
{
    // TODO Add documentation.
    public struct GameTime : IEquatable<GameTime>
    {
        public static DateTime StartUpTime { get; internal set; }
        public static TimeSpan FixedDeltaTime { get; internal set; }
        public static TimeSpan TimeSinceStartUp => DateTime.Now - StartUpTime;

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