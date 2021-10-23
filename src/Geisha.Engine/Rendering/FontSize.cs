using System;

namespace Geisha.Engine.Rendering
{
    /// <summary>
    ///     Represents font size encapsulating particular value together with unit.
    /// </summary>
    public readonly struct FontSize : IEquatable<FontSize>
    {
        /// <summary>
        ///     Font size in points unit.
        /// </summary>
        public double Points { get; }

        /// <summary>
        ///     Font size in device-independent pixels unit.
        /// </summary>
        public double Dips => Points / 72.0d * 96.0d; // TODO Is this calculation correct? Shouldn't DPI be taken from OS?

        private FontSize(double points)
        {
            Points = points;
        }

        /// <summary>
        ///     Creates new instance of <see cref="FontSize" /> with size given in points.
        /// </summary>
        /// <param name="points">Size of the font in points.</param>
        /// <returns><see cref="FontSize" /> instance with specified size.</returns>
        public static FontSize FromPoints(double points) => new FontSize(points);

        /// <summary>
        ///     Creates new instance of <see cref="FontSize" /> with size given in device-independent pixels.
        /// </summary>
        /// <param name="dips">Size of the font in device-independent pixels.</param>
        /// <returns><see cref="FontSize" /> instance with specified size.</returns>
        public static FontSize FromDips(double dips) => new FontSize(dips / 96.0d * 72.0d); // TODO Is this calculation correct? Shouldn't DPI be taken from OS?

        #region Equality members

        /// <inheritdoc />
        public bool Equals(FontSize other) => Points.Equals(other.Points);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is FontSize other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Points.GetHashCode();

        /// <summary>
        ///     Determines whether two specified instances of <see cref="FontSize" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="FontSize" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(FontSize left, FontSize right) => left.Equals(right);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="FontSize" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="FontSize" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(FontSize left, FontSize right) => !left.Equals(right);

        #endregion

        /// <summary>
        ///     Converts the value of the current <see cref="FontSize" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="FontSize" /> object.</returns>
        public override string ToString() => $"{nameof(Points)}: {Points}, {nameof(Dips)}: {Dips}";
    }
}