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
}