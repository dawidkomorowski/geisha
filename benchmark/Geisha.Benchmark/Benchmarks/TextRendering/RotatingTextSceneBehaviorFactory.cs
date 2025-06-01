using System;
using Geisha.Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Benchmark.Benchmarks.TextRendering
{
    internal sealed class RotatingTextSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "RotatingText";
        private readonly EntityFactory _entityFactory;

        public RotatingTextSceneBehaviorFactory(EntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new RotatingTextSceneBehavior(scene, _entityFactory);

        private sealed class RotatingTextSceneBehavior : SceneBehavior
        {
            private readonly EntityFactory _entityFactory;

            public RotatingTextSceneBehavior(Scene scene, EntityFactory entityFactory) : base(scene)
            {
                _entityFactory = entityFactory;
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                var camera = _entityFactory.CreateCamera(Scene).GetComponent<CameraComponent>();

                var width = camera.ViewRectangle.X * 3;
                var height = camera.ViewRectangle.Y * 3;

                var random = new Random(0);

                for (var i = 0; i < 1000; i++)
                {
                    var x = width * random.NextDouble() - width / 2d;
                    var y = height * random.NextDouble() - height / 2d;

                    _entityFactory.CreateMovingText(Scene, x, y, random, false);
                }
            }
        }
    }
}