using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal interface ISpriteNode : IRenderNode
    {
        Sprite? Sprite { get; set; }
        double Opacity { get; set; }
    }

    internal sealed class DetachedSpriteNode : DetachedRenderNode, ISpriteNode
    {
        public Sprite? Sprite { get; set; }
        public double Opacity { get; set; }
    }

    internal sealed class SpriteNode : RenderNode, ISpriteNode
    {
        private readonly SpriteRendererComponent _spriteRendererComponent;

        public SpriteNode(Transform2DComponent transform, SpriteRendererComponent spriteRendererComponent)
            : base(transform, spriteRendererComponent)
        {
            _spriteRendererComponent = spriteRendererComponent;
            CopyData(_spriteRendererComponent.SpriteNode, this);
            _spriteRendererComponent.SpriteNode = this;
        }

        public override RuntimeId BatchId => Sprite is null ? RuntimeId.Invalid : Sprite.SourceTexture.RuntimeId;

        public override AxisAlignedRectangle GetBoundingRectangle()
        {
            if (_spriteRendererComponent.Sprite == null)
            {
                return new AxisAlignedRectangle();
            }

            var transform = TransformHierarchy.Calculate2DTransformationMatrix(Entity);
            var quad = _spriteRendererComponent.Sprite.Rectangle.ToQuad();
            return quad.Transform(transform).GetBoundingRectangle();
        }

        public override void Accept(IRenderNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override bool ShouldSkipRendering() => base.ShouldSkipRendering() || Sprite is null;

        #region Implementaion of ISpriteNode

        public Sprite? Sprite { get; set; }
        public double Opacity { get; set; }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                var detachedSpriteNode = new DetachedSpriteNode();
                CopyData(this, detachedSpriteNode);
                _spriteRendererComponent.SpriteNode = detachedSpriteNode;
            }
        }

        protected override void CopyData(IRenderNode source, IRenderNode target)
        {
            base.CopyData(source, target);

            var sourceSpriteNode = (ISpriteNode)source;
            var targetSpriteNode = (ISpriteNode)target;

            targetSpriteNode.Sprite = sourceSpriteNode.Sprite;
            targetSpriteNode.Opacity = sourceSpriteNode.Opacity;
        }
    }
}