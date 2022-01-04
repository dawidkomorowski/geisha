using Benchmark.Common;
using Geisha.Engine.Core.SceneModel;

namespace Benchmark.Benchmarks.Sprites
{
    internal sealed class StaticSpritesSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "StaticSpritesSceneBenchmark";
        private readonly IEntityFactory _entityFactory;

        public StaticSpritesSceneBehaviorFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene) => new StaticSpritesSceneBehavior(scene, _entityFactory);

        private sealed class StaticSpritesSceneBehavior : SceneBehavior
        {
            private readonly IEntityFactory _entityFactory;

            public StaticSpritesSceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
            {
                _entityFactory = entityFactory;
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                _entityFactory.CreateCamera(Scene);

                for (var i = 0; i < 20; i++)
                {
                    for (var j = 0; j < 50; j++)
                    {
                        const int size = 32;
                        const int margin = 4;

                        var x = 0 - margin / 2 - size / 2 + (-24 + j) * (size + margin);
                        var y = 0 - margin / 2 - size / 2 + (-9 + i) * (size + margin);

                        _entityFactory.CreateStaticSprite(Scene, x, y);
                    }
                }
            }
        }
    }
}