using System;
using Benchmark.Common;
using Geisha.Engine.Core.SceneModel;

namespace Benchmark.Benchmarks.Sprites
{
    internal sealed class MovingSpritesSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "MovingSprites";
        private readonly IEntityFactory _entityFactory;

        public MovingSpritesSceneBehaviorFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene) => new MovingSpritesSceneBehavior(scene, _entityFactory);

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

                const int screenWidth = 1280;
                const int screenHeight = 720;

                var random = new Random(0);

                for (var i = 0; i < 10000; i++)
                {
                    var x = (screenWidth * 3) * random.NextDouble() - ((screenWidth * 3) / 2d);
                    var y = (screenHeight * 3) * random.NextDouble() - ((screenHeight * 3) / 2d);

                    _entityFactory.CreateMovingSprite(Scene, x, y, random);
                }
            }
        }
    }
}