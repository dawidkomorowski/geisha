using System;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Physics.Components;

// TODO Add documentation.
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