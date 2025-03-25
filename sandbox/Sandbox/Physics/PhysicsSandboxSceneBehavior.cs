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

            PhysicsEntityFactory.CreateRectangleStaticBody(Scene, 0, -200, 100, 100);
            PhysicsEntityFactory.CreateRectangleStaticBody(Scene, -300, -300, 200, 100);
            PhysicsEntityFactory.CreateRectangleStaticBody(Scene, -600, -300, 50, 100);
            PhysicsEntityFactory.CreateRectangleStaticBody(Scene, -200, 300, 100, 100);
            PhysicsEntityFactory.CreateRectangleStaticBody(Scene, -300, 200, 100, 100);
            PhysicsEntityFactory.CreateRectangleStaticBody(Scene, 200, -300, 100, 100);
            PhysicsEntityFactory.CreateRectangleStaticBody(Scene, 300, -300, 100, 100);
            PhysicsEntityFactory.CreateRectangleStaticBody(Scene, 400, -300, 100, 100);

            PhysicsEntityFactory.CreateCircleStaticBody(Scene, 200, 0, 50);
            PhysicsEntityFactory.CreateCircleStaticBody(Scene, 350, 0, 50);
            PhysicsEntityFactory.CreateCircleStaticBody(Scene, 450, 0, 50);

            PhysicsEntityFactory.CreateRectangleKinematicBody(Scene, 0, 300, 100, 100);

            var entity = PhysicsEntityFactory.CreateRectangleKinematicBody(Scene, -300, 0, 100, 100);
            entity.CreateComponent<EntityControllerComponent>();
        }
    }
}