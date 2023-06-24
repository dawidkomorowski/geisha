using System;

namespace Geisha.Engine.Rendering
{
    /// <summary>
    ///     Contains the metrics associated with text after layout. All coordinates are in device independent pixels (DIPs).
    /// </summary>
    public readonly struct TextMetrics : IEquatable<TextMetrics>
    {
        /// <summary>
        ///     A value that indicates the left-most point of formatted text relative to the layout box.
        /// </summary>
        public double Left { get; init; }

        /// <summary>
        ///     A value that indicates the top-most point of formatted text relative to the layout box.
        /// </summary>
        public double Top { get; init; }

        /// <summary>
        ///     A value that indicates the width of the formatted text, while ignoring trailing whitespace at the end of each line.
        /// </summary>
        public double Width { get; init; }

        /// <summary>
        ///     The height of the formatted text. The height of an empty string is set to the same value as that of the default
        ///     font.
        /// </summary>
        public double Height { get; init; }

        /// <summary>
        ///     The initial width given to the layout. It can be either larger or smaller than the text content width, depending on
        ///     whether the text was wrapped.
        /// </summary>
        public double LayoutWidth { get; init; }

        /// <summary>
        ///     Initial height given to the layout. Depending on the length of the text, it may be larger or smaller than the text
        ///     content height.
        /// </summary>
        public double LayoutHeight { get; init; }

        /// <summary>
        ///     Total number of lines.
        /// </summary>
        public int LineCount { get; init; }

        /// <summary>
        ///     Converts the value of the current <see cref="TextMetrics" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="TextMetrics" /> object.</returns>
        public override string ToString() =>
            $"{nameof(Left)}: {Left}, {nameof(Top)}: {Top}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(LayoutWidth)}: {LayoutWidth}, {nameof(LayoutHeight)}: {LayoutHeight}, {nameof(LineCount)}: {LineCount}";


        /// <inheritdoc />
        public bool Equals(TextMetrics other) => Left.Equals(other.Left) && Top.Equals(other.Top) && Width.Equals(other.Width) && Height.Equals(other.Height) &&
                                                 LayoutWidth.Equals(other.LayoutWidth) && LayoutHeight.Equals(other.LayoutHeight) &&
                                                 LineCount == other.LineCount;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is TextMetrics other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Left, Top, Width, Height, LayoutWidth, LayoutHeight, LineCount);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="TextMetrics" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="TextMetrics" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(in TextMetrics left, in TextMetrics right) => left.Equals(right);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="TextMetrics" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="TextMetrics" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(in TextMetrics left, in TextMetrics right) => !left.Equals(right);
    }
}