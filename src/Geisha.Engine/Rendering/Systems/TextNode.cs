﻿using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal interface ITextNode : IRenderNode
    {
        string Text { get; set; }
        string FontFamilyName { get; set; }
        FontSize FontSize { get; set; }
        double MaxWidth { get; set; }
        double MaxHeight { get; set; }
        TextAlignment TextAlignment { get; set; }
        ParagraphAlignment ParagraphAlignment { get; set; }
        Color Color { set; get; }
        Vector2 Pivot { get; set; }
        bool ClipToLayoutBox { get; set; }
        TextMetrics Metrics { get; }
        AxisAlignedRectangle LayoutRectangle { get; }
        AxisAlignedRectangle TextRectangle { get; }
    }

    internal sealed class DetachedTextNode : DetachedRenderNode, ITextNode
    {
        public string Text { get; set; } = string.Empty;
        public string FontFamilyName { get; set; } = string.Empty;
        public FontSize FontSize { get; set; }
        public double MaxWidth { get; set; }
        public double MaxHeight { get; set; }
        public TextAlignment TextAlignment { get; set; }
        public ParagraphAlignment ParagraphAlignment { get; set; }
        public Color Color { get; set; }
        public Vector2 Pivot { get; set; }
        public bool ClipToLayoutBox { get; set; }
        public TextMetrics Metrics => default;
        public AxisAlignedRectangle LayoutRectangle => default;
        public AxisAlignedRectangle TextRectangle => default;
    }

    internal sealed class TextNode : RenderNode, ITextNode
    {
        private readonly TextRendererComponent _textRendererComponent;
        private readonly IRenderingContext2D _renderingContext2D;

        public TextNode(Transform2DComponent transform, TextRendererComponent textRendererComponent, IRenderingContext2D renderingContext2D)
            : base(transform, textRendererComponent)
        {
            _textRendererComponent = textRendererComponent;
            _renderingContext2D = renderingContext2D;

            TextLayout = _renderingContext2D.CreateTextLayout(string.Empty, "Consolas", FontSize.FromDips(10), 0, 0);
            CopyData(textRendererComponent.TextNode, this);
            textRendererComponent.TextNode = this;
        }

        public ITextLayout TextLayout { get; private set; }

        #region Implementation of ITextNode

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
        public bool ClipToLayoutBox { get; set; }
        public TextMetrics Metrics => TextLayout.Metrics;
        public AxisAlignedRectangle LayoutRectangle => new((MaxWidth / 2d) - Pivot.X, Pivot.Y - (MaxHeight / 2d), MaxWidth, MaxHeight);

        public AxisAlignedRectangle TextRectangle
        {
            get
            {
                var metrics = Metrics;
                return new AxisAlignedRectangle(
                    metrics.Left + (metrics.Width / 2d) - Pivot.X,
                    Pivot.Y - (metrics.Top + (metrics.Height / 2d)),
                    metrics.Width,
                    metrics.Height
                );
            }
        }

        #endregion

        public override AxisAlignedRectangle GetBoundingRectangle()
        {
            var transform = TransformHierarchy.Calculate2DTransformationMatrix(Entity);
            var quad = TextRectangle.ToQuad();
            return quad.Transform(transform).GetBoundingRectangle();
        }

        public override void Accept(IRenderNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override bool ShouldSkipRendering() => base.ShouldSkipRendering() || string.IsNullOrWhiteSpace(Text);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                var detachedTextNode = new DetachedTextNode();
                CopyData(this, detachedTextNode);
                _textRendererComponent.TextNode = detachedTextNode;
                TextLayout.Dispose();
            }
        }

        protected override void CopyData(IRenderNode source, IRenderNode target)
        {
            base.CopyData(source, target);

            var sourceTextNode = (ITextNode)source;
            var targetTextNode = (ITextNode)target;

            targetTextNode.Text = sourceTextNode.Text;
            targetTextNode.FontFamilyName = sourceTextNode.FontFamilyName;
            targetTextNode.FontSize = sourceTextNode.FontSize;
            targetTextNode.MaxWidth = sourceTextNode.MaxWidth;
            targetTextNode.MaxHeight = sourceTextNode.MaxHeight;
            targetTextNode.TextAlignment = sourceTextNode.TextAlignment;
            targetTextNode.ParagraphAlignment = sourceTextNode.ParagraphAlignment;
            targetTextNode.Color = sourceTextNode.Color;
            targetTextNode.Pivot = sourceTextNode.Pivot;
            targetTextNode.ClipToLayoutBox = sourceTextNode.ClipToLayoutBox;
        }
    }
}