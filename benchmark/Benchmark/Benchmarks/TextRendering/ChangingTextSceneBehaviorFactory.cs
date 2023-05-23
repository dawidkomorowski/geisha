using System;
using Benchmark.Common;
using Geisha.Engine.Core.SceneModel;

namespace Benchmark.Benchmarks.TextRendering
{
    internal sealed class ChangingTextSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "ChangingText";
        private readonly IEntityFactory _entityFactory;

        public ChangingTextSceneBehaviorFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new ChangingTextSceneBehavior(scene, _entityFactory);

        internal sealed class ChangingTextSceneBehavior : SceneBehavior
        {
            private readonly IEntityFactory _entityFactory;

            public ChangingTextSceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
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

                    _entityFactory.CreateChangingText(Scene, x, y, random);
                }
            }
        }
    }
}