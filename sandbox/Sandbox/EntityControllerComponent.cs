using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Physics.Components;

namespace Sandbox;

public sealed class EntityControllerComponent : BehaviorComponent
{
    private const double Velocity = 2000;
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

        var velocity = Vector2.Zero;

        if (_inputComponent.HardwareInput.KeyboardInput.Up)
        {
            velocity += Vector2.UnitY;
        }

        if (_inputComponent.HardwareInput.KeyboardInput.Down)
        {
            velocity += -Vector2.UnitY;
        }

        if (_inputComponent.HardwareInput.KeyboardInput.Right)
        {
            velocity += Vector2.UnitX;
        }

        if (_inputComponent.HardwareInput.KeyboardInput.Left)
        {
            velocity += -Vector2.UnitX;
        }

        var kinematicBody = ControlledEntity.GetComponent<KinematicRigidBody2DComponent>();
        kinematicBody.LinearVelocity = velocity.OfLength(Velocity);
    }
}

public sealed class EntityControllerComponentFactory : ComponentFactory<EntityControllerComponent>
{
    protected override EntityControllerComponent CreateComponent(Entity entity) => new(entity);
}