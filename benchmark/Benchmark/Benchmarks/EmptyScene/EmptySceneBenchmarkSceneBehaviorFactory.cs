using Geisha.Engine.Core.SceneModel;

namespace Benchmark.Benchmarks.EmptyScene
{
    internal sealed class EmptySceneBenchmarkSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "EmptySceneBenchmark";

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene) => new EmptySceneBenchmarkSceneBehavior(scene);

        private sealed class EmptySceneBenchmarkSceneBehavior : SceneBehavior
        {
            public EmptySceneBenchmarkSceneBehavior(Scene scene) : base(scene)
            {
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
            }
        }
    }
}