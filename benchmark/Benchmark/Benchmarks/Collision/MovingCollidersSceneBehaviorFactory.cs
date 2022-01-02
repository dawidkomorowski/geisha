using System;
using Benchmark.Common;
using Geisha.Engine.Core.SceneModel;

namespace Benchmark.Benchmarks.Collision
{
    internal sealed class MovingCollidersSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "MovingCollidersSceneBenchmark";
        private readonly IEntityFactory _entityFactory;

        public MovingCollidersSceneBehaviorFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene)
        {
            return new MovingCollidersSceneBehavior(scene, _entityFactory);
        }

        private sealed class MovingCollidersSceneBehavior : SceneBehavior
        {
            private readonly IEntityFactory _entityFactory;

            public MovingCollidersSceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
            {
                _entityFactory = entityFactory;
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                _entityFactory.CreateCamera(Scene);

                var random = new Random(0);

                for (var i = 0; i < 200; i++)
                {
                    var x = random.Next(-1000, 1000);
                    var y = random.Next(-1000, 1000);

                    if (i % 2 == 0)
                    {
                        _entityFactory.CreateMovingCircleCollider(Scene, x, y, random.NextDouble());
                    }
                    else
                    {
                        _entityFactory.CreateMovingRectangleCollider(Scene, x, y, random.NextDouble());
                    }
                }
            }
        }
    }
}