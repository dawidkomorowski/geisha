using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal sealed class TextNode : RenderNode
    {
        private readonly TextRendererComponent _textRendererComponent;
        private readonly IRenderingContext2D _renderingContext2D;

        public TextNode(Transform2DComponent transform, TextRendererComponent textRendererComponent, IRenderingContext2D renderingContext2D)
            : base(transform, textRendererComponent)
        {
            _textRendererComponent = textRendererComponent;
            _renderingContext2D = renderingContext2D;

            TextLayout = _renderingContext2D.CreateTextLayout(string.Empty, "Consolas", FontSize.FromDips(10), 0, 0);
            textRendererComponent.TextNode = this;
        }

        public ITextLayout TextLayout { get; private set; }

        public string Text
        {
            get => TextLayout.Text;
            set
            {
                var newTextLayout = _renderingContext2D.CreateTextLayout(value, FontFamilyName, FontSize, MaxWidth, MaxHeight);
                newTextLayout.TextAlignment = TextAlignment;
                newTextLayout.ParagraphAlignment = ParagraphAlignment;
                newTextLayout.Pivot = Pivot;
                TextLayout.Dispose();
                TextLayout = newTextLayout;
            }
        }

        public string FontFamilyName
        {
            get => TextLayout.FontFamilyName;
            set => TextLayout.FontFamilyName = value;
        }

        public FontSize FontSize
        {
            get => TextLayout.FontSize;
            set => TextLayout.FontSize = value;
        }

        public double MaxWidth
        {
            get => TextLayout.MaxWidth;
            set => TextLayout.MaxWidth = value;
        }

        public double MaxHeight
        {
            get => TextLayout.MaxHeight;
            set => TextLayout.MaxHeight = value;
        }

        public TextAlignment TextAlignment
        {
            get => TextLayout.TextAlignment;
            set => TextLayout.TextAlignment = value;
        }

        public ParagraphAlignment ParagraphAlignment
        {
            get => TextLayout.ParagraphAlignment;
            set => TextLayout.ParagraphAlignment = value;
        }

        public Color Color { set; get; }

        public Vector2 Pivot
        {
            get => TextLayout.Pivot;
            set => TextLayout.Pivot = value;
        }

        public bool ClipToLayoutBox { get; set; }
        public TextMetrics Metrics => TextLayout.Metrics;
        public AxisAlignedRectangle LayoutRectangle => new((MaxWidth / 2d) - Pivot.X, Pivot.Y - (MaxHeight / 2d), MaxWidth, MaxHeight);

        public AxisAlignedRectangle TextRectangle
        {
            get
            {
                var metrics = Metrics;
                return new(
                    metrics.Left + (metrics.Width / 2d) - Pivot.X,
                    Pivot.Y - (metrics.Top + (metrics.Height / 2d)),
                    metrics.Width,
                    metrics.Height
                );
            }
        }

        public override void Accept(IRenderNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _textRendererComponent.TextNode = null;
                TextLayout.Dispose();
            }
        }
    }
}