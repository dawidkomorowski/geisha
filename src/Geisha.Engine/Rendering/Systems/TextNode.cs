using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal sealed class TextNode : RenderNode
    {
        private readonly IRenderingContext2D _renderingContext2D;

        public TextNode(Transform2DComponent transform, TextRendererComponent textRendererComponent, IRenderingContext2D renderingContext2D)
            : base(transform, textRendererComponent)
        {
            TextRendererComponent = textRendererComponent;
            _renderingContext2D = renderingContext2D;

            TextLayout = _renderingContext2D.CreateTextLayout(
                textRendererComponent.Text,
                textRendererComponent.FontFamilyName,
                textRendererComponent.FontSize,
                textRendererComponent.MaxWidth,
                textRendererComponent.MaxHeight
            );
            Color = textRendererComponent.Color;
            textRendererComponent.TextNode = this;
        }

        public TextRendererComponent TextRendererComponent { get; }
        public ITextLayout TextLayout { get; private set; }

        public string Text
        {
            get => TextLayout.Text;
            set
            {
                var newTextLayout = _renderingContext2D.CreateTextLayout(value, FontFamilyName, FontSize, MaxWidth, MaxHeight);
                newTextLayout.TextAlignment = TextAlignment;
                newTextLayout.ParagraphAlignment = ParagraphAlignment;
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
        public Vector2 Pivot { get; set; }
        public TextMetrics Metrics => TextLayout.Metrics;

        public override void Accept(IRenderNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                TextRendererComponent.TextNode = null;
                TextLayout.Dispose();
            }
        }
    }
}