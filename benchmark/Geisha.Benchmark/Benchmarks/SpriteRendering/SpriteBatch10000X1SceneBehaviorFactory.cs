using System;
using Benchmark.Common;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Benchmark.Benchmarks.SpriteRendering
{
    internal sealed class SpriteBatch10000X1SceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "SpriteBatch10000X1";
        private readonly IEntityFactory _entityFactory;

        public SpriteBatch10000X1SceneBehaviorFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene) => new SpriteBatch10000X1SceneBehavior(scene, _entityFactory);

        private sealed class SpriteBatch10000X1SceneBehavior : SceneBehavior
        {
            private readonly IEntityFactory _entityFactory;

            public SpriteBatch10000X1SceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
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

                for (var batchId = 0; batchId < 10000; batchId++)
                {
                    var x = width * random.NextDouble() - width / 2d;
                    var y = height * random.NextDouble() - height / 2d;

                    var entity = _entityFactory.CreateStaticSprite(Scene, x, y, GetSpriteAssetId(batchId), batchId / 10);
                    entity.GetComponent<Transform2DComponent>().Scale = Vector2.One * 0.04;
                }
            }

            private static AssetId GetSpriteAssetId(int batchId)
            {
                return (batchId % 10) switch
                {
                    0 => AssetsIds.Planet00,
                    1 => AssetsIds.Planet01,
                    2 => AssetsIds.Planet02,
                    3 => AssetsIds.Planet03,
                    4 => AssetsIds.Planet04,
                    5 => AssetsIds.Planet05,
                    6 => AssetsIds.Planet06,
                    7 => AssetsIds.Planet07,
                    8 => AssetsIds.Planet08,
                    9 => AssetsIds.Planet09,
                    _ => throw new ArgumentOutOfRangeException(nameof(batchId))
                };
            }
        }
    }
}