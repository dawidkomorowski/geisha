using System;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Physics.Components;

[ComponentId("Geisha.Engine.Physics.KinematicRigidBody2DComponent")]
public sealed class KinematicRigidBody2DComponent : Component
{
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