using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
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

        public override AxisAlignedRectangle GetBoundingRectangle()
        {
            var transform = TransformHierarchy.Calculate2DTransformationMatrix(Entity);
            var quad = new AxisAlignedRectangle(RectangleRendererComponent.Dimension).ToQuad();
            return quad.Transform(transform).GetBoundingRectangle();
        }

        public override void Accept(IRenderNodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}