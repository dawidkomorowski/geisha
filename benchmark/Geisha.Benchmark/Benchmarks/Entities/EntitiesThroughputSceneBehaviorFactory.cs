using System;
using Geisha.Benchmark.Common;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Benchmark.Benchmarks.Entities
{
    internal sealed class EntitiesThroughputSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "EntitiesThroughput";
        private readonly IEntityFactory _entityFactory;

        public EntitiesThroughputSceneBehaviorFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new EntitiesThroughputSceneBehavior(scene, _entityFactory);

        private sealed class EntitiesThroughputSceneBehavior : SceneBehavior
        {
            private readonly IEntityFactory _entityFactory;

            public EntitiesThroughputSceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
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

                    _entityFactory.CreateTurret(Scene, x, y, random);
                }
            }
        }
    }
}