using System;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Engine.Rendering.Systems
{
    internal interface ICameraNode
    {
        bool IsManagedByRenderingSystem { get; }
        AspectRatioBehavior AspectRatioBehavior { get; set; }
        int ScreenWidth { get; }
        int ScreenHeight { get; }
        Vector2 ViewRectangle { get; set; }
        Vector2 ScreenPointToWorld2DPoint(Vector2 screenPoint);
        Vector2 World2DPointToScreenPoint(Vector2 worldPoint);
        Matrix3x3 CreateViewMatrix();
        Matrix3x3 CreateViewMatrixScaledToScreen();
        AxisAlignedRectangle GetBoundingRectangleOfView();
    }

    internal sealed class DetachedCameraNode : ICameraNode
    {
        public bool IsManagedByRenderingSystem => false;
        public AspectRatioBehavior AspectRatioBehavior { get; set; }
        public int ScreenWidth => 0;
        public int ScreenHeight => 0;
        public Vector2 ViewRectangle { get; set; }
        public Vector2 ScreenPointToWorld2DPoint(Vector2 screenPoint) => default;
        public Vector2 World2DPointToScreenPoint(Vector2 worldPoint) => default;
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
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public Vector2 ViewRectangle { get; set; }

        public Vector2 ScreenPointToWorld2DPoint(Vector2 screenPoint)
        {
            var viewRectangleScale = GetViewRectangleScale();
            var transformationMatrix = _transform.ToMatrix() *
                                       Matrix3x3.CreateScale(new Vector2(viewRectangleScale.X, -viewRectangleScale.Y)) *
                                       Matrix3x3.CreateTranslation(new Vector2(-ScreenWidth / 2.0, -ScreenHeight / 2.0));

            return (transformationMatrix * screenPoint.Homogeneous).ToVector2();
        }

        public Vector2 World2DPointToScreenPoint(Vector2 worldPoint)
        {
            var transformationMatrix = Matrix3x3.CreateTranslation(new Vector2(ScreenWidth / 2.0, ScreenHeight / 2.0)) *
                                       Matrix3x3.CreateScale(new Vector2(1, -1)) *
                                       CreateViewMatrixScaledToScreen();

            return (transformationMatrix * worldPoint.Homogeneous).ToVector2();
        }

        public Matrix3x3 CreateViewMatrix()
        {
            var cameraScale = _transform.Scale;

            return Matrix3x3.CreateScale(new Vector2(1 / cameraScale.X, 1 / cameraScale.Y)) *
                   Matrix3x3.CreateRotation(-_transform.Rotation) *
                   Matrix3x3.CreateTranslation(-_transform.Translation) * Matrix3x3.Identity;
        }

        public Matrix3x3 CreateViewMatrixScaledToScreen()
        {
            var viewRectangleScale = GetViewRectangleScale();
            return Matrix3x3.CreateScale(new Vector2(1 / viewRectangleScale.X, 1 / viewRectangleScale.Y)) * CreateViewMatrix();
        }

        public AxisAlignedRectangle GetBoundingRectangleOfView()
        {
            // TODO : Review other transform dependent methods in this class to see if they should use interpolated transform.
            var transform = _transform.ComputeWorldTransformMatrix();
            var quad = new AxisAlignedRectangle(_camera.ViewRectangle).ToQuad();
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
            if (CameraIsWiderThanScreen())
            {
                var scaleFactor = ScreenWidth / ViewRectangle.X;
                return new AxisAlignedRectangle(new Vector2(ScreenWidth, ViewRectangle.Y * scaleFactor));
            }
            else
            {
                var scaleFactor = ScreenHeight / ViewRectangle.Y;
                return new AxisAlignedRectangle(new Vector2(ViewRectangle.X * scaleFactor, ScreenHeight));
            }
        }

        private Vector2 GetViewRectangleScale()
        {
            var viewRectangleScale = AspectRatioBehavior switch
            {
                AspectRatioBehavior.Overscan => ComputeOverscan(),
                AspectRatioBehavior.Underscan => ComputeUnderscan(),
                _ => throw new ArgumentOutOfRangeException()
            };

            return viewRectangleScale;
        }

        private Vector2 ComputeOverscan()
        {
            if (CameraIsWiderThanScreen())
            {
                var scaleForHeight = ViewRectangle.Y / ScreenHeight;
                return new Vector2(scaleForHeight, scaleForHeight);
            }

            var scaleForWidth = ViewRectangle.X / ScreenWidth;
            return new Vector2(scaleForWidth, scaleForWidth);
        }

        private Vector2 ComputeUnderscan()
        {
            if (CameraIsWiderThanScreen())
            {
                var scaleForWidth = ViewRectangle.X / ScreenWidth;
                return new Vector2(scaleForWidth, scaleForWidth);
            }

            var scaleForHeight = ViewRectangle.Y / ScreenHeight;
            return new Vector2(scaleForHeight, scaleForHeight);
        }

        private bool CameraIsWiderThanScreen()
        {
            var cameraAspectRatio = ViewRectangle.X / ViewRectangle.Y;
            var screenAspectRatio = (double)ScreenWidth / ScreenHeight;

            return cameraAspectRatio > screenAspectRatio;
        }

        private static void CopyData(ICameraNode source, ICameraNode target)
        {
            target.AspectRatioBehavior = source.AspectRatioBehavior;
            target.ViewRectangle = source.ViewRectangle;
        }
    }
}