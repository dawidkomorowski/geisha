using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Physics.Components;

/// <summary>
///     Represents a 2D collider in the shape of a rectangle. Use this component to define rectangular geometry for static
///     or kinematic rigid bodies.
/// </summary>
/// <remarks>
///     <para>
///         To create a rectangular 2D static rigid body, an entity must have both a
///         <see cref="Core.Components.Transform2DComponent" /> and a <see cref="RectangleColliderComponent" />.
///     </para>
///     <para>
///         To create a kinematic rigid body, see <see cref="KinematicRigidBody2DComponent" />.
///     </para>
///     <para>
///         Static rigid bodies do not support scaling: https://github.com/dawidkomorowski/geisha/issues/604. To avoid
///         unexpected behavior, it is recommended to use scale of (1, 1).
///     </para>
/// </remarks>
[ComponentId("Geisha.Engine.Physics.RectangleColliderComponent")]
public sealed class RectangleColliderComponent : Collider2DComponent
{
    internal RectangleColliderComponent(Entity entity) : base(entity)
    {
    }

    /// <summary>
    ///     Gets or sets the dimensions of the rectangle, defined as width and height in meters. The rectangle is centered at
    ///     the point (0,0) in the local coordinate system.
    /// </summary>
    /// <remarks>
    ///     The dimensions define the size of the rectangle collider in meters.
    ///     It determines the area used for collision detection and physics interactions.
    ///     A larger dimensions result in a bigger collision area for the associated entity.
    /// </remarks>
    public Vector2 Dimensions { get; set; }

    protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
    {
        base.Serialize(writer, assetStore);
        writer.WriteVector2("Dimensions", Dimensions);
    }

    protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
    {
        base.Deserialize(reader, assetStore);
        Dimensions = reader.ReadVector2("Dimensions");
    }
}

internal sealed class RectangleColliderComponentFactory : ComponentFactory<RectangleColliderComponent>
{
    protected override RectangleColliderComponent CreateComponent(Entity entity) => new(entity);
}