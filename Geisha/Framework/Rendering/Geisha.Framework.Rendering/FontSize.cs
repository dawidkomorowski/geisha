using System;

namespace Geisha.Framework.Rendering
{
    // TODO Add unit tests
    // TODO Add docs
    public struct FontSize : IEquatable<FontSize>
    {
        public double Points { get; }
        public double Dips => Points / 72.0d * 96.0d;

        private FontSize(double points)
        {
            Points = points;
        }

        public static FontSize FromPoints(double points)
        {
            return new FontSize(points);
        }

        public static FontSize FromDips(double dips)
        {
            return new FontSize(dips / 96.0d * 72.0d);
        }

        public bool Equals(FontSize other)
        {
            return Points.Equals(other.Points);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is FontSize other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Points.GetHashCode();
        }

        public static bool operator ==(FontSize left, FontSize right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FontSize left, FontSize right)
        {
            return !left.Equals(right);
        }
    }
}