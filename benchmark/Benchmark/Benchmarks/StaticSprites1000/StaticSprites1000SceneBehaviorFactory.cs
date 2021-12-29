using Geisha.Common.Math;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Benchmark.Benchmarks.StaticSprites1000
{
    internal sealed class StaticSprites1000SceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "StaticSprites1000SceneBenchmark";
        private readonly IAssetStore _assetStore;

        public StaticSprites1000SceneBehaviorFactory(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene)
        {
            return new StaticSprites1000SceneBehavior(scene, _assetStore);
        }

        private sealed class StaticSprites1000SceneBehavior : SceneBehavior
        {
            private readonly IAssetStore _assetStore;

            public StaticSprites1000SceneBehavior(Scene scene, IAssetStore assetStore) : base(scene)
            {
                _assetStore = assetStore;
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                CreateCamera();

                for (var i = 0; i < 20; i++)
                {
                    for (var j = 0; j < 50; j++)
                    {
                        const int size = 32;
                        const int margin = 4;

                        var x = 0 - (margin / 2) - (size / 2) + (-24 + j) * (size + margin);
                        var y = 0 - (margin / 2) - (size / 2) + (-9 + i) * (size + margin);

                        CreateStaticSprite(x, y);
                    }
                }
            }

            private void CreateStaticSprite(double x, double y)
            {
                var entity = new Entity();
                Scene.AddEntity(entity);

                entity.AddComponent(new Transform2DComponent
                {
                    Translation = new Vector2(x, y),
                    Rotation = 0,
                    Scale = Vector2.One
                });
                entity.AddComponent(new SpriteRendererComponent
                {
                    Sprite = _assetStore.GetAsset<Sprite>(AssetsIds.PaintColorPalette)
                });
            }

            private void CreateCamera()
            {
                var entity = new Entity();
                Scene.AddEntity(entity);

                entity.AddComponent(new Transform2DComponent
                {
                    Translation = Vector2.Zero,
                    Rotation = 0,
                    Scale = Vector2.One
                });
                entity.AddComponent(new CameraComponent
                {
                    ViewRectangle = new Vector2(1280, 720)
                });
            }
        }
    }
}