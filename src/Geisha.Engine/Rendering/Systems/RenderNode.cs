using System;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal abstract class RenderNode : IDisposable
    {
        protected RenderNode(Transform2DComponent transform, Renderer2DComponent renderer2DComponent)
        {
            Transform = transform;
            Renderer2DComponent = renderer2DComponent;
        }

        public Entity Entity => Transform.Entity;
        public Transform2DComponent Transform { get; }
        public Renderer2DComponent Renderer2DComponent { get; }

        public abstract AxisAlignedRectangle GetBoundingRectangle();

        public abstract void Accept(IRenderNodeVisitor visitor);

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}