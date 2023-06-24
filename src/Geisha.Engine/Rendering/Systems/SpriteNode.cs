using Geisha.Engine.Core.Components;
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

        public override void Accept(IRenderNodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}