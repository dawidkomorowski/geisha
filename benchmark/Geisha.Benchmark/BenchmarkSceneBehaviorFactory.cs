using Geisha.Engine.Core.SceneModel;

namespace Benchmark
{
    internal sealed class BenchmarkSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "BenchmarkSceneBehavior";
        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene)
        {
            return new BenchmarkSceneBehavior(scene);
        }

        private sealed class BenchmarkSceneBehavior : SceneBehavior
        {
            public BenchmarkSceneBehavior(Scene scene) : base(scene)
            {
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
            }
        }
    }
}