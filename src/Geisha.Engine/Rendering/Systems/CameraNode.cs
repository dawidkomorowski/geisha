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
        Matrix3x3 Create2DWorldToScreenMatrix();
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
        public Matrix3x3 Create2DWorldToScreenMatrix() => default;
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
            var cameraTransform = Entity.GetComponent<Transform2DComponent>();

            var viewRectangleScale = GetViewRectangleScale();
            var transformationMatrix = cameraTransform.ToMatrix() * Matrix3x3.CreateScale(new Vector2(viewRectangleScale.X, -viewRectangleScale.Y)) *
                                       Matrix3x3.CreateTranslation(new Vector2(-ScreenWidth / 2.0, -ScreenHeight / 2.0));

            return (transformationMatrix * screenPoint.Homogeneous).ToVector2();
        }

        public Matrix3x3 Create2DWorldToScreenMatrix()
        {
            var cameraTransform = Entity.GetComponent<Transform2DComponent>();

            var cameraScale = cameraTransform.Scale;
            var viewRectangleScale = GetViewRectangleScale();
            var finalCameraScale = new Vector2(cameraScale.X * viewRectangleScale.X, cameraScale.Y * viewRectangleScale.Y);

            return Matrix3x3.CreateScale(new Vector2(1 / finalCameraScale.X, 1 / finalCameraScale.Y)) *
                   Matrix3x3.CreateRotation(-cameraTransform.Rotation) *
                   Matrix3x3.CreateTranslation(-cameraTransform.Translation) * Matrix3x3.Identity;
        }

        public AxisAlignedRectangle GetBoundingRectangleOfView()
        {
            var transform = TransformHierarchy.Calculate2DTransformationMatrix(Entity);
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

            // TODO This is workaround for scenarios when ScreenWidth and ScreenHeight is not yet set on CameraComponent and therefore it is zero.
            if (!double.IsFinite(viewRectangleScale.X) || !double.IsFinite(viewRectangleScale.Y)) viewRectangleScale = Vector2.One;

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