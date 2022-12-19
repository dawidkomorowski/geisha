using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Demo
{
    internal sealed class HelloSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "HelloSceneBehavior";
        private readonly IAssetStore _assetStore;

        public HelloSceneBehaviorFactory(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new HelloSceneBehavior(scene, _assetStore);

        private sealed class HelloSceneBehavior : SceneBehavior
        {
            private readonly IAssetStore _assetStore;

            public HelloSceneBehavior(Scene scene, IAssetStore assetStore) : base(scene)
            {
                _assetStore = assetStore;
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                var cameraEntity = Scene.CreateEntity();
                cameraEntity.CreateComponent<Transform2DComponent>();
                var cameraComponent = cameraEntity.CreateComponent<CameraComponent>();
                cameraComponent.ViewRectangle = new Vector2(1600, 900);

                var entity = Scene.CreateEntity();
                entity.CreateComponent<Transform2DComponent>();
                var spriteRendererComponent = entity.CreateComponent<SpriteRendererComponent>();
                spriteRendererComponent.Sprite = _assetStore.GetAsset<Sprite>(new AssetId(new Guid("adcce4a8-9648-40ee-95b2-b5d984504dd6")));
            }
        }
    }
}