using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.E2EApp
{
    internal sealed class MainSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "MainSceneBehavior";
        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new MainSceneBehavior(scene);

        private sealed class MainSceneBehavior : SceneBehavior
        {
            public MainSceneBehavior(Scene scene) : base(scene)
            {
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                var exitTestAppComponent = Scene.CreateEntity().CreateComponent<ExitTestAppComponent>();
                exitTestAppComponent.ExitOnFrame = 5;
            }
        }
    }
}