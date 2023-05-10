using Geisha.Engine.Core.Components;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal sealed class RectangleNode : RenderNode
    {
        public RectangleNode(Transform2DComponent transform, RectangleRendererComponent rectangleRendererComponent) : base(transform,
            rectangleRendererComponent)
        {
            RectangleRendererComponent = rectangleRendererComponent;
        }

        public RectangleRendererComponent RectangleRendererComponent { get; }

        public override void Accept(IRenderNodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}