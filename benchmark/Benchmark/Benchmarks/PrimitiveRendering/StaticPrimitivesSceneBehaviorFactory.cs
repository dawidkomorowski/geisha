using System;
using Benchmark.Common;
using Geisha.Engine.Core.SceneModel;

namespace Benchmark.Benchmarks.PrimitiveRendering
{
    internal sealed class StaticPrimitivesSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "StaticPrimitives";
        private readonly IEntityFactory _entityFactory;

        public StaticPrimitivesSceneBehaviorFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new StaticPrimitivesSceneBehavior(scene, _entityFactory);

        private sealed class StaticPrimitivesSceneBehavior : SceneBehavior
        {
            private readonly IEntityFactory _entityFactory;

            public StaticPrimitivesSceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
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
                    var x = screenWidth * 3 * random.NextDouble() - screenWidth * 3 / 2d;
                    var y = screenHeight * 3 * random.NextDouble() - screenHeight * 3 / 2d;

                    if (i % 2 == 0)
                    {
                        _entityFactory.CreateStaticEllipse(Scene, x, y, random);
                    }
                    else
                    {
                        _entityFactory.CreateStaticRectangle(Scene, x, y, random);
                    }
                }
            }
        }
    }
}