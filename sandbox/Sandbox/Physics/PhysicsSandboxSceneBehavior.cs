using Geisha.Engine.Core.SceneModel;

namespace Sandbox.Physics;

public sealed class PhysicsSandboxSceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "PhysicsSandbox";

    public string BehaviorName => SceneBehaviorName;
    public SceneBehavior Create(Scene scene) => new PhysicsSandboxSceneBehavior(scene);

    private sealed class PhysicsSandboxSceneBehavior : SceneBehavior
    {
        public PhysicsSandboxSceneBehavior(Scene scene) : base(scene)
        {
        }

        public override string Name => SceneBehaviorName;

        protected override void OnLoaded()
        {
        }
    }
}