using Geisha.Engine.Core.SceneModel;

namespace Benchmark.Benchmarks.EmptyScene
{
    internal sealed class EmptySceneSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "EmptyScene";

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene) => new EmptySceneSceneBehavior(scene);

        private sealed class EmptySceneSceneBehavior : SceneBehavior
        {
            public EmptySceneSceneBehavior(Scene scene) : base(scene)
            {
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
            }
        }
    }
}