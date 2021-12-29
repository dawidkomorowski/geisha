using Geisha.Engine.Core.SceneModel;

namespace Benchmark.Benchmarks.NoComponents10000EntitiesScene
{
    internal sealed class NoComponents10000EntitiesSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "NoComponents10000EntitiesSceneBenchmark";

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene)
        {
            return new NoComponents10000EntitiesSceneBehavior(scene);
        }

        private sealed class NoComponents10000EntitiesSceneBehavior : SceneBehavior
        {
            public NoComponents10000EntitiesSceneBehavior(Scene scene) : base(scene)
            {
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                for (var i = 0; i < 10000; i++)
                {
                    Scene.AddEntity(new Entity());
                }
            }
        }
    }
}