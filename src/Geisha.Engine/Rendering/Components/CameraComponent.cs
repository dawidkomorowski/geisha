using System;
using Geisha.Common.Math;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components
{
    // TODO Should Camera be actually a component? Maybe it should be separate thing directly on Scene?
    // TODO what if there are more than one camera? (introduce active flag?)
    // TODO optimization(camera viewing space is good point to optimizing draw calls by clipping things out of camera or not visible by camera)
    // TODO viewing space for 3D is frustum space that defines observable clipping polyhedron
    // TODO projection type (only meaningful for 3D ?)
    /// <summary>
    ///     Represents camera that defines view-port.
    /// </summary>
    [ComponentId("Geisha.Engine.Rendering.CameraComponent")]
    public sealed class CameraComponent : Component
    {
        /// <summary>
        ///     Defines how camera view is fit in the screen when there is an aspect ratio mismatch.
        /// </summary>
        public AspectRatioBehavior AspectRatioBehavior { get; set; } = AspectRatioBehavior.Overscan;

        /// <summary>
        ///     Width of the screen (full screen) or client area in the window (excluding window frame) in pixels.
        /// </summary>
        public int ScreenWidth { get; internal set; }

        /// <summary>
        ///     Height of the screen (full screen) or client area in the window (excluding window frame) in pixels.
        /// </summary>
        public int ScreenHeight { get; internal set; }

        /// <summary>
        ///     Dimensions of rectangle that defines fragment of space visible for camera using logical units that are independent
        ///     of window size or screen resolution.
        /// </summary>
        public Vector2 ViewRectangle { get; set; }

        protected internal override void Serialize(IComponentDataWriter componentDataWriter, IAssetStore assetStore)
        {
            base.Serialize(componentDataWriter, assetStore);
            componentDataWriter.WriteEnum("AspectRatioBehavior", AspectRatioBehavior);
            componentDataWriter.WriteVector2("ViewRectangle", ViewRectangle);
        }

        protected internal override void Deserialize(IComponentDataReader componentDataReader, IAssetStore assetStore)
        {
            base.Deserialize(componentDataReader, assetStore);
            AspectRatioBehavior = componentDataReader.ReadEnum<AspectRatioBehavior>("AspectRatioBehavior");
            ViewRectangle = componentDataReader.ReadVector2("ViewRectangle");
        }
    }

    /// <summary>
    ///     Defines behaviors of camera view fitting in the screen when there is an aspect ratio mismatch.
    /// </summary>
    public enum AspectRatioBehavior
    {
        /// <summary>
        ///     Whole screen is used to present camera view while keeping aspect ratio. It may result in parts of the view being
        ///     not visible as scaled outside of the screen.
        /// </summary>
        Overscan,

        /// <summary>
        ///     Whole camera view is visible on the screen and it is fit to match either width or height of the screen while
        ///     keeping aspect ratio. It may result in some kind of windowboxed view with black bars filling the missing space.
        /// </summary>
        Underscan
    }

    // TODO Should it be part of CameraComponent?
    /// <summary>
    ///     Provides common methods for camera that is for entity with camera component attached.
    /// </summary>
    public static class CameraExtensions
    {
        // TODO There are no tests of this method.
        /// <summary>
        ///     Transforms point in screen space to point in 2D world space as seen by camera.
        /// </summary>
        /// <param name="cameraEntity">Entity with camera component attached.</param>
        /// <param name="screenPoint">Point in screen space.</param>
        /// <returns>Point in 2D world space corresponding to given point in screen space as seen by camera.</returns>
        public static Vector2 ScreenPointTo2DWorldPoint(this Entity cameraEntity, Vector2 screenPoint)
        {
            if (!cameraEntity.HasComponent<CameraComponent>()) throw new ArgumentException("Entity is not a camera.");

            var cameraComponent = cameraEntity.GetComponent<CameraComponent>();
            var cameraTransform = cameraEntity.GetComponent<Transform2DComponent>();

            var viewRectangleScale = GetViewRectangleScale(cameraEntity);
            var transformationMatrix = cameraTransform.ToMatrix() * Matrix3x3.CreateScale(new Vector2(viewRectangleScale.X, -viewRectangleScale.Y)) *
                                       Matrix3x3.CreateTranslation(new Vector2(-cameraComponent.ScreenWidth / 2.0, -cameraComponent.ScreenHeight / 2.0));

            return (transformationMatrix * screenPoint.Homogeneous).ToVector2();
        }

        /// <summary>
        ///     Creates view matrix that converts coordinates from 2D space to the screen space as seen by camera.
        /// </summary>
        /// <param name="cameraEntity">Entity with camera component attached.</param>
        /// <returns>View matrix that converts coordinates from 2D space to the screen space as seen by camera.</returns>
        public static Matrix3x3 Create2DWorldToScreenMatrix(this Entity cameraEntity)
        {
            if (!cameraEntity.HasComponent<CameraComponent>()) throw new ArgumentException("Entity is not a camera.");

            var cameraTransform = cameraEntity.GetComponent<Transform2DComponent>();

            var cameraScale = cameraTransform.Scale;
            var viewRectangleScale = GetViewRectangleScale(cameraEntity);
            var finalCameraScale = new Vector2(cameraScale.X * viewRectangleScale.X, cameraScale.Y * viewRectangleScale.Y);

            return Matrix3x3.CreateScale(new Vector2(1 / finalCameraScale.X, 1 / finalCameraScale.Y)) *
                   Matrix3x3.CreateRotation(-cameraTransform.Rotation) *
                   Matrix3x3.CreateTranslation(-cameraTransform.Translation) * Matrix3x3.Identity;
        }

        internal static bool CameraIsWiderThanScreen(this CameraComponent cameraComponent)
        {
            var cameraAspectRatio = cameraComponent.ViewRectangle.X / cameraComponent.ViewRectangle.Y;
            var screenAspectRatio = (double) cameraComponent.ScreenWidth / cameraComponent.ScreenHeight;

            return cameraAspectRatio > screenAspectRatio;
        }

        private static Vector2 GetViewRectangleScale(Entity cameraEntity)
        {
            var cameraComponent = cameraEntity.GetComponent<CameraComponent>();

            var viewRectangleScale = cameraComponent.AspectRatioBehavior switch
            {
                AspectRatioBehavior.Overscan => ComputeOverscan(cameraComponent),
                AspectRatioBehavior.Underscan => ComputeUnderscan(cameraComponent),
                _ => throw new ArgumentOutOfRangeException()
            };

            // TODO This is workaround for scenarios when ScreenWidth and ScreenHeight is not yet set on CameraComponent and therefore it is zero.
            if (!double.IsFinite(viewRectangleScale.X) || !double.IsFinite(viewRectangleScale.Y)) viewRectangleScale = Vector2.One;

            return viewRectangleScale;
        }

        private static Vector2 ComputeOverscan(CameraComponent cameraComponent)
        {
            if (cameraComponent.CameraIsWiderThanScreen())
            {
                var scaleForHeight = cameraComponent.ViewRectangle.Y / cameraComponent.ScreenHeight;
                return new Vector2(scaleForHeight, scaleForHeight);
            }
            else
            {
                var scaleForWidth = cameraComponent.ViewRectangle.X / cameraComponent.ScreenWidth;
                return new Vector2(scaleForWidth, scaleForWidth);
            }
        }

        private static Vector2 ComputeUnderscan(CameraComponent cameraComponent)
        {
            if (cameraComponent.CameraIsWiderThanScreen())
            {
                var scaleForWidth = cameraComponent.ViewRectangle.X / cameraComponent.ScreenWidth;
                return new Vector2(scaleForWidth, scaleForWidth);
            }
            else
            {
                var scaleForHeight = cameraComponent.ViewRectangle.Y / cameraComponent.ScreenHeight;
                return new Vector2(scaleForHeight, scaleForHeight);
            }
        }
    }

    internal sealed class CameraComponentFactory : ComponentFactory<CameraComponent>
    {
        protected override CameraComponent CreateComponent() => new CameraComponent();
    }
}