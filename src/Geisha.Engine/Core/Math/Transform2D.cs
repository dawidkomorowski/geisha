using System;

namespace Geisha.Engine.Core.Math
{
    /// <summary>
    ///     Represents 2D geometrical transformation composed of translation, rotation and scale.
    /// </summary>
    public readonly struct Transform2D : IEquatable<Transform2D>
    {
        /// <summary>
        ///     Creates new instance of <see cref="Transform2D" /> with specified <paramref name="translation" />,
        ///     <paramref name="rotation" /> and <paramref name="scale" />.
        /// </summary>
        /// <param name="translation">Translation component of <see cref="Transform2D" />.</param>
        /// <param name="rotation">Rotation component of <see cref="Transform2D" />.</param>
        /// <param name="scale">Scale component of <see cref="Transform2D" />.</param>
        public Transform2D(in Vector2 translation, double rotation, in Vector2 scale)
        {
            Translation = translation;
            Rotation = rotation;
            Scale = scale;
        }

        /// <summary>
        ///     Returns <see cref="Transform2D" /> representing identity transformation. It is composed of zero translation, zero
        ///     rotation and scale of one.
        /// </summary>
        public static Transform2D Identity { get; } = new(Vector2.Zero, 0, Vector2.One);

        /// <summary>
        ///     Translation along X and Y axes from the origin of the coordinate system.
        /// </summary>
        public Vector2 Translation { get; init; }

        /// <summary>
        ///     Counterclockwise rotation in radians around the origin of the coordinate system.
        /// </summary>
        public double Rotation { get; init; }

        /// <summary>
        ///     Scale along X and Y axes of the local coordinate system.
        /// </summary>
        public Vector2 Scale { get; init; }

        /// <summary>
        ///     Unit vector in global coordinate system pointing along X axis of coordinate system defined by this
        ///     <see cref="Transform2D" />.
        /// </summary>
        public Vector2 VectorX => (Matrix3x3.CreateRotation(Rotation) * Vector2.UnitX.Homogeneous).ToVector2();

        /// <summary>
        ///     Unit vector in global coordinate system pointing along Y axis of coordinate system defined by this
        ///     <see cref="Transform2D" />.
        /// </summary>
        public Vector2 VectorY => (Matrix3x3.CreateRotation(Rotation) * Vector2.UnitY.Homogeneous).ToVector2();

        /// <summary>
        ///     Linearly interpolates from <see cref="Transform2D" /> <paramref name="t1" /> to <see cref="Transform2D" />
        ///     <paramref name="t2" /> proportionally to factor <paramref name="alpha" />.
        /// </summary>
        /// <param name="t1">Source value for <see cref="Transform2D" /> interpolation.</param>
        /// <param name="t2">Target value for <see cref="Transform2D" /> interpolation.</param>
        /// <param name="alpha">Interpolation factor in range from <c>0.0</c> to <c>1.0</c>.</param>
        /// <returns>Interpolated value of <see cref="Transform2D" />.</returns>
        /// <remarks>
        ///     <p>
        ///         When <paramref name="alpha" /> value is <c>0.0</c> the returned value is equal to <paramref name="t1" />. When
        ///         <paramref name="alpha" /> value is <c>1.0</c> the returned value is equal to <paramref name="t2" />.
        ///     </p>
        ///     <p>
        ///         <see cref="Transform2D" /> interpolation is made by respectively interpolating Translation, Rotation and Scale
        ///         components.
        ///     </p>
        /// </remarks>
        public static Transform2D Lerp(in Transform2D t1, in Transform2D t2, double alpha) =>
            new()
            {
                Translation = Vector2.Lerp(t1.Translation, t2.Translation, alpha),
                Rotation = GMath.Lerp(t1.Rotation, t2.Rotation, alpha),
                Scale = Vector2.Lerp(t1.Scale, t2.Scale, alpha)
            };

        /// <summary>
        ///     Creates 2D transformation matrix representing this <see cref="Transform2D" />.
        /// </summary>
        /// <returns><see cref="Matrix3x3" /> representing this <see cref="Transform2D" />.</returns>
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