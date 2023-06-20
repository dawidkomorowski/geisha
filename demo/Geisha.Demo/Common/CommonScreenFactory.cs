using System;
using System.Runtime.CompilerServices;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Demo.Common
{
    internal sealed class CommonScreenFactory
    {
        private readonly IAssetStore _assetStore;

        public CommonScreenFactory(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        public void CreateBackgroundAndMenu(Scene scene, [CallerFilePath] string sourceFilePath = "")
        {
            var grid = scene.CreateEntity();
            grid.CreateComponent<Transform2DComponent>();
            var spriteRendererComponent = grid.CreateComponent<SpriteRendererComponent>();
            spriteRendererComponent.SortingLayerName = "Background";
            spriteRendererComponent.Sprite = _assetStore.GetAsset<Sprite>(new AssetId(new Guid("adcce4a8-9648-40ee-95b2-b5d984504dd6")));

            var currentSourceFile = new Uri(GetCurrentSourceFilePath());
            var sourceFile = new Uri(sourceFilePath);
            var sourceFileRelativeUri = currentSourceFile.MakeRelativeUri(sourceFile);
            var uri = new Uri(new Uri("https://github.com/dawidkomorowski/geisha/blob/master/demo/Geisha.Demo/Common/"), sourceFileRelativeUri);
            CreateLink(scene, uri.AbsoluteUri);

            CreateMenuItem(scene, "Escape - Exit", 0);
            CreateMenuItem(scene, "Enter - Next", 1);
            CreateMenuItem(scene, "Backspace - Previous", 2);
            CreateMenuItem(scene, "F1 - Go to URL", 3);

            var menuControls = scene.CreateEntity();
            menuControls.CreateComponent<InputComponent>();
            menuControls.CreateComponent<MenuControlsComponent>();
        }

        private static void CreateLink(Scene scene, string url)
        {
            var link = scene.CreateEntity();
            link.Name = "url";
            var transform2DComponent = link.CreateComponent<Transform2DComponent>();
            transform2DComponent.Translation = new Vector2(-790, -420);
            var textRendererComponent = link.CreateComponent<TextRendererComponent>();
            textRendererComponent.SortingLayerName = "Menu";
            textRendererComponent.Color = Color.FromArgb(255, 150, 150, 150);
            textRendererComponent.FontSize = FontSize.FromDips(20);
            textRendererComponent.MaxWidth = 1600;
            textRendererComponent.Text = url;
        }

        private static void CreateMenuItem(Scene scene, string text, int index)
        {
            var link = scene.CreateEntity();
            var transform2DComponent = link.CreateComponent<Transform2DComponent>();
            transform2DComponent.Translation = new Vector2(-790, 445 + index * -23d);
            var textRendererComponent = link.CreateComponent<TextRendererComponent>();
            textRendererComponent.SortingLayerName = "Menu";
            textRendererComponent.Color = Color.FromArgb(255, 150, 150, 150);
            textRendererComponent.FontSize = FontSize.FromDips(20);
            textRendererComponent.Text = text;
        }

        private static string GetCurrentSourceFilePath([CallerFilePath] string sourceFilePath = "")
        {
            return sourceFilePath;
        }
    }
}