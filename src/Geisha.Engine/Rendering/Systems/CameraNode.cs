using System;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems;

internal interface ICameraNode
{
    bool IsManagedByRenderingSystem { get; }
    AspectRatioBehavior AspectRatioBehavior { get; set; }
    Size ScreenSize { get; }
    Vector2 ViewRectangle { get; set; }
    Vector2 ScreenPointToWorld2DPoint(in Vector2 screenPoint);
    Vector2 World2DPointToScreenPoint(in Vector2 worldPoint);
    Matrix3x3 CreateViewMatrix();
    Matrix3x3 CreateViewMatrixScaledToScreen();
    AxisAlignedRectangle GetBoundingRectangleOfView();
}

internal sealed class DetachedCameraNode : ICameraNode
{
    public bool IsManagedByRenderingSystem => false;
    public AspectRatioBehavior AspectRatioBehavior { get; set; }
    public Size ScreenSize => Size.Empty;
    public Vector2 ViewRectangle { get; set; }
    public Vector2 ScreenPointToWorld2DPoint(in Vector2 screenPoint) => default;
    public Vector2 World2DPointToScreenPoint(in Vector2 worldPoint) => default;
    public Matrix3x3 CreateViewMatrix() => default;
    public Matrix3x3 CreateViewMatrixScaledToScreen() => default;

    public AxisAlignedRectangle GetBoundingRectangleOfView() => default;
}

internal sealed class CameraNode : ICameraNode, IDisposable
{
    private readonly Transform2DComponent _transform;
    private readonly CameraComponent _camera;

    public CameraNode(Transform2DComponent transform, CameraComponent camera)
    {
        _transform = transform;
        _camera = camera;

        CopyData(_camera.CameraNode, this);
        _camera.CameraNode = this;
    }

    public Entity Entity => _transform.Entity;

    #region Implementation of ICameraNode

    public bool IsManagedByRenderingSystem => true;
    public AspectRatioBehavior AspectRatioBehavior { get; set; }
    public Size ScreenSize { get; set; }
    public Vector2 ViewRectangle { get; set; }

    public Vector2 ScreenPointToWorld2DPoint(in Vector2 screenPoint)
    {
        var viewRectangleScale = GetViewRectangleScale();
        var transformationMatrix = _transform.InterpolatedTransform.ToMatrix() *
                                   Matrix3x3.CreateScale(new Vector2(viewRectangleScale.X, -viewRectangleScale.Y)) *
                                   Matrix3x3.CreateTranslation(ScreenSize.ToVector2() / -2d);

        return (transformationMatrix * screenPoint.Homogeneous).ToVector2();
    }

    public Vector2 World2DPointToScreenPoint(in Vector2 worldPoint)
    {
        var transformationMatrix = Matrix3x3.CreateTranslation(ScreenSize.ToVector2() / 2d) *
                                   Matrix3x3.CreateScale(new Vector2(1, -1)) *
                                   CreateViewMatrixScaledToScreen();

        return (transformationMatrix * worldPoint.Homogeneous).ToVector2();
    }

    public Matrix3x3 CreateViewMatrix()
    {
        var transform = _transform.InterpolatedTransform;

        return Matrix3x3.CreateScale(new Vector2(1 / transform.Scale.X, 1 / transform.Scale.Y)) *
               Matrix3x3.CreateRotation(-transform.Rotation) *
               Matrix3x3.CreateTranslation(-transform.Translation) * Matrix3x3.Identity;
    }

    public Matrix3x3 CreateViewMatrixScaledToScreen()
    {
        var viewRectangleScale = GetViewRectangleScale();
        return Matrix3x3.CreateScale(new Vector2(1 / viewRectangleScale.X, 1 / viewRectangleScale.Y)) * CreateViewMatrix();
    }

    public AxisAlignedRectangle GetBoundingRectangleOfView()
    {
        var transform = _transform.ComputeInterpolatedWorldTransformMatrix();
        var effectiveViewRectangle = GetEffectiveViewRectangle();
        var quad = new AxisAlignedRectangle(effectiveViewRectangle).ToQuad();
        return quad.Transform(transform).GetBoundingRectangle();
    }

    #endregion

    public void Dispose()
    {
        var detachedCameraNode = new DetachedCameraNode();
        CopyData(this, detachedCameraNode);
        _camera.CameraNode = detachedCameraNode;
    }

    public AxisAlignedRectangle GetClippingRectangle()
    {
        var effectiveViewRectangle = GetEffectiveViewRectangle();
        if (CameraIsWiderThanScreen(effectiveViewRectangle))
        {
            var scaleFactor = ScreenSize.Width / effectiveViewRectangle.X;
            return new AxisAlignedRectangle(new Vector2(ScreenSize.Width, effectiveViewRectangle.Y * scaleFactor));
        }
        else
        {
            var scaleFactor = ScreenSize.Height / effectiveViewRectangle.Y;
            return new AxisAlignedRectangle(new Vector2(effectiveViewRectangle.X * scaleFactor, ScreenSize.Height));
        }
    }

    private Vector2 GetEffectiveViewRectangle()
    {
        // When ViewRectangle is 0x0 (or any non-positive value), use ScreenSize as the effective view rectangle
        if (ViewRectangle.X <= 0 || ViewRectangle.Y <= 0)
        {
            return ScreenSize.ToVector2();
        }

        return ViewRectangle;
    }

    private Vector2 GetViewRectangleScale()
    {
        var effectiveViewRectangle = GetEffectiveViewRectangle();
        var viewRectangleScale = AspectRatioBehavior switch
        {
            AspectRatioBehavior.Overscan => ComputeOverscan(effectiveViewRectangle),
            AspectRatioBehavior.Underscan => ComputeUnderscan(effectiveViewRectangle),
            _ => throw new ArgumentOutOfRangeException()
        };

        return viewRectangleScale;
    }

    private Vector2 ComputeOverscan(in Vector2 effectiveViewRectangle)
    {
        if (CameraIsWiderThanScreen(effectiveViewRectangle))
        {
            var scaleForHeight = effectiveViewRectangle.Y / ScreenSize.Height;
            return new Vector2(scaleForHeight, scaleForHeight);
        }

        var scaleForWidth = effectiveViewRectangle.X / ScreenSize.Width;
        return new Vector2(scaleForWidth, scaleForWidth);
    }

    private Vector2 ComputeUnderscan(in Vector2 effectiveViewRectangle)
    {
        if (CameraIsWiderThanScreen(effectiveViewRectangle))
        {
            var scaleForWidth = effectiveViewRectangle.X / ScreenSize.Width;
            return new Vector2(scaleForWidth, scaleForWidth);
        }

        var scaleForHeight = effectiveViewRectangle.Y / ScreenSize.Height;
        return new Vector2(scaleForHeight, scaleForHeight);
    }

    private bool CameraIsWiderThanScreen(in Vector2 effectiveViewRectangle)
    {
        var cameraAspectRatio = effectiveViewRectangle.X / effectiveViewRectangle.Y;
        var screenAspectRatio = (double)ScreenSize.Width / ScreenSize.Height;

        return cameraAspectRatio > screenAspectRatio;
    }

    private static void CopyData(ICameraNode source, ICameraNode target)
    {
        target.AspectRatioBehavior = source.AspectRatioBehavior;
        target.ViewRectangle = source.ViewRectangle;
    }
}