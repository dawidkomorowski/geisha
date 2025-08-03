using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering.Components;
using Sandbox.Common;

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

            Scene.CreateEntity().CreateComponent<InfoComponent>();
            Scene.CreateEntity().CreateComponent<LayoutControllerComponent>();

            var entity = PhysicsEntityFactory.CreateRectangleKinematicBody(Scene, 0, 0, 100, 100);
            entity.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
            entity.RemoveComponent(entity.GetComponent<DynamicPhysicsEntityComponent>());
            entity.CreateComponent<EntityControllerComponent>();

            // Test entity with interpolated transform
            entity.GetComponent<Transform2DComponent>().IsInterpolated = true;
            var rectangleRendererComponent = entity.CreateComponent<RectangleRendererComponent>();
            rectangleRendererComponent.Color = Color.Black;
            rectangleRendererComponent.Dimensions = new Vector2(50, 50);
        }
    }
}