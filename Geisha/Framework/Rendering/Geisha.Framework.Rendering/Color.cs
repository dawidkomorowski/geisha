using System;

namespace Geisha.Framework.Rendering
{
    /// <summary>
    ///     Encapsulates 32-bit color as an ARGB integer value.
    /// </summary>
    public struct Color : IEquatable<Color>
    {
        private const int MaxComponentValue = 255;
        private const int MinComponentValue = 0;

        private const int AlphaMask = unchecked((int) 0xFF000000u);
        private const int RedMask = unchecked((int) 0x00FF0000u);
        private const int GreenMask = unchecked((int) 0x0000FF00u);
        private const int BlueMask = unchecked((int) 0x000000FFu);

        private const int AlphaOffset = 24;
        private const int RedOffset = 16;
        private const int GreenOffset = 8;
        private const int BlueOffset = 0;

        private readonly int _argb;

        /// <summary>
        ///     Alpha component of color.
        /// </summary>
        public byte A => (byte) ((_argb & AlphaMask) >> AlphaOffset);

        /// <summary>
        ///     Red component of color.
        /// </summary>
        public byte R => (byte) ((_argb & RedMask) >> RedOffset);

        /// <summary>
        ///     Green component of color.
        /// </summary>
        public byte G => (byte) ((_argb & GreenMask) >> GreenOffset);

        /// <summary>
        ///     Blue component of color.
        /// </summary>
        public byte B => (byte) ((_argb & BlueMask) >> BlueOffset);

        /// <summary>
        ///     Alpha component of color as double value between 0.0 to 1.0.
        /// </summary>
        public double DoubleA => (double) A / MaxComponentValue;

        /// <summary>
        ///     Red component of color as double value between 0.0 to 1.0.
        /// </summary>
        public double DoubleR => (double) R / MaxComponentValue;

        /// <summary>
        ///     Green component of color as double value between 0.0 to 1.0.
        /// </summary>
        public double DoubleG => (double) G / MaxComponentValue;

        /// <summary>
        ///     Blue component of color as double value between 0.0 to 1.0.
        /// </summary>
        public double DoubleB => (double) B / MaxComponentValue;

        private Color(int argb)
        {
            _argb = argb;
        }

        /// <summary>
        ///     Creates new instance of <see cref="Color" /> out of raw ARGB 32-bit integer.
        /// </summary>
        /// <param name="argb">32-bit ARGB color value.</param>
        /// <returns><see cref="Color" /> instance with specified color.</returns>
        public static Color FromArgb(int argb)
        {
            return new Color(argb);
        }

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
        public static Color FromArgb(double alpha, double red, double green, double blue)
        {
            return FromArgb(
                (int) (alpha * MaxComponentValue),
                (int) (red * MaxComponentValue),
                (int) (green * MaxComponentValue),
                (int) (blue * MaxComponentValue)
            );
        }

        /// <summary>
        ///     Returns ARGB color value as 32-bit integer.
        /// </summary>
        /// <returns>Integer consisting ARGB color value.</returns>
        public int ToArgb()
        {
            return _argb;
        }

        #region Equality members

        /// <inheritdoc />
        public bool Equals(Color other)
        {
            return _argb == other._argb;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Color other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _argb;
        }

        /// <summary>
        ///     Tests equality of two <see cref="Color" /> instances.
        /// </summary>
        /// <param name="left">First instance of <see cref="Color" />.</param>
        /// <param name="right">Second instance of <see cref="Color" />.</param>
        /// <returns>True, of both instances are equal; false otherwise.</returns>
        public static bool operator ==(Color left, Color right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Tests inequality of two <see cref="Color" /> instances.
        /// </summary>
        /// <param name="left">First instance of <see cref="Color" />.</param>
        /// <param name="right">Second instance of <see cref="Color" />.</param>
        /// <returns>True, if both instances are not equal; false otherwise.</returns>
        public static bool operator !=(Color left, Color right)
        {
            return !left.Equals(right);
        }

        #endregion

        /// <summary>
        ///     Returns textual representation of <see cref="Color" />.
        /// </summary>
        /// <returns>String containing information about all four components.</returns>
        public override string ToString()
        {
            return $"{nameof(A)}: {A}, {nameof(R)}: {R}, {nameof(G)}: {G}, {nameof(B)}: {B}";
        }

        private static int Clamp(int value)
        {
            return value > MaxComponentValue ? MaxComponentValue : (value < MinComponentValue ? MinComponentValue : value);
        }
    }
}