using System;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Rendering.Backend
{
    // TODO Add documentation.
    public interface ITextLayout : IDisposable
    {
        public string Text { get; }
        public string FontFamilyName { get; set; }
        public FontSize FontSize { get; set; }
        public double MaxWidth { get; set; }
        public double MaxHeight { get; set; }
        public TextAlignment TextAlignment { get; set; }
        public ParagraphAlignment ParagraphAlignment { get; set; }
        public Vector2 Pivot { get; set; }
        public TextMetrics Metrics { get; }
    }

    // TODO Add documentation.
    public enum TextAlignment
    {
        Leading,
        Trailing,
        Center,
        Justified
    }

    // TODO Add documentation.
    public enum ParagraphAlignment
    {
        Near,
        Far,
        Center
    }

    // TODO Add documentation.
    // TODO Add equality and formatting members.
    public readonly struct TextMetrics
    {
        public double Left { get; init; }
        public double Top { get; init; }
        public double Width { get; init; }
        public double Height { get; init; }
        public double LayoutWidth { get; init; }
        public double LayoutHeight { get; init; }
        public int LineCount { get; init; }
    }
}