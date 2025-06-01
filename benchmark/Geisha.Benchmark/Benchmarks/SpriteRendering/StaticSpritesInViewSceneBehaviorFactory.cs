using System;
using Geisha.Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Benchmark.Benchmarks.SpriteRendering
{
    internal sealed class StaticSpritesInViewSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "StaticSpritesInView";
        private readonly EntityFactory _entityFactory;

        public StaticSpritesInViewSceneBehaviorFactory(EntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene) => new StaticSpritesInViewSceneBehavior(scene, _entityFactory);

        private sealed class StaticSpritesInViewSceneBehavior : SceneBehavior
        {
            private readonly EntityFactory _entityFactory;

            public StaticSpritesInViewSceneBehavior(Scene scene, EntityFactory entityFactory) : base(scene)
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

                    _entityFactory.CreateStaticSprite(Scene, x, y);
                }
            }
        }
    }
}