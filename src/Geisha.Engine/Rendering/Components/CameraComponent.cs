using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components
{
    // TODO Add API IsManagedByRenderingSystem? (as for other rendering related components?)
    // TODO Should Camera be actually a component? Maybe it should be separate thing directly on Scene?
    // TODO what if there are more than one camera? (introduce active flag?)
    // TODO viewing space for 3D is frustum space that defines observable clipping polyhedron
    // TODO projection type (only meaningful for 3D ?)
    /// <summary>
    ///     Represents camera that controls what is visible in viewport.
    /// </summary>
    [ComponentId("Geisha.Engine.Rendering.CameraComponent")]
    public sealed class CameraComponent : Component
    {
        internal CameraComponent(Entity entity) : base(entity)
        {
        }

        /// <summary>
        ///     Defines how camera view is fit in the screen when there is an aspect ratio mismatch. Default is
        ///     <see cref="AspectRatioBehavior.Overscan" />.
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

        // TODO There are no tests of this method.
        /// <summary>
        ///     Transforms point in screen space to point in 2D world space as seen by camera.
        /// </summary>
        /// <param name="screenPoint">Point in screen space.</param>
        /// <returns>Point in 2D world space corresponding to given point in screen space as seen by camera.</returns>
        public Vector2 ScreenPointTo2DWorldPoint(Vector2 screenPoint)
        {
            var cameraTransform = Entity.GetComponent<Transform2DComponent>();

            var viewRectangleScale = GetViewRectangleScale();
            var transformationMatrix = cameraTransform.ToMatrix() * Matrix3x3.CreateScale(new Vector2(viewRectangleScale.X, -viewRectangleScale.Y)) *
                                       Matrix3x3.CreateTranslation(new Vector2(-ScreenWidth / 2.0, -ScreenHeight / 2.0));

            return (transformationMatrix * screenPoint.Homogeneous).ToVector2();
        }

        // TODO There are no tests of this method.
        /// <summary>
        ///     Creates view matrix that converts coordinates from 2D space to the screen space as seen by camera.
        /// </summary>
        /// <returns>View matrix that converts coordinates from 2D space to the screen space as seen by camera.</returns>
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

        protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
        {
            base.Serialize(writer, assetStore);
            writer.WriteEnum("AspectRatioBehavior", AspectRatioBehavior);
            writer.WriteVector2("ViewRectangle", ViewRectangle);
        }

        protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
        {
            base.Deserialize(reader, assetStore);
            AspectRatioBehavior = reader.ReadEnum<AspectRatioBehavior>("AspectRatioBehavior");
            ViewRectangle = reader.ReadVector2("ViewRectangle");
        }

        internal bool CameraIsWiderThanScreen()
        {
            var cameraAspectRatio = ViewRectangle.X / ViewRectangle.Y;
            var screenAspectRatio = (double)ScreenWidth / ScreenHeight;

            return cameraAspectRatio > screenAspectRatio;
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
    }

    /// <summary>
    ///     Defines behaviors of camera view fitting in the screen when there is an aspect ratio mismatch.
    /// </summary>
    public enum AspectRatioBehavior
    {
        /// <summary>
        ///     Whole screen is used to present camera view while keeping aspect ratio. It may result in parts of the view being
        ///     not visible as scaled outside of the screen. It is default <see cref="AspectRatioBehavior" />.
        /// </summary>
        Overscan,

        /// <summary>
        ///     Whole camera view is visible on the screen and it is fit to match either width or height of the screen while
        ///     keeping aspect ratio. It may result in some kind of windowboxed view with black bars filling the missing space.
        /// </summary>
        Underscan
    }

    internal sealed class CameraComponentFactory : ComponentFactory<CameraComponent>
    {
        protected override CameraComponent CreateComponent(Entity entity) => new(entity);
    }
}