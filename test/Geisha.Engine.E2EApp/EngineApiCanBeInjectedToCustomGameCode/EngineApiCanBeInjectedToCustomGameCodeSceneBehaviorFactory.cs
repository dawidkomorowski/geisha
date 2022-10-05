using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.E2EApp.EngineApiCanBeInjectedToCustomGameCode
{
    internal sealed class EngineApiCanBeInjectedToCustomGameCodeSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "EngineApiCanBeInjectedToCustomGameCode";
        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new EngineApiCanBeInjectedToCustomGameCodeSceneBehavior(scene);

        private sealed class EngineApiCanBeInjectedToCustomGameCodeSceneBehavior : SceneBehavior
        {
            public EngineApiCanBeInjectedToCustomGameCodeSceneBehavior(Scene scene) : base(scene)
            {
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                var exitTestAppComponent = Scene.CreateEntity().CreateComponent<ExitTestAppComponent>();
                exitTestAppComponent.ExitOnFrame = 1;

                E2ETest.Report("9CA85BC0-A6B3-44ED-9FA7-C64F0909F1A3", "Engine API Injected Into SceneBehavior");

                Scene.CreateEntity().CreateComponent<EngineApiDependencyInjectionTestComponent>();
            }
        }
    }
}