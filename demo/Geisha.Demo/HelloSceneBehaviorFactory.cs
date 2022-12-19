using Geisha.Engine.Core.SceneModel;

namespace Geisha.Demo
{
    internal sealed class HelloSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "HelloSceneBehavior";
        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new HelloSceneBehavior(scene);

        private sealed class HelloSceneBehavior : SceneBehavior
        {
            public HelloSceneBehavior(Scene scene) : base(scene)
            {
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
            }
        }
    }
}