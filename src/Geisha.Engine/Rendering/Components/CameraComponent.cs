using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Rendering.Systems;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     Represents camera that controls what is visible in viewport.
    /// </summary>
    [ComponentId("Geisha.Engine.Rendering.CameraComponent")]
    public sealed class CameraComponent : Component
    {
        internal CameraComponent(Entity entity) : base(entity)
        {
            CameraNode = new DetachedCameraNode
            {
                AspectRatioBehavior = AspectRatioBehavior.Overscan,
                ViewRectangle = default
            };
        }

        internal ICameraNode CameraNode { get; set; }

        /// <summary>
        ///     Indicates whether this <see cref="CameraComponent" /> is managed by rendering system.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         <see cref="CameraComponent" /> is managed by rendering system when it belongs to <see cref="Scene" /> that is
        ///         managed by rendering system. It is true for components that are part of currently processed scene at runtime
        ///         but it may not be true during serialization or in context of some tools.
        ///     </para>
        ///     <para>
        ///         <see cref="CameraComponent" /> has limited functionality when it is not managed by rendering system. For
        ///         example some APIs may return default values instead of being actually computed.
        ///     </para>
        /// </remarks>
        public bool IsManagedByRenderingSystem => CameraNode.IsManagedByRenderingSystem;

        /// <summary>
        ///     Defines how camera view is fit in the screen when there is an aspect ratio mismatch. Default is
        ///     <see cref="AspectRatioBehavior.Overscan" />.
        /// </summary>
        public AspectRatioBehavior AspectRatioBehavior
        {
            get => CameraNode.AspectRatioBehavior;
            set => CameraNode.AspectRatioBehavior = value;
        }

        /// <summary>
        ///     Width of the screen (full screen) or client area in the window (excluding window frame) in pixels.
        /// </summary>
        public int ScreenWidth => CameraNode.ScreenWidth;

        /// <summary>
        ///     Height of the screen (full screen) or client area in the window (excluding window frame) in pixels.
        /// </summary>
        public int ScreenHeight => CameraNode.ScreenHeight;

        /// <summary>
        ///     Dimensions of rectangle that defines fragment of space visible for camera using logical units that are independent
        ///     of window size or screen resolution.
        /// </summary>
        public Vector2 ViewRectangle
        {
            get => CameraNode.ViewRectangle;
            set => CameraNode.ViewRectangle = value;
        }

        /// <summary>
        ///     Transforms point in screen space to point in 2D world space as seen by camera.
        /// </summary>
        /// <param name="screenPoint">Point in screen space.</param>
        /// <returns>Point in 2D world space corresponding to given point in screen space as seen by camera.</returns>
        public Vector2 ScreenPointToWorld2DPoint(Vector2 screenPoint) => CameraNode.ScreenPointToWorld2DPoint(screenPoint);

        /// <summary>
        ///     Transforms point in 2D world space to point in screen space as seen by camera.
        /// </summary>
        /// <param name="worldPoint">Point in 2D world space.</param>
        /// <returns>Point in screen space corresponding to given point in 2D world space as seen by camera.</returns>
        public Vector2 World2DPointToScreenPoint(Vector2 worldPoint) => CameraNode.World2DPointToScreenPoint(worldPoint);

        // TODO There are no tests of this method.
        /// <summary>
        ///     Creates view matrix that converts coordinates from 2D world space to the view space that is space relative to the
        ///     view of camera.
        /// </summary>
        /// <returns>
        ///     View matrix that converts coordinates from 2D world space to the view space that is space relative to the view
        ///     of camera.
        /// </returns>
        public Matrix3x3 CreateViewMatrix() => CameraNode.CreateViewMatrix();

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