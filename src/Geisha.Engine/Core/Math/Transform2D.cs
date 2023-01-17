using System;

namespace Geisha.Engine.Core.Math
{
    // TODO Add documentation.
    public readonly struct Transform2D : IEquatable<Transform2D>
    {
        public Transform2D(in Vector2 translation, double rotation, in Vector2 scale)
        {
            Translation = translation;
            Rotation = rotation;
            Scale = scale;
        }

        public static Transform2D Identity { get; } = new(Vector2.Zero, 0, Vector2.One);

        public Vector2 Translation { get; init; }
        public double Rotation { get; init; }
        public Vector2 Scale { get; init; }

        public Vector2 VectorX => (Matrix3x3.CreateRotation(Rotation) * Vector2.UnitX.Homogeneous).ToVector2();
        public Vector2 VectorY => (Matrix3x3.CreateRotation(Rotation) * Vector2.UnitY.Homogeneous).ToVector2();

        public static Transform2D Lerp(in Transform2D t1, in Transform2D t2, double alpha) =>
            new()
            {
                Translation = Vector2.Lerp(t1.Translation, t2.Translation, alpha),
                Rotation = GMath.Lerp(t1.Rotation, t2.Rotation, alpha),
                Scale = Vector2.Lerp(t1.Scale, t2.Scale, alpha)
            };

        public Matrix3x3 ToMatrix() => Matrix3x3.CreateTRS(Translation, Rotation, Scale);

        /// <summary>
        ///     Converts the value of the current <see cref="Transform2D" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Transform2D" /> object.</returns>
        public override string ToString()
        {
            return $"{nameof(Translation)}: {Translation}, {nameof(Rotation)}: {Rotation}, {nameof(Scale)}: {Scale}";
        }

        /// <inheritdoc />
        public bool Equals(Transform2D other)
        {
            return Translation.Equals(other.Translation) && Rotation.Equals(other.Rotation) && Scale.Equals(other.Scale);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Transform2D other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Translation, Rotation, Scale);
        }

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Transform2D" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="Transform2D" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(in Transform2D left, in Transform2D right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Transform2D" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="Transform2D" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(in Transform2D left, in Transform2D right)
        {
            return !left.Equals(right);
        }
    }
}