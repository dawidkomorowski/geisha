using System;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems;

internal interface IRenderNode
{
    bool IsManagedByRenderingSystem { get; }
    bool Visible { get; set; }
    string SortingLayerName { get; set; }
    int OrderInLayer { get; set; }
    bool IsStatic { get; set; }
    AxisAlignedRectangle GetBoundingRectangle();
}

internal abstract class DetachedRenderNode : IRenderNode
{
    public bool IsManagedByRenderingSystem => false;
    public bool Visible { get; set; }
    public string SortingLayerName { get; set; } = string.Empty;
    public int OrderInLayer { get; set; }
    public bool IsStatic { get; set; }
    public AxisAlignedRectangle GetBoundingRectangle() => default;
}

internal abstract class RenderNode : IRenderNode, IDisposable
{
    private string _sortingLayerName = string.Empty;

    private bool _isStatic;
    private Matrix3x3 _staticWorldTransform;
    private AxisAlignedRectangle _staticBoundingRectangle;

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

    public bool IsStatic
    {
        get => _isStatic;
        set
        {
            _isStatic = value;

            if (_isStatic)
            {
                _staticWorldTransform = Transform.ComputeInterpolatedWorldTransformMatrix();
                _staticBoundingRectangle = ComputeBoundingRectangle();
            }
        }
    }

    public AxisAlignedRectangle GetBoundingRectangle() => IsStatic ? _staticBoundingRectangle : ComputeBoundingRectangle();

    #endregion

    public abstract void Accept(IRenderNodeVisitor visitor);
    public virtual bool ShouldSkipRendering() => !Renderer2DComponent.Visible;
    public Matrix3x3 GetWorldTransform() => IsStatic ? _staticWorldTransform : Transform.ComputeInterpolatedWorldTransformMatrix();

    protected abstract AxisAlignedRectangle ComputeBoundingRectangle();

    public void Dispose()
    {
        Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
        SortingLayerNameChangedCallback = null;
    }

    // TODO: Include IsStatic.
    protected virtual void CopyData(IRenderNode source, IRenderNode target)
    {
        target.Visible = source.Visible;
        target.SortingLayerName = source.SortingLayerName;
        target.OrderInLayer = source.OrderInLayer;
    }
}