using System;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Physics.Components;

namespace Sandbox.Physics;

public sealed class EntityControllerComponent : BehaviorComponent
{
    private const double AngularVelocity = Math.PI / 4;
    private double _linearVelocity = 400;
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

        UpdateInfoComponent();
    }

    public override void OnFixedUpdate()
    {
        var linearVelocity = Vector2.Zero;
        var angularVelocity = 0d;

        if (_inputComponent.HardwareInput.KeyboardInput is { LeftCtrl: false, Up: true })
        {
            linearVelocity += Vector2.UnitY;
        }

        if (_inputComponent.HardwareInput.KeyboardInput is { LeftCtrl: false, Down: true })
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

        if (_inputComponent.HardwareInput.KeyboardInput is { LeftCtrl: true, Up: true })
        {
            _linearVelocity += 5;
            UpdateInfoComponent();
        }

        if (_inputComponent.HardwareInput.KeyboardInput is { LeftCtrl: true, Down: true })
        {
            _linearVelocity -= 5;
            UpdateInfoComponent();
        }

        _kinematicBody.LinearVelocity = linearVelocity.OfLength(_linearVelocity);
        _kinematicBody.AngularVelocity = angularVelocity;
    }

    private void UpdateInfoComponent()
    {
        var infoComponent = Scene.RootEntities.Single(e => e.HasComponent<InfoComponent>()).GetComponent<InfoComponent>();
        infoComponent.OnLinearVelocity(_linearVelocity);
    }
}

public sealed class EntityControllerComponentFactory : ComponentFactory<EntityControllerComponent>
{
    protected override EntityControllerComponent CreateComponent(Entity entity) => new(entity);
}