using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal sealed class EllipseNode : RenderNode
    {
        public EllipseNode(Transform2DComponent transform, EllipseRendererComponent ellipseRendererComponent) : base(transform, ellipseRendererComponent)
        {
            EllipseRendererComponent = ellipseRendererComponent;
        }

        public EllipseRendererComponent EllipseRendererComponent { get; }

        public override AxisAlignedRectangle GetBoundingRectangle()
        {
            var transform = TransformHierarchy.Calculate2DTransformationMatrix(Entity);
            var quad = new Ellipse(EllipseRendererComponent.RadiusX, EllipseRendererComponent.RadiusY).GetBoundingRectangle().ToQuad();
            return quad.Transform(transform).GetBoundingRectangle();
        }

        public override void Accept(IRenderNodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}