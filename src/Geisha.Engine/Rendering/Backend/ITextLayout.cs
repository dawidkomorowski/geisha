using System;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Rendering.Backend
{
    // TODO Add documentation.
    public interface ITextLayout : IDisposable
    {
        public string Text { get; }
        public string FontFamilyName { get; }
        public FontSize FontSize { get; }
        public double MaxWidth { get; set; }
        public double MaxHeight { get; set; }
        public TextAlignment TextAlignment { get; set; }
        public ParagraphAlignment ParagraphAlignment { get; set; }
        public Vector2 Pivot { get; set; }
        public TextMetrics Metrics { get; }
    }

    // TODO
    public enum TextAlignment
    {
        Leading,
        Trailing,
        Center,
        Justified
    }

    // TODO
    public enum ParagraphAlignment
    {
        Near,
        Far,
        Center
    }

    // TODO
    public readonly struct TextMetrics
    {
        public double Left { get; }
        public double Top { get; }
        public double Width { get; }
        public double Height { get; }
        public double LayoutWidth { get; }
        public double LayoutHeight { get; }
        public int LineCount { get; }
    }
}