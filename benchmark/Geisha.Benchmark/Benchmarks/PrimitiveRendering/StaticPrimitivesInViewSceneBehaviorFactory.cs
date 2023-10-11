using System;
using Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Benchmark.Benchmarks.PrimitiveRendering
{
    internal sealed class StaticPrimitivesInViewSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "StaticPrimitivesInView";
        private readonly IEntityFactory _entityFactory;

        public StaticPrimitivesInViewSceneBehaviorFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new StaticPrimitivesInViewSceneBehavior(scene, _entityFactory);

        private sealed class StaticPrimitivesInViewSceneBehavior : SceneBehavior
        {
            private readonly IEntityFactory _entityFactory;

            public StaticPrimitivesInViewSceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
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