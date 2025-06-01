using System;
using Geisha.Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Benchmark.Benchmarks.PrimitiveRendering
{
    internal sealed class StaticPrimitivesOutOfViewSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "StaticPrimitivesOutOfView";
        private readonly EntityFactory _entityFactory;

        public StaticPrimitivesOutOfViewSceneBehaviorFactory(EntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new StaticPrimitivesOutOfViewSceneBehavior(scene, _entityFactory);

        private sealed class StaticPrimitivesOutOfViewSceneBehavior : SceneBehavior
        {
            private readonly EntityFactory _entityFactory;

            public StaticPrimitivesOutOfViewSceneBehavior(Scene scene, EntityFactory entityFactory) : base(scene)
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