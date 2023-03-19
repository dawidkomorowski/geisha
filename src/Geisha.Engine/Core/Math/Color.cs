using System;

namespace Geisha.Engine.Core.Math
{
    /// <summary>
    ///     Encapsulates 32-bit color as an ARGB integer value.
    /// </summary>
    public readonly struct Color : IEquatable<Color>
    {
        private const int MaxComponentValue = 255;
        private const int MinComponentValue = 0;

        private const int AlphaMask = unchecked((int)0xFF000000u);
        private const int RedMask = unchecked((int)0x00FF0000u);
        private const int GreenMask = unchecked((int)0x0000FF00u);
        private const int BlueMask = unchecked((int)0x000000FFu);

        private const int AlphaOffset = 24;
        private const int RedOffset = 16;
        private const int GreenOffset = 8;
        private const int BlueOffset = 0;

        private readonly int _argb;

        /// <summary>
        ///     Alpha component of color.
        /// </summary>
        public byte A => (byte)((_argb & AlphaMask) >> AlphaOffset);

        /// <summary>
        ///     Red component of color.
        /// </summary>
        public byte R => (byte)((_argb & RedMask) >> RedOffset);

        /// <summary>
        ///     Green component of color.
        /// </summary>
        public byte G => (byte)((_argb & GreenMask) >> GreenOffset);

        /// <summary>
        ///     Blue component of color.
        /// </summary>
        public byte B => (byte)((_argb & BlueMask) >> BlueOffset);

        /// <summary>
        ///     Alpha component of color as double value between 0.0 to 1.0.
        /// </summary>
        public double DoubleA => (double)A / MaxComponentValue;

        /// <summary>
        ///     Red component of color as double value between 0.0 to 1.0.
        /// </summary>
        public double DoubleR => (double)R / MaxComponentValue;

        /// <summary>
        ///     Green component of color as double value between 0.0 to 1.0.
        /// </summary>
        public double DoubleG => (double)G / MaxComponentValue;

        /// <summary>
        ///     Blue component of color as double value between 0.0 to 1.0.
        /// </summary>
        public double DoubleB => (double)B / MaxComponentValue;

        private Color(int argb)
        {
            _argb = argb;
        }

        /// <summary>
        ///     Creates new instance of <see cref="Color" /> out of raw ARGB 32-bit integer.
        /// </summary>
        /// <param name="argb">32-bit ARGB color value.</param>
        /// <returns><see cref="Color" /> instance with specified color.</returns>
        public static Color FromArgb(int argb) => new(argb);

        /// <summary>
        ///     Creates new instance of <see cref="Color" /> given ARGB components. Component value should be between 0 and 255.
        /// </summary>
        /// <param name="alpha">Alpha component value.</param>
        /// <param name="red">Red component value.</param>
        /// <param name="green">Green component value.</param>
        /// <param name="blue">Blue component value.</param>
        /// <returns><see cref="Color" /> instance with specified color components.</returns>
        /// <remarks>
        ///     If component value is outside 0 and 255 range then it is clamped that is values below 0 are treated as 0 and
        ///     values above 255 are treated as 255.
        /// </remarks>
        public static Color FromArgb(int alpha, int red, int green, int blue)
        {
            alpha = Clamp(alpha) << AlphaOffset;
            red = Clamp(red) << RedOffset;
            green = Clamp(green) << GreenOffset;
            blue = Clamp(blue) << BlueOffset;

            var argb = alpha | red | green | blue;

            return new Color(argb);
        }

        /// <summary>
        ///     Creates new instance of <see cref="Color" /> given ARGB components. Component value should be between 0.0 and 1.0.
        /// </summary>
        /// <param name="alpha">Alpha component value.</param>
        /// <param name="red">Red component value.</param>
        /// <param name="green">Green component value.</param>
        /// <param name="blue">Blue component value.</param>
        /// <returns><see cref="Color" /> instance with specified color components.</returns>
        /// <remarks>
        ///     If component value is outside 0.0 and 1.0 range then it is clamped that is values below 0.0 are treated as 0.0
        ///     and values above 1.0 are treated as 1.0.
        /// </remarks>
        public static Color FromArgb(double alpha, double red, double green, double blue) =>
            FromArgb(
                (int)(alpha * MaxComponentValue),
                (int)(red * MaxComponentValue),
                (int)(green * MaxComponentValue),
                (int)(blue * MaxComponentValue)
            );

        /// <summary>
        ///     Linearly interpolates from <see cref="Color" /> <paramref name="c1" /> to <see cref="Color" />
        ///     <paramref name="c2" /> proportionally to factor <paramref name="alpha" />.
        /// </summary>
        /// <param name="c1">Source value for <see cref="Color" /> interpolation.</param>
        /// <param name="c2">Target value for <see cref="Color" /> interpolation.</param>
        /// <param name="alpha">Interpolation factor in range from <c>0.0</c> to <c>1.0</c>.</param>
        /// <returns>Interpolated value of <see cref="Color" />.</returns>
        /// <remarks>
        ///     <para>
        ///         When <paramref name="alpha" /> value is <c>0.0</c> the returned value is equal to <paramref name="c1" />. When
        ///         <paramref name="alpha" /> value is <c>1.0</c> the returned value is equal to <paramref name="c2" />.
        ///     </para>
        ///     <para>
        ///         <see cref="Color" /> interpolation is made by respectively interpolating Alpha, Red, Green and Blue components.
        ///     </para>
        /// </remarks>
        public static Color Lerp(Color c1, Color c2, double alpha) =>
            FromArgb(
                GMath.Lerp(c1.DoubleA, c2.DoubleA, alpha),
                GMath.Lerp(c1.DoubleR, c2.DoubleR, alpha),
                GMath.Lerp(c1.DoubleG, c2.DoubleG, alpha),
                GMath.Lerp(c1.DoubleB, c2.DoubleB, alpha)
            );

        /// <summary>
        ///     Returns ARGB color value as 32-bit integer.
        /// </summary>
        /// <returns>Integer consisting ARGB color value.</returns>
        public int ToArgb() => _argb;

        #region Equality members

        /// <inheritdoc />
        public bool Equals(Color other) => _argb == other._argb;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Color other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => _argb;

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Color" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="Color" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Color left, Color right) => left.Equals(right);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Color" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="Color" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Color left, Color right) => !left.Equals(right);

        #endregion

        /// <summary>
        ///     Converts the value of the current <see cref="Color" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Color" /> object.</returns>
        public override string ToString() => $"{nameof(A)}: {A}, {nameof(R)}: {R}, {nameof(G)}: {G}, {nameof(B)}: {B}";

        private static int Clamp(int value) => value > MaxComponentValue ? MaxComponentValue : value < MinComponentValue ? MinComponentValue : value;
    }
}