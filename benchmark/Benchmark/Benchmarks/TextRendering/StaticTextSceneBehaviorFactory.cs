using Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using System;

namespace Benchmark.Benchmarks.TextRendering
{
    internal sealed class StaticTextSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "StaticText";
        private readonly IEntityFactory _entityFactory;

        public StaticTextSceneBehaviorFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new StaticTextSceneBehavior(scene, _entityFactory);

        private sealed class StaticTextSceneBehavior : SceneBehavior
        {
            private readonly IEntityFactory _entityFactory;

            public StaticTextSceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
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

                for (var i = 0; i < 1000; i++)
                {
                    var x = screenWidth * 3 * random.NextDouble() - screenWidth * 3 / 2d;
                    var y = screenHeight * 3 * random.NextDouble() - screenHeight * 3 / 2d;

                    _entityFactory.CreateStaticText(Scene, x, y, random);
                }
            }
        }
    }
}