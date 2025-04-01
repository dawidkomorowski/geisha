using Geisha.Engine.Core.SceneModel;
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
            entity.RemoveComponent(entity.GetComponent<DynamicPhysicsEntityComponent>());
            entity.CreateComponent<EntityControllerComponent>();
        }
    }
}