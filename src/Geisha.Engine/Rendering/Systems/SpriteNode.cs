﻿using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal sealed class SpriteNode : RenderNode
    {
        public SpriteNode(Transform2DComponent transform, SpriteRendererComponent spriteRendererComponent) : base(transform, spriteRendererComponent)
        {
            SpriteRendererComponent = spriteRendererComponent;
        }

        public SpriteRendererComponent SpriteRendererComponent { get; }

        public override AxisAlignedRectangle GetBoundingRectangle()
        {
            // TODO If sprite is null then SpriteNode should not be processed at all.
            // TODO Provide ShouldRender() method for all nodes to early filter nodes to render?
            if (SpriteRendererComponent.Sprite == null)
            {
                return new AxisAlignedRectangle();
            }

            var transform = TransformHierarchy.Calculate2DTransformationMatrix(Entity);
            var quad = SpriteRendererComponent.Sprite.Rectangle.ToQuad();
            return quad.Transform(transform).GetBoundingRectangle();
        }

        public override void Accept(IRenderNodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}