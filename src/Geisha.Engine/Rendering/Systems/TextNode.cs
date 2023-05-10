using Geisha.Engine.Core.Components;
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

            TextLayout = _renderingContext2D.CreateTextLayout(textRendererComponent.Text, "Consolas", textRendererComponent.FontSize, 500, 500);
        }

        public TextRendererComponent TextRendererComponent { get; }
        public ITextLayout TextLayout { get; private set; }

        public override void Accept(IRenderNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                TextLayout.Dispose();
            }
        }
    }
}