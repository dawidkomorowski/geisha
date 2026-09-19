using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Rendering.Systems;

namespace Geisha.Engine.Rendering.Components;

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
    ///         managed by rendering system. It is true for components that are part of currently processed scene at runtime,
    ///         but it may not be true during serialization or in context of some tools.
    ///     </para>
    ///     <para>
    ///         <see cref="CameraComponent" /> has limited functionality when it is not managed by rendering system. For
    ///         example some APIs may return default values instead of being actually computed.
    ///     </para>
    /// </remarks>
    public bool IsManagedByRenderingSystem => CameraNode.IsManagedByRenderingSystem;

    /// <summary>
    ///     Defines how camera view is fit in the viewport when there is an aspect ratio mismatch. Default is
    ///     <see cref="AspectRatioBehavior.Overscan" />.
    /// </summary>
    public AspectRatioBehavior AspectRatioBehavior
    {
        get => CameraNode.AspectRatioBehavior;
        set => CameraNode.AspectRatioBehavior = value;
    }

    /// <summary>
    ///     Gets the size of the camera viewport in pixels.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The viewport is the pixel area to which this camera renders.
    ///     </para>
    ///     <para>
    ///         This property returns <see cref="Size.Empty" /> when <see cref="CameraComponent" /> is not managed by the
    ///         rendering system.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IsManagedByRenderingSystem" />
    public Size ViewportSize => CameraNode.ViewportSize;

    /// <summary>
    ///     Dimensions of rectangle that defines fragment of space visible for camera using logical units that are independent
    ///     of window size or screen resolution.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         When <see cref="ViewRectangle" /> is set to (0, 0) or any non-positive value, and the
    ///         <see cref="CameraComponent" /> is managed by the rendering system, the engine automatically uses
    ///         <see cref="ViewportSize" /> as the effective view rectangle for all camera computations. This allows the camera
    ///         to adapt to the current screen resolution without requiring explicit configuration.
    ///     </para>
    ///     <para>
    ///         The stored value of <see cref="ViewRectangle" /> is never mutated by the rendering system. If left at the
    ///         default (0, 0), it remains (0, 0) even during rendering and serialization, ensuring that scenes saved with
    ///         default settings will adapt to any screen resolution when loaded.
    ///     </para>
    ///     <para>
    ///         Setting an explicit non-zero <see cref="ViewRectangle" /> enables logical scaling independent of screen
    ///         resolution, which is useful for achieving consistent game world dimensions across different display sizes.
    ///     </para>
    /// </remarks>
    public Vector2 ViewRectangle
    {
        get => CameraNode.ViewRectangle;
        set => CameraNode.ViewRectangle = value;
    }

    /// <summary>
    ///     Gets axis aligned bounding rectangle of camera <see cref="ViewRectangle" /> in global coordinates.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="BoundingRectangleOfView" /> is axis aligned rectangle that fully encloses
    ///         <see cref="ViewRectangle" /> of this <see cref="CameraComponent" />.
    ///     </para>
    ///     <para>
    ///         This property returns default value of <see cref="AxisAlignedRectangle" /> when
    ///         <see cref="CameraComponent" /> is not managed by rendering system.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IsManagedByRenderingSystem" />
    public AxisAlignedRectangle BoundingRectangleOfView => CameraNode.GetBoundingRectangleOfView();

    /// <summary>
    ///     Transforms a point from camera viewport pixel coordinates to 2D world coordinates as seen by camera.
    /// </summary>
    /// <param name="viewportPoint">Point in camera viewport pixel coordinates, with the origin in the top-left corner.</param>
    /// <returns>
    ///     Point in 2D world coordinates corresponding to given point in camera viewport pixel coordinates as seen by
    ///     camera.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This method returns default value of <see cref="Vector2" /> when <see cref="CameraComponent" /> is not managed
    ///         by rendering system.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IsManagedByRenderingSystem" />
    public Vector2 ViewportPointToWorld2DPoint(in Vector2 viewportPoint) => CameraNode.ViewportPointToWorld2DPoint(viewportPoint);

    /// <summary>
    ///     Transforms a point from 2D world coordinates to camera viewport pixel coordinates as seen by camera.
    /// </summary>
    /// <param name="worldPoint">Point in 2D world coordinates.</param>
    /// <returns>
    ///     Point in camera viewport pixel coordinates corresponding to given point in 2D world coordinates as seen by
    ///     camera.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This method returns default value of <see cref="Vector2" /> when <see cref="CameraComponent" /> is not managed
    ///         by rendering system.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IsManagedByRenderingSystem" />
    public Vector2 World2DPointToViewportPoint(in Vector2 worldPoint) => CameraNode.World2DPointToViewportPoint(worldPoint);

    /// <summary>
    ///     Creates view matrix that converts coordinates from 2D world space to the view space that is space relative to the
    ///     view of camera.
    /// </summary>
    /// <returns>
    ///     View matrix that converts coordinates from 2D world space to the view space that is space relative to the view
    ///     of camera.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This method returns default value of <see cref="Matrix3x3" /> when <see cref="CameraComponent" /> is not
    ///         managed by rendering system.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IsManagedByRenderingSystem" />
    public Matrix3x3 CreateViewMatrix() => CameraNode.CreateViewMatrix();

    /// <summary>
    ///     Creates view matrix that includes scaling <see cref="ViewRectangle" /> to match viewport size.
    /// </summary>
    /// <returns>View matrix that is scaled to match viewport size.</returns>
    /// <remarks>
    ///     <para>
    ///         This method returns default value of <see cref="Matrix3x3" /> when <see cref="CameraComponent" /> is not
    ///         managed by rendering system.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IsManagedByRenderingSystem" />
    public Matrix3x3 CreateViewMatrixScaledToViewport() => CameraNode.CreateViewMatrixScaledToViewport();

    /// <inheritdoc />
    protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
    {
        base.Serialize(writer, assetStore);
        writer.WriteEnum("AspectRatioBehavior", AspectRatioBehavior);
        writer.WriteVector2("ViewRectangle", ViewRectangle);
    }

    /// <inheritdoc />
    protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
    {
        base.Deserialize(reader, assetStore);
        AspectRatioBehavior = reader.ReadEnum<AspectRatioBehavior>("AspectRatioBehavior");
        ViewRectangle = reader.ReadVector2("ViewRectangle");
    }
}

/// <summary>
///     Defines behaviors of camera view fitting in the viewport when there is an aspect ratio mismatch.
/// </summary>
public enum AspectRatioBehavior
{
    /// <summary>
    ///     Whole viewport is used to present camera view while keeping aspect ratio. It may result in parts of the view being
    ///     not visible as scaled outside the viewport. It is default <see cref="AspectRatioBehavior" />.
    /// </summary>
    Overscan,

    /// <summary>
    ///     Whole camera view is visible in the viewport, and it is fit to match either width or height of the viewport while
    ///     keeping aspect ratio. It may result in some kind of window-boxed view with black bars filling the missing space.
    /// </summary>
    Underscan
}

internal sealed class CameraComponentFactory : ComponentFactory<CameraComponent>
{
    protected override CameraComponent CreateComponent(Entity entity) => new(entity);
}