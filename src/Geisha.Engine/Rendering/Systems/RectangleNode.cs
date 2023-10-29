using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal interface IRectangleNode : IRenderNode
    {
        Vector2 Dimensions { get; set; }
        Color Color { get; set; }
        bool FillInterior { get; set; }
    }

    internal sealed class DetachedRectangleNode : DetachedRenderNode, IRectangleNode
    {
        public Vector2 Dimensions { get; set; }
        public Color Color { get; set; }
        public bool FillInterior { get; set; }
    }

    internal sealed class RectangleNode : RenderNode, IRectangleNode
    {
        private readonly RectangleRendererComponent _rectangleRendererComponent;

        public RectangleNode(Transform2DComponent transform, RectangleRendererComponent rectangleRendererComponent)
            : base(transform, rectangleRendererComponent)
        {
            _rectangleRendererComponent = rectangleRendererComponent;
            CopyData(_rectangleRendererComponent.RectangleNode, this);
            _rectangleRendererComponent.RectangleNode = this;
        }

        public override AxisAlignedRectangle GetBoundingRectangle()
        {
            var transform = TransformHierarchy.Calculate2DTransformationMatrix(Entity);
            var quad = new AxisAlignedRectangle(_rectangleRendererComponent.Dimensions).ToQuad();
            return quad.Transform(transform).GetBoundingRectangle();
        }

        public override void Accept(IRenderNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        #region Implementation of IRectangleNode

        public Vector2 Dimensions { get; set; }
        public Color Color { get; set; }
        public bool FillInterior { get; set; }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                var detachedRectangleNode = new DetachedRectangleNode();
                CopyData(this, detachedRectangleNode);
                _rectangleRendererComponent.RectangleNode = detachedRectangleNode;
            }
        }

        protected override void CopyData(IRenderNode source, IRenderNode target)
        {
            base.CopyData(source, target);

            var sourceRectangleNode = (IRectangleNode)source;
            var targetRectangleNode = (IRectangleNode)target;

            targetRectangleNode.Dimensions = sourceRectangleNode.Dimensions;
            targetRectangleNode.Color = sourceRectangleNode.Color;
            targetRectangleNode.FillInterior = sourceRectangleNode.FillInterior;
        }
    }
}