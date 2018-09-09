using System;

namespace Geisha.Framework.Rendering
{
    /// <summary>
    ///     Represents font size encapsulating particular value together with unit.
    /// </summary>
    public struct FontSize : IEquatable<FontSize>
    {
        /// <summary>
        ///     Font size in points unit.
        /// </summary>
        public double Points { get; }

        /// <summary>
        ///     Font size in device-independent pixels unit.
        /// </summary>
        public double Dips => Points / 72.0d * 96.0d;

        private FontSize(double points)
        {
            Points = points;
        }

        /// <summary>
        ///     Creates new instance of <see cref="FontSize" /> with size given in points.
        /// </summary>
        /// <param name="points">Size of the font in points.</param>
        /// <returns><see cref="FontSize" /> instance with specified size.</returns>
        public static FontSize FromPoints(double points)
        {
            return new FontSize(points);
        }

        /// <summary>
        ///     Creates new instance of <see cref="FontSize" /> with size given in device-independent pixels.
        /// </summary>
        /// <param name="dips">Size of the font in device-independent pixels.</param>
        /// <returns><see cref="FontSize" /> instance with specified size.</returns>
        public static FontSize FromDips(double dips)
        {
            return new FontSize(dips / 96.0d * 72.0d);
        }

        #region Equality members

        /// <inheritdoc />
        public bool Equals(FontSize other)
        {
            return Points.Equals(other.Points);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is FontSize other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Points.GetHashCode();
        }

        /// <summary>
        ///     Tests equality of two <see cref="FontSize" /> instances.
        /// </summary>
        /// <param name="left">First instance of <see cref="FontSize" />.</param>
        /// <param name="right">Second instance of <see cref="FontSize" />.</param>
        /// <returns>True, if both instances are equal; false otherwise.</returns>
        public static bool operator ==(FontSize left, FontSize right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Tests inequality of two <see cref="FontSize" /> instances.
        /// </summary>
        /// <param name="left">First instance of <see cref="FontSize" />.</param>
        /// <param name="right">Second instance of <see cref="FontSize" />.</param>
        /// <returns>True, if both instances are not equal; false otherwise.</returns>
        public static bool operator !=(FontSize left, FontSize right)
        {
            return !left.Equals(right);
        }

        #endregion

        /// <summary>
        ///     Returns textual representation of <see cref="FontSize" />.
        /// </summary>
        /// <returns>String containing information about about <see cref="Points" /> and <see cref="Dips" />.</returns>
        public override string ToString()
        {
            return $"{nameof(Points)}: {Points}, {nameof(Dips)}: {Dips}";
        }
    }
}