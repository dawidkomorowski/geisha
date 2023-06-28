using Geisha.Demo.Common;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using System;

namespace Geisha.Demo.Screens
{
    internal sealed class TransformSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "Screen06_Transform";
        private readonly CommonScreenFactory _commonScreenFactory;
        private readonly IAssetStore _assetStore;

        public TransformSceneBehaviorFactory(CommonScreenFactory commonScreenFactory, IAssetStore assetStore)
        {
            _commonScreenFactory = commonScreenFactory;
            _assetStore = assetStore;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new TransformSceneBehavior(scene, _commonScreenFactory, _assetStore);

        private sealed class TransformSceneBehavior : SceneBehavior
        {
            private readonly CommonScreenFactory _commonScreenFactory;
            private readonly IAssetStore _assetStore;

            public TransformSceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory, IAssetStore assetStore) : base(scene)
            {
                _commonScreenFactory = commonScreenFactory;
                _assetStore = assetStore;
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                // Create background and menu.
                _commonScreenFactory.CreateBackgroundAndMenu(Scene);

                // Create entity representing camera.
                var camera = Scene.CreateEntity();
                // Add Transform2DComponent to entity to set position of the camera at origin.
                camera.CreateComponent<Transform2DComponent>();
                // Add CameraComponent to entity so we can control what is visible on the screen.
                var cameraComponent = camera.CreateComponent<CameraComponent>();
                // Set size of the camera to be 1600x900 units - in this case it corresponds to widow size in pixels.
                cameraComponent.ViewRectangle = new Vector2(1600, 900);

                // Create entity representing first text block.
                var textBlock1 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var textBlock1Transform = textBlock1.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                textBlock1Transform.Translation = new Vector2(0, 200);
                // Add TextRendererComponent to entity so it can show text on the screen.
                var textRenderer1 = textBlock1.CreateComponent<TextRendererComponent>();
                // Set text properties.
                textRenderer1.Color = Color.Black;
                textRenderer1.FontSize = FontSize.FromDips(40);
                textRenderer1.TextAlignment = TextAlignment.Center;
                textRenderer1.ParagraphAlignment = ParagraphAlignment.Center;
                textRenderer1.MaxWidth = 1600;
                textRenderer1.MaxHeight = 900;
                textRenderer1.Pivot = new Vector2(800, 450);
                textRenderer1.Text = "Geisha Engine provides transform components for controlling position, rotation and scale of entities.";

                // Create entity representing computer.
                var entity1 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var entity1Transform = entity1.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                entity1Transform.Translation = new Vector2(-400, -50);
                // Add SpriteRendererComponent to entity so it can show computer sprite on the screen.
                var spriteRenderer1 = entity1.CreateComponent<SpriteRendererComponent>();
                // Set sprite to be computer (sprite is accessed through IAssetStore using id of asset file).
                spriteRenderer1.Sprite = _assetStore.GetAsset<Sprite>(new AssetId(new Guid("9ac8d30b-39e6-4f03-9219-ef8e7fc0037d")));

                // Create entity representing computer.
                var entity2 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var entity2Transform = entity2.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                entity2Transform.Translation = new Vector2(0, -50);
                entity2Transform.Rotation = Angle.Deg2Rad(45);
                // Add SpriteRendererComponent to entity so it can show computer sprite on the screen.
                var spriteRenderer2 = entity2.CreateComponent<SpriteRendererComponent>();
                // Set sprite to be computer (sprite is accessed through IAssetStore using id of asset file).
                spriteRenderer2.Sprite = _assetStore.GetAsset<Sprite>(new AssetId(new Guid("9ac8d30b-39e6-4f03-9219-ef8e7fc0037d")));

                // Create entity representing computer.
                var entity3 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var entity3Transform = entity3.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                entity3Transform.Translation = new Vector2(400, -50);
                entity3Transform.Scale = new Vector2(2, 2);
                // Add SpriteRendererComponent to entity so it can show computer sprite on the screen.
                var spriteRenderer3 = entity3.CreateComponent<SpriteRendererComponent>();
                // Set sprite to be computer (sprite is accessed through IAssetStore using id of asset file).
                spriteRenderer3.Sprite = _assetStore.GetAsset<Sprite>(new AssetId(new Guid("9ac8d30b-39e6-4f03-9219-ef8e7fc0037d")));

                // Create entity representing second text block.
                var textBlock2 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var textBlock2Transform = textBlock2.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                textBlock2Transform.Translation = new Vector2(0, -325);
                // Add TextRendererComponent to entity so it can show text on the screen.
                var textRenderer2 = textBlock2.CreateComponent<TextRendererComponent>();
                // Set text properties.
                textRenderer2.Color = Color.Black;
                textRenderer2.FontSize = FontSize.FromDips(40);
                textRenderer2.TextAlignment = TextAlignment.Center;
                textRenderer2.ParagraphAlignment = ParagraphAlignment.Center;
                textRenderer2.MaxWidth = 1600;
                textRenderer2.MaxHeight = 900;
                textRenderer2.Pivot = new Vector2(800, 450);
                textRenderer2.Text = "Press [ENTER] to go to the next screen. Press [BACKSPACE] to go back.";
            }
        }
    }
}