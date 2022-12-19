using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Demo
{
    internal sealed class CommonScreenFactory
    {
        private readonly IAssetStore _assetStore;

        public CommonScreenFactory(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        public void CreateCommonScreen(Scene scene)
        {
            var cameraEntity = scene.CreateEntity();
            cameraEntity.CreateComponent<Transform2DComponent>();
            var cameraComponent = cameraEntity.CreateComponent<CameraComponent>();
            cameraComponent.ViewRectangle = new Vector2(1600, 900);

            var grid = scene.CreateEntity();
            grid.CreateComponent<Transform2DComponent>();
            var spriteRendererComponent = grid.CreateComponent<SpriteRendererComponent>();
            spriteRendererComponent.Sprite = _assetStore.GetAsset<Sprite>(new AssetId(new Guid("adcce4a8-9648-40ee-95b2-b5d984504dd6")));

            var link = scene.CreateEntity();
            var transform2DComponent = link.CreateComponent<Transform2DComponent>();
            transform2DComponent.Translation = new Vector2(-790, -420);
            var textRendererComponent = link.CreateComponent<TextRendererComponent>();
            textRendererComponent.Color = Color.FromArgb(255, 150, 150, 150);
            textRendererComponent.FontSize = FontSize.FromDips(20);
            textRendererComponent.Text = "https://github.com/dawidkomorowski/geisha/blob/master/src/Geisha.Engine/Engine.cs";
        }
    }
}