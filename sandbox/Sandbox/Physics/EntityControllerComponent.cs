using System;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Physics.Components;

namespace Sandbox.Physics;

public sealed class EntityControllerComponent : BehaviorComponent
{
    private const double LinearVelocity = 400;
    private const double AngularVelocity = Math.PI / 4;
    private InputComponent _inputComponent = null!;
    private KinematicRigidBody2DComponent _kinematicBody = null!;

    public EntityControllerComponent(Entity entity) : base(entity)
    {
    }

    public override void OnStart()
    {
        if (!Entity.HasComponent<InputComponent>())
        {
            Entity.CreateComponent<InputComponent>();
        }

        _inputComponent = Entity.GetComponent<InputComponent>();
        _kinematicBody = Entity.GetComponent<KinematicRigidBody2DComponent>();
    }

    public override void OnFixedUpdate()
    {
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

        _kinematicBody.LinearVelocity = linearVelocity.OfLength(LinearVelocity);
        _kinematicBody.AngularVelocity = angularVelocity;
    }
}

public sealed class EntityControllerComponentFactory : ComponentFactory<EntityControllerComponent>
{
    protected override EntityControllerComponent CreateComponent(Entity entity) => new(entity);
}