using System;
using Benchmark.Common;
using Geisha.Engine.Core.SceneModel;

namespace Benchmark.Benchmarks.Sprites
{
    internal sealed class MovingSpritesSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "MovingSpritesSceneBenchmark";
        private readonly IEntityFactory _entityFactory;

        public MovingSpritesSceneBehaviorFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene)
        {
            return new MovingSpritesSceneBehavior(scene, _entityFactory);
        }

        private sealed class MovingSpritesSceneBehavior : SceneBehavior
        {
            private readonly IEntityFactory _entityFactory;

            public MovingSpritesSceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
            {
                _entityFactory = entityFactory;
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                _entityFactory.CreateCamera(Scene);

                var random = new Random(0);

                for (var i = 0; i < 20; i++)
                {
                    for (var j = 0; j < 50; j++)
                    {
                        const int size = 32;
                        const int margin = 4;

                        var x = 0 - (margin / 2) - (size / 2) + (-24 + j) * (size + margin);
                        var y = 0 - (margin / 2) - (size / 2) + (-9 + i) * (size + margin);

                        _entityFactory.CreateMovingSprite(Scene, x, y, random.NextDouble() * 10);
                    }
                }
            }
        }
    }
}