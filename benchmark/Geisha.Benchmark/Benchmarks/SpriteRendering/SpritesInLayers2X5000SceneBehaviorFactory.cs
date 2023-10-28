using System;
using Geisha.Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Benchmark.Benchmarks.SpriteRendering
{
    internal sealed class SpritesInLayers2X5000SceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "SpritesInLayers2X5000";
        private readonly IEntityFactory _entityFactory;

        public SpritesInLayers2X5000SceneBehaviorFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene) => new SpritesInLayers2X5000SceneBehavior(scene, _entityFactory);

        private sealed class SpritesInLayers2X5000SceneBehavior : SceneBehavior
        {
            private readonly IEntityFactory _entityFactory;

            public SpritesInLayers2X5000SceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
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
                    entity.GetComponent<SpriteRendererComponent>().SortingLayerName = $"Layer{i % 2}";
                }
            }
        }
    }
}