using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;

namespace Geisha.Benchmark.Common;

internal sealed class GravityBehaviorComponent : BehaviorComponent
{
    private KinematicRigidBody2DComponent _kinematicRigidBody2DComponent = null!;

    public GravityBehaviorComponent(Entity entity) : base(entity)
    {
    }

    public Vector2 Gravity { get; set; } = new(0, -9.81);

    public override void OnStart()
    {
        _kinematicRigidBody2DComponent = Entity.GetComponent<KinematicRigidBody2DComponent>();
    }

    public override void OnFixedUpdate()
    {
        _kinematicRigidBody2DComponent.LinearVelocity += Gravity * GameTime.FixedDeltaTime.TotalSeconds;
    }
}

internal sealed class GravityBehaviorComponentFactory : ComponentFactory<GravityBehaviorComponent>
{
    protected override GravityBehaviorComponent CreateComponent(Entity entity) => new(entity);
}