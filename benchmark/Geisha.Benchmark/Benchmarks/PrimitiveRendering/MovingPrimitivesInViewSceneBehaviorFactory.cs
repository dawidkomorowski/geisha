using System;
using Geisha.Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Benchmark.Benchmarks.PrimitiveRendering
{
    internal sealed class MovingPrimitivesInViewSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "MovingPrimitivesInView";
        private readonly EntityFactory _entityFactory;

        public MovingPrimitivesInViewSceneBehaviorFactory(EntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new MovingPrimitivesInViewSceneBehavior(scene, _entityFactory);

        private sealed class MovingPrimitivesInViewSceneBehavior : SceneBehavior
        {
            private readonly EntityFactory _entityFactory;

            public MovingPrimitivesInViewSceneBehavior(Scene scene, EntityFactory entityFactory) : base(scene)
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

                for (var i = 0; i < 10000; i++)
                {
                    var x = width * random.NextDouble() - width / 2d;
                    var y = height * random.NextDouble() - height / 2d;

                    if (i % 2 == 0)
                    {
                        _entityFactory.CreateMovingEllipse(Scene, x, y, random);
                    }
                    else
                    {
                        _entityFactory.CreateMovingRectangle(Scene, x, y, random);
                    }
                }
            }
        }
    }
}