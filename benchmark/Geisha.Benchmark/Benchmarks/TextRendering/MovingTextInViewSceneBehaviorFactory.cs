using System;
using Geisha.Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Benchmark.Benchmarks.TextRendering
{
    internal sealed class MovingTextInViewSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "MovingTextInView";
        private readonly EntityFactory _entityFactory;

        public MovingTextInViewSceneBehaviorFactory(EntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new MovingTextInViewSceneBehavior(scene, _entityFactory);

        private sealed class MovingTextInViewSceneBehavior : SceneBehavior
        {
            private readonly EntityFactory _entityFactory;

            public MovingTextInViewSceneBehavior(Scene scene, EntityFactory entityFactory) : base(scene)
            {
                _entityFactory = entityFactory;
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                var camera = _entityFactory.CreateCamera(Scene).GetComponent<CameraComponent>();

                var width = camera.ViewRectangle.X;
                var height = camera.ViewRectangle.Y;

                var random = new Random(0);

                for (var i = 0; i < 1000; i++)
                {
                    var x = width * random.NextDouble() - width / 2d;
                    var y = height * random.NextDouble() - height / 2d;

                    _entityFactory.CreateMovingText(Scene, x, y, random, true);
                }
            }
        }
    }
}