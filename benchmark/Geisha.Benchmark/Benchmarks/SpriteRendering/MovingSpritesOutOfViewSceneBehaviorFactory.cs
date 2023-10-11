using System;
using Geisha.Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Benchmark.Benchmarks.SpriteRendering
{
    internal sealed class MovingSpritesOutOfViewSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "MovingSpritesOutOfView";
        private readonly IEntityFactory _entityFactory;

        public MovingSpritesOutOfViewSceneBehaviorFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene) => new MovingSpritesOutOfViewSceneBehavior(scene, _entityFactory);

        private sealed class MovingSpritesOutOfViewSceneBehavior : SceneBehavior
        {
            private readonly IEntityFactory _entityFactory;

            public MovingSpritesOutOfViewSceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
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

                    _entityFactory.CreateMovingSprite(Scene, x, y, random);
                }
            }
        }
    }
}