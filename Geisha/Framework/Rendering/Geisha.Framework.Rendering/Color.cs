using System;

namespace Geisha.Framework.Rendering
{
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

        public byte A => (byte) ((_argb & AlphaMask) >> AlphaOffset);
        public byte R => (byte) ((_argb & RedMask) >> RedOffset);
        public byte G => (byte) ((_argb & GreenMask) >> GreenOffset);
        public byte B => (byte) ((_argb & BlueMask) >> BlueOffset);

        public double ScA => (double) A / MaxComponentValue;
        public double ScR => (double) R / MaxComponentValue;
        public double ScG => (double) G / MaxComponentValue;
        public double ScB => (double) B / MaxComponentValue;

        private Color(int argb)
        {
            _argb = argb;
        }

        public static Color FromArgb(int argb)
        {
            return new Color(argb);
        }

        public static Color FromArgb(int alpha, int red, int green, int blue)
        {
            alpha = Clamp(alpha) << AlphaOffset;
            red = Clamp(red) << RedOffset;
            green = Clamp(green) << GreenOffset;
            blue = Clamp(blue) << BlueOffset;

            var argb = alpha | red | green | blue;

            return new Color(argb);
        }

        public static Color FromArgb(double alpha, double red, double green, double blue)
        {
            return FromArgb(
                (int) (alpha * MaxComponentValue),
                (int) (red * MaxComponentValue),
                (int) (green * MaxComponentValue),
                (int) (blue * MaxComponentValue)
            );
        }

        public int ToArgb()
        {
            return _argb;
        }

        public bool Equals(Color other)
        {
            return _argb == other._argb;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Color other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _argb;
        }

        public override string ToString()
        {
            return $"{nameof(A)}: {A}, {nameof(R)}: {R}, {nameof(G)}: {G}, {nameof(B)}: {B}";
        }

        public static bool operator ==(Color left, Color right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Color left, Color right)
        {
            return !left.Equals(right);
        }

        private static int Clamp(int value) => value > MaxComponentValue ? MaxComponentValue : (value < MinComponentValue ? MinComponentValue : value);
    }
}