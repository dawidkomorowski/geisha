using System;

namespace Geisha.Common.Geometry
{
    public struct Vector2 : IVector<Vector2>
    {
        public static Vector2 Zero => new Vector2(0, 0);
        public static Vector2 One => new Vector2(1, 1);

        public double X { get; }
        public double Y { get; }
        public double Length => Math.Sqrt(X*X + Y*Y);
        public Vector2 Unit => new Vector2(X/Length, Y/Length);
        public Vector2 Opposite => new Vector2(-X, -Y);
        public double[] Array => new[] {X, Y};

        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector2(double[] array)
        {
            if (array.Length != 2)
            {
                throw new ArgumentException("Array must be the length of 2 elements.");
            }

            X = array[0];
            Y = array[1];
        }

        public Vector2 Add(Vector2 other)
        {
            return new Vector2(X + other.X, Y + other.Y);
        }

        public Vector2 Subtract(Vector2 other)
        {
            return new Vector2(X - other.X, Y - other.Y);
        }

        public Vector2 Multiply(double scalar)
        {
            return new Vector2(X*scalar, Y*scalar);
        }

        public Vector2 Divide(double scalar)
        {
            return new Vector2(X/scalar, Y/scalar);
        }

        public double Dot(Vector2 other)
        {
            return X*other.X + Y*other.Y;
        }

        public double Distance(Vector2 other)
        {
            return Subtract(other).Length;
        }

        public bool Equals(Vector2 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector2 && Equals((Vector2) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode()*397) ^ Y.GetHashCode();
            }
        }

        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return left.Add(right);
        }

        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return left.Subtract(right);
        }

        public static Vector2 operator *(Vector2 left, double right)
        {
            return left.Multiply(right);
        }

        public static Vector2 operator /(Vector2 left, double right)
        {
            return left.Divide(right);
        }

        public static Vector2 operator -(Vector2 right)
        {
            return right.Opposite;
        }

        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return !left.Equals(right);
        }
    }
}