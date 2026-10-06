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
        private string _sortingLayerName = string.Empty;

        // TODO: This needs to be exposed to user code.
        private bool _allowCaching = true;
        private bool _isCached;
        private AxisAlignedRectangle _cachedBoundingRectangle;
        private Matrix3x3 _cachedMatrix;

        protected RenderNode(Transform2DComponent transform, Renderer2DComponent renderer2DComponent)
        {
            Transform = transform;
            Renderer2DComponent = renderer2DComponent;
        }

        public delegate void SortingLayerNameChangedCallbackDelegate(RenderNode renderNode, string newLayerName, string oldLayerName);

        public Entity Entity => Transform.Entity;
        public Transform2DComponent Transform { get; }
        public Renderer2DComponent Renderer2DComponent { get; }
        public virtual BatchId BatchId => BatchId.Empty;
        public SortingLayerNameChangedCallbackDelegate? SortingLayerNameChangedCallback { get; set; }

        #region Implementation of IRenderNode

        public bool IsManagedByRenderingSystem => true;
        public bool Visible { get; set; }

        public string SortingLayerName
        {
            get => _sortingLayerName;
            set
            {
                SortingLayerNameChangedCallback?.Invoke(this, value, _sortingLayerName);
                _sortingLayerName = value;
            }
        }

        public int OrderInLayer { get; set; }
        public abstract AxisAlignedRectangle GetBoundingRectangle();

        #endregion

        public abstract void Accept(IRenderNodeVisitor visitor);
        public virtual bool ShouldSkipRendering() => !Renderer2DComponent.Visible;

        // TODO: GetCached* is not the best name. Figure out some alternatives.
        public AxisAlignedRectangle GetCachedBoundingRectangle()
        {
            if (!_allowCaching)
            {
                return GetBoundingRectangle();
            }

            if (_isCached)
            {
                return _cachedBoundingRectangle;
            }

            _cachedMatrix = Transform.ComputeInterpolatedWorldTransformMatrix();
            _cachedBoundingRectangle = GetBoundingRectangle();
            _isCached = true;

            return _cachedBoundingRectangle;
        }

        public Matrix3x3 GetCachedWorldTransform()
        {
            if (!_allowCaching)
            {
                return Transform.ComputeInterpolatedWorldTransformMatrix();
            }

            if (_isCached)
            {
                return _cachedMatrix;
            }

            _cachedMatrix = Transform.ComputeInterpolatedWorldTransformMatrix();
            _cachedBoundingRectangle = GetBoundingRectangle();
            _isCached = true;

            return _cachedMatrix;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            SortingLayerNameChangedCallback = null;
        }

        protected virtual void CopyData(IRenderNode source, IRenderNode target)
        {
            target.Visible = source.Visible;
            target.SortingLayerName = source.SortingLayerName;
            target.OrderInLayer = source.OrderInLayer;
        }
    }
}