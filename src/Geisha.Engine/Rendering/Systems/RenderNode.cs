using System;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal interface IRenderNode
    {
        bool IsManagedByRenderingSystem { get; }
        bool Visible { get; set; }
        string SortingLayerName { get; set; }
        int OrderInLayer { get; set; }
        AxisAlignedRectangle GetBoundingRectangle();
    }

    internal abstract class DetachedRenderNode : IRenderNode
    {
        public bool IsManagedByRenderingSystem => false;
        public bool Visible { get; set; }
        public string SortingLayerName { get; set; } = string.Empty;
        public int OrderInLayer { get; set; }
        public AxisAlignedRectangle GetBoundingRectangle() => default;
    }

    internal abstract class RenderNode : IRenderNode, IDisposable
    {
        protected RenderNode(Transform2DComponent transform, Renderer2DComponent renderer2DComponent)
        {
            Transform = transform;
            Renderer2DComponent = renderer2DComponent;
        }

        public Entity Entity => Transform.Entity;
        public Transform2DComponent Transform { get; }
        public Renderer2DComponent Renderer2DComponent { get; }

        #region Implementation of IRenderNode

        public bool IsManagedByRenderingSystem => true;
        public bool Visible { get; set; }
        public string SortingLayerName { get; set; } = string.Empty;
        public int OrderInLayer { get; set; }
        public abstract AxisAlignedRectangle GetBoundingRectangle();

        #endregion

        public abstract void Accept(IRenderNodeVisitor visitor);
        public virtual bool ShouldSkipRendering() => !Renderer2DComponent.Visible;

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        protected virtual void CopyData(IRenderNode source, IRenderNode target)
        {
            target.Visible = source.Visible;
            target.SortingLayerName = source.SortingLayerName;
            target.OrderInLayer = source.OrderInLayer;
        }
    }
}