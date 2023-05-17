using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Backend;
using System;
using SharpDX.DirectWrite;
using ParagraphAlignment = Geisha.Engine.Rendering.ParagraphAlignment;
using TextAlignment = Geisha.Engine.Rendering.TextAlignment;
using TextMetrics = Geisha.Engine.Rendering.TextMetrics;

namespace Geisha.Engine.Rendering.DirectX
{
    internal sealed class TextLayout : ITextLayout
    {
        private readonly TextFormat _dwTextFormat;

        // ReSharper disable once InconsistentNaming
        internal readonly SharpDX.DirectWrite.TextLayout DWTextLayout;
        private bool _isDisposed;

        public TextLayout(TextFormat dwTextFormat, SharpDX.DirectWrite.TextLayout dwTextLayout, string text)
        {
            _dwTextFormat = dwTextFormat;
            DWTextLayout = dwTextLayout;
            Text = text;
        }

        // Text.Length + 1 guarantees that properties of layout will be applied for empty Text.
        private TextRange FullRange => new(0, Text.Length + 1);

        public string Text { get; }

        public string FontFamilyName
        {
            get
            {
                ThrowIfDisposed();
                return DWTextLayout.GetFontFamilyName(0);
            }
            set
            {
                ThrowIfDisposed();
                DWTextLayout.SetFontFamilyName(value, FullRange);
            }
        }

        public FontSize FontSize
        {
            get
            {
                ThrowIfDisposed();
                return FontSize.FromDips(DWTextLayout.GetFontSize(0));
            }
            set
            {
                ThrowIfDisposed();
                DWTextLayout.SetFontSize((float)value.Dips, FullRange);
            }
        }

        public double MaxWidth
        {
            get
            {
                ThrowIfDisposed();
                return DWTextLayout.MaxWidth;
            }
            set
            {
                ThrowIfDisposed();
                DWTextLayout.MaxWidth = (float)value;
            }
        }

        public double MaxHeight
        {
            get
            {
                ThrowIfDisposed();
                return DWTextLayout.MaxHeight;
            }
            set
            {
                ThrowIfDisposed();
                DWTextLayout.MaxHeight = (float)value;
            }
        }

        public TextAlignment TextAlignment
        {
            get
            {
                ThrowIfDisposed();
                return DWTextLayout.TextAlignment switch
                {
                    SharpDX.DirectWrite.TextAlignment.Leading => TextAlignment.Leading,
                    SharpDX.DirectWrite.TextAlignment.Trailing => TextAlignment.Trailing,
                    SharpDX.DirectWrite.TextAlignment.Center => TextAlignment.Center,
                    SharpDX.DirectWrite.TextAlignment.Justified => TextAlignment.Justified,
                    _ => throw new ArgumentOutOfRangeException(nameof(DWTextLayout.TextAlignment), DWTextLayout.TextAlignment, null)
                };
            }
            set
            {
                ThrowIfDisposed();
                DWTextLayout.TextAlignment = value switch
                {
                    TextAlignment.Leading => SharpDX.DirectWrite.TextAlignment.Leading,
                    TextAlignment.Trailing => SharpDX.DirectWrite.TextAlignment.Trailing,
                    TextAlignment.Center => SharpDX.DirectWrite.TextAlignment.Center,
                    TextAlignment.Justified => SharpDX.DirectWrite.TextAlignment.Justified,
                    _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
                };
            }
        }

        public ParagraphAlignment ParagraphAlignment
        {
            get
            {
                ThrowIfDisposed();
                return DWTextLayout.ParagraphAlignment switch
                {
                    SharpDX.DirectWrite.ParagraphAlignment.Near => ParagraphAlignment.Near,
                    SharpDX.DirectWrite.ParagraphAlignment.Far => ParagraphAlignment.Far,
                    SharpDX.DirectWrite.ParagraphAlignment.Center => ParagraphAlignment.Center,
                    _ => throw new ArgumentOutOfRangeException(nameof(DWTextLayout.ParagraphAlignment), DWTextLayout.ParagraphAlignment, null)
                };
            }
            set
            {
                ThrowIfDisposed();
                DWTextLayout.ParagraphAlignment = value switch
                {
                    ParagraphAlignment.Near => SharpDX.DirectWrite.ParagraphAlignment.Near,
                    ParagraphAlignment.Far => SharpDX.DirectWrite.ParagraphAlignment.Far,
                    ParagraphAlignment.Center => SharpDX.DirectWrite.ParagraphAlignment.Center,
                    _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
                };
            }
        }

        public Vector2 Pivot { get; set; }

        public TextMetrics Metrics
        {
            get
            {
                ThrowIfDisposed();
                var dwMetrics = DWTextLayout.Metrics;
                return new TextMetrics
                {
                    Left = dwMetrics.Left,
                    Top = dwMetrics.Top,
                    Width = dwMetrics.Width,
                    Height = dwMetrics.Height,
                    LayoutWidth = dwMetrics.LayoutWidth,
                    LayoutHeight = dwMetrics.LayoutHeight,
                    LineCount = dwMetrics.LineCount
                };
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            DWTextLayout.Dispose();
            _dwTextFormat.Dispose();
            _isDisposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(TextLayout));
        }
    }
}