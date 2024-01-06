using System;
using Geisha.Engine.Core.SceneModel;

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
}

internal sealed class KinematicRigidBody2DComponentFactory : ComponentFactory<KinematicRigidBody2DComponent>
{
    protected override KinematicRigidBody2DComponent CreateComponent(Entity entity) => new(entity);
}