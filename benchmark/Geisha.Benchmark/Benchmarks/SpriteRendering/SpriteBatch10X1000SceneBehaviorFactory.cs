using System;
using Geisha.Benchmark.Common;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Benchmark.Benchmarks.SpriteRendering
{
    internal sealed class SpriteBatch10X1000SceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "SpriteBatch10X1000";
        private readonly EntityFactory _entityFactory;

        public SpriteBatch10X1000SceneBehaviorFactory(EntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene) => new SpriteBatch10X1000SceneBehavior(scene, _entityFactory);

        private sealed class SpriteBatch10X1000SceneBehavior : SceneBehavior
        {
            private readonly EntityFactory _entityFactory;

            public SpriteBatch10X1000SceneBehavior(Scene scene, EntityFactory entityFactory) : base(scene)
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
                    for (var batchId = 0; batchId < 10; batchId++)
                    {
                        const int columns = 5;
                        const int rows = 2;
                        var column = batchId % columns;
                        var row = (batchId / columns) % rows;
                        var x = row % 2 == 0
                            ? width / columns * random.NextDouble() - width / 2d + (column * (width / columns))
                            : width / columns * random.NextDouble() + width / 2d - ((column + 1) * (width / columns));
                        var y = height / rows * random.NextDouble() - height / 2d + (row * (height / rows));

                        var entity = _entityFactory.CreateStaticSprite(Scene, x, y, GetSpriteAssetId(batchId));
                        entity.GetComponent<Transform2DComponent>().Scale = Vector2.One * 0.04;
                    }
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