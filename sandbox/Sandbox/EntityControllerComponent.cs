using System;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Physics.Components;

namespace Sandbox;

public sealed class EntityControllerComponent : BehaviorComponent
{
    private const double Velocity = 400;
    private const double AngularVelocity = Math.PI / 4;
    private InputComponent _inputComponent = null!;

    public Entity? ControlledEntity { get; set; }

    public EntityControllerComponent(Entity entity) : base(entity)
    {
    }

    public override void OnStart()
    {
        _inputComponent = Entity.GetComponent<InputComponent>();
    }

    public override void OnFixedUpdate()
    {
        if (ControlledEntity is null) return;

        var linearVelocity = Vector2.Zero;
        var angularVelocity = 0d;

        if (_inputComponent.HardwareInput.KeyboardInput.Up)
        {
            linearVelocity += Vector2.UnitY;
        }

        if (_inputComponent.HardwareInput.KeyboardInput.Down)
        {
            linearVelocity += -Vector2.UnitY;
        }

        if (_inputComponent.HardwareInput.KeyboardInput.Right)
        {
            linearVelocity += Vector2.UnitX;
        }

        if (_inputComponent.HardwareInput.KeyboardInput.Left)
        {
            linearVelocity += -Vector2.UnitX;
        }

        if (_inputComponent.HardwareInput.KeyboardInput.X)
        {
            angularVelocity -= AngularVelocity;
        }

        if (_inputComponent.HardwareInput.KeyboardInput.Z)
        {
            angularVelocity += AngularVelocity;
        }

        var kinematicBody = ControlledEntity.GetComponent<KinematicRigidBody2DComponent>();
        kinematicBody.LinearVelocity = linearVelocity.OfLength(Velocity);
        kinematicBody.AngularVelocity = angularVelocity;
    }
}

public sealed class EntityControllerComponentFactory : ComponentFactory<EntityControllerComponent>
{
    protected override EntityControllerComponent CreateComponent(Entity entity) => new(entity);
}