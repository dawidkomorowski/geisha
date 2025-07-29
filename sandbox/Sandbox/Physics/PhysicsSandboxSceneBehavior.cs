using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering.Components;
using Sandbox.Common;
using System;
using System.Runtime;

namespace Sandbox.Physics;

public sealed class PhysicsSandboxSceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "PhysicsSandbox";

    private readonly CommonEntityFactory _commonEntityFactory;

    public PhysicsSandboxSceneBehaviorFactory(CommonEntityFactory commonEntityFactory)
    {
        _commonEntityFactory = commonEntityFactory;
    }

    public string BehaviorName => SceneBehaviorName;
    public SceneBehavior Create(Scene scene) => new PhysicsSandboxSceneBehavior(scene, _commonEntityFactory);

    private sealed class PhysicsSandboxSceneBehavior : SceneBehavior
    {
        private readonly CommonEntityFactory _commonEntityFactory;

        public PhysicsSandboxSceneBehavior(Scene scene, CommonEntityFactory commonEntityFactory) : base(scene)
        {
            _commonEntityFactory = commonEntityFactory;
        }

        public override string Name => SceneBehaviorName;

        protected override void OnLoaded()
        {
            _commonEntityFactory.CreateCamera(Scene);
            _commonEntityFactory.CreateBasicControls(Scene);

            //Scene.CreateEntity().CreateComponent<InfoComponent>();
            //Scene.CreateEntity().CreateComponent<LayoutControllerComponent>();

            //var entity = PhysicsEntityFactory.CreateRectangleKinematicBody(Scene, 0, 0, 100, 100);
            //entity.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
            //entity.RemoveComponent(entity.GetComponent<DynamicPhysicsEntityComponent>());
            //entity.CreateComponent<EntityControllerComponent>();

            //var rectangleRendererComponent = entity.CreateComponent<RectangleRendererComponent>();
            //rectangleRendererComponent.Color = Color.Black;
            //rectangleRendererComponent.FillInterior = true;
            //rectangleRendererComponent.Dimensions = new Vector2(50, 50);

            var entityNotInterpolated = Scene.CreateEntity();
            entityNotInterpolated.CreateComponent<MovementComponent>().UseFixedUpdate = false;
            entityNotInterpolated.CreateComponent<Transform2DComponent>().Translation = new Vector2(0, 100);
            var r1 = entityNotInterpolated.CreateComponent<EllipseRendererComponent>();
            r1.Color = Color.Red;
            r1.FillInterior = true;
            r1.Radius = 25;

            var entityInterpolated = Scene.CreateEntity();
            entityInterpolated.CreateComponent<MovementComponent>().UseFixedUpdate = true;
            entityInterpolated.CreateComponent<Transform2DComponent>();
            var r2 = entityInterpolated.CreateComponent<RectangleRendererComponent>();
            r2.Color = Color.Black;
            r2.FillInterior = true;
            r2.Dimensions = new Vector2(50, 50);

            for (int i = -10; i < 10; i++)
            {
                var e = Scene.CreateEntity();
                e.CreateComponent<Transform2DComponent>().Translation = new Vector2(i * 100, 200);
                var ellipseRendererComponent = e.CreateComponent<RectangleRendererComponent>();
                ellipseRendererComponent.Color = Color.Blue;
                ellipseRendererComponent.FillInterior = true;
                ellipseRendererComponent.Dimensions = new Vector2(50, 50);
            }
        }
    }
}

public sealed class MovementComponent : BehaviorComponent
{
    private const double BaseSpeed = 400.0;
    private double _speed = BaseSpeed;

    public MovementComponent(Entity entity) : base(entity)
    {
    }

    public bool UseFixedUpdate { get; set; }

    public override void OnUpdate(GameTime gameTime)
    {
        if (UseFixedUpdate) return;

        var transformComponent = Entity.GetComponent<Transform2DComponent>();

        transformComponent.Translation += Vector2.UnitX * _speed * gameTime.DeltaTimeSeconds;

        if (transformComponent.Translation.X > 800)
        {
            _speed = -BaseSpeed;
        }

        if (transformComponent.Translation.X < -800)
        {
            _speed = BaseSpeed;
        }
    }

    public override void OnFixedUpdate()
    {
        if (!UseFixedUpdate) return;

        var transformComponent = Entity.GetComponent<Transform2DComponent>();

        transformComponent.Translation += Vector2.UnitX * _speed * GameTime.FixedDeltaTimeSeconds;

        if (transformComponent.Translation.X > 800)
        {
            _speed = -BaseSpeed;
        }

        if (transformComponent.Translation.X < -800)
        {
            _speed = BaseSpeed;
        }
    }
}

public sealed class MovementComponentFactory : ComponentFactory<MovementComponent>
{
    protected override MovementComponent CreateComponent(Entity entity) => new(entity);
}