using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal interface IEllipseNode : IRenderNode
    {
        double RadiusX { get; set; }
        double RadiusY { get; set; }
        Color Color { get; set; }
        bool FillInterior { get; set; }
    }

    internal sealed class DetachedEllipseNode : DetachedRenderNode, IEllipseNode
    {
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }
        public Color Color { get; set; }
        public bool FillInterior { get; set; }
    }

    internal sealed class EllipseNode : RenderNode, IEllipseNode
    {
        private readonly EllipseRendererComponent _ellipseRendererComponent;

        public EllipseNode(Transform2DComponent transform, EllipseRendererComponent ellipseRendererComponent)
            : base(transform, ellipseRendererComponent)
        {
            _ellipseRendererComponent = ellipseRendererComponent;
            CopyData(_ellipseRendererComponent.EllipseNode, this);
            _ellipseRendererComponent.EllipseNode = this;
        }

        public override AxisAlignedRectangle GetBoundingRectangle()
        {
            var transform = TransformHierarchy.Calculate2DTransformationMatrix(Entity);
            var quad = new Ellipse(_ellipseRendererComponent.RadiusX, _ellipseRendererComponent.RadiusY).GetBoundingRectangle().ToQuad();
            return quad.Transform(transform).GetBoundingRectangle();
        }

        public override void Accept(IRenderNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        #region Implementation of IEllipseNode

        public double RadiusX { get; set; }
        public double RadiusY { get; set; }
        public Color Color { get; set; }
        public bool FillInterior { get; set; }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                var detachedEllipseNode = new DetachedEllipseNode();
                CopyData(this, detachedEllipseNode);
                _ellipseRendererComponent.EllipseNode = detachedEllipseNode;
            }
        }

        protected override void CopyData(IRenderNode source, IRenderNode target)
        {
            base.CopyData(source, target);

            var sourceEllipseNode = (IEllipseNode)source;
            var targetEllipseNode = (IEllipseNode)target;

            targetEllipseNode.RadiusX = sourceEllipseNode.RadiusX;
            targetEllipseNode.RadiusY = sourceEllipseNode.RadiusY;
            targetEllipseNode.Color = sourceEllipseNode.Color;
            targetEllipseNode.FillInterior = sourceEllipseNode.FillInterior;
        }
    }
}