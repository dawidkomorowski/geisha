using System;
using Geisha.Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Benchmark.Benchmarks.SpriteRendering
{
    internal sealed class SpritesInLayers10X1000SceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "SpritesInLayers10X1000";
        private readonly IEntityFactory _entityFactory;

        public SpritesInLayers10X1000SceneBehaviorFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene) => new SpritesInLayers10X1000SceneBehavior(scene, _entityFactory);

        private sealed class SpritesInLayers10X1000SceneBehavior : SceneBehavior
        {
            private readonly IEntityFactory _entityFactory;

            public SpritesInLayers10X1000SceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
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

                    var entity = _entityFactory.CreateStaticSprite(Scene, x, y);
                    entity.GetComponent<SpriteRendererComponent>().SortingLayerName = $"Layer{i % 10}";
                }
            }
        }
    }
}