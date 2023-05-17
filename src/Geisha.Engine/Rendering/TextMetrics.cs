using System;

namespace Geisha.Engine.Rendering
{
    // TODO Add documentation.
    public readonly struct TextMetrics : IEquatable<TextMetrics>
    {
        public double Left { get; init; }
        public double Top { get; init; }
        public double Width { get; init; }
        public double Height { get; init; }
        public double LayoutWidth { get; init; }
        public double LayoutHeight { get; init; }
        public int LineCount { get; init; }

        public override string ToString() =>
            $"{nameof(Left)}: {Left}, {nameof(Top)}: {Top}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(LayoutWidth)}: {LayoutWidth}, {nameof(LayoutHeight)}: {LayoutHeight}, {nameof(LineCount)}: {LineCount}";

        public bool Equals(TextMetrics other) => Left.Equals(other.Left) && Top.Equals(other.Top) && Width.Equals(other.Width) && Height.Equals(other.Height) &&
                                                 LayoutWidth.Equals(other.LayoutWidth) && LayoutHeight.Equals(other.LayoutHeight) &&
                                                 LineCount == other.LineCount;

        public override bool Equals(object? obj) => obj is TextMetrics other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Left, Top, Width, Height, LayoutWidth, LayoutHeight, LineCount);
        public static bool operator ==(in TextMetrics left, in TextMetrics right) => left.Equals(right);
        public static bool operator !=(in TextMetrics left, in TextMetrics right) => !left.Equals(right);
    }
}