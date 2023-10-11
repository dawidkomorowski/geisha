using System;
using Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Benchmark.Benchmarks.PrimitiveRendering
{
    internal sealed class MovingPrimitivesOutOfViewSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "MovingPrimitivesOutOfView";
        private readonly IEntityFactory _entityFactory;

        public MovingPrimitivesOutOfViewSceneBehaviorFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new MovingPrimitivesOutOfViewSceneBehavior(scene, _entityFactory);

        private sealed class MovingPrimitivesOutOfViewSceneBehavior : SceneBehavior
        {
            private readonly IEntityFactory _entityFactory;

            public MovingPrimitivesOutOfViewSceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
            {
                _entityFactory = entityFactory;
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                var camera = _entityFactory.CreateCamera(Scene).GetComponent<CameraComponent>();

                var width = camera.ViewRectangle.X * 10;
                var height = camera.ViewRectangle.Y * 10;

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