using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Physics.Components;

/// <summary>
///     <see cref="KinematicRigidBody2DComponent" /> gives an entity capability of a 2D kinematic rigid body.
/// </summary>
/// <remarks>
///     <para>
///         A kinematic rigid body is not affected by forces. It can be moved manually by setting its velocity or directly
///         updating its position. This component is typically used to create entities that are controlled directly by user
///         input or AI.
///     </para>
///     <para>
///         To create 2D kinematic rigid body an entity needs to be composed of
///         <see cref="Core.Components.Transform2DComponent" />, one of collider components (see classes derived from
///         <see cref="Collider2DComponent" />) and <see cref="KinematicRigidBody2DComponent" />. Only root entities are
///         supported as 2D kinematic rigid bodies. Child colliders are not supported.
///     </para>
/// </remarks>
[ComponentId("Geisha.Engine.Physics.KinematicRigidBody2DComponent")]
public sealed class KinematicRigidBody2DComponent : Component
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="KinematicRigidBody2DComponent" /> class.
    /// </summary>
    /// <param name="entity">The entity to which this component is attached.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown when the entity already has a <see cref="KinematicRigidBody2DComponent" />
    ///     attached.
    /// </exception>
    public KinematicRigidBody2DComponent(Entity entity) : base(entity)
    {
        if (entity.HasComponent<KinematicRigidBody2DComponent>())
        {
            throw new ArgumentException($"{nameof(KinematicRigidBody2DComponent)} is already added to entity.");
        }
    }

    /// <summary>
    ///     Gets or sets linear velocity of 2D kinematic rigid body in meters per second.
    /// </summary>
    public Vector2 LinearVelocity { get; set; }

    /// <summary>
    ///     Gets or sets angular velocity of 2D kinematic rigid body in radians per second. Rotation is counterclockwise.
    /// </summary>
    public double AngularVelocity { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether collision response is enabled for this kinematic rigid body.
    /// </summary>
    /// <remarks>
    ///     When set to <c>true</c>, the kinematic rigid body will receive appropriate collision responses when colliding with
    ///     other objects. When set to <c>false</c>, collisions will be detected but no response will be applied to this body.
    /// </remarks>
    public bool EnableCollisionResponse { get; set; }

    protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
    {
        base.Serialize(writer, assetStore);
        writer.WriteVector2("LinearVelocity", LinearVelocity);
        writer.WriteDouble("AngularVelocity", AngularVelocity);
        writer.WriteBool("EnableCollisionResponse", EnableCollisionResponse);
    }

    protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
    {
        base.Deserialize(reader, assetStore);
        LinearVelocity = reader.ReadVector2("LinearVelocity");
        AngularVelocity = reader.ReadDouble("AngularVelocity");
        EnableCollisionResponse = reader.ReadBool("EnableCollisionResponse");
    }
}

internal sealed class KinematicRigidBody2DComponentFactory : ComponentFactory<KinematicRigidBody2DComponent>
{
    protected override KinematicRigidBody2DComponent CreateComponent(Entity entity) => new(entity);
}