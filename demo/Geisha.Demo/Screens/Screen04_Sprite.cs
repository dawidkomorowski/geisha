using Geisha.Demo.Common;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering;
using System;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Input;

namespace Geisha.Demo.Screens
{
    internal sealed class SpriteSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "Screen04_Sprite";
        private readonly CommonScreenFactory _commonScreenFactory;
        private readonly IAssetStore _assetStore;

        public SpriteSceneBehaviorFactory(CommonScreenFactory commonScreenFactory, IAssetStore assetStore)
        {
            _commonScreenFactory = commonScreenFactory;
            _assetStore = assetStore;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new SpriteSceneBehavior(scene, _commonScreenFactory, _assetStore);

        private sealed class SpriteSceneBehavior : SceneBehavior
        {
            private readonly CommonScreenFactory _commonScreenFactory;
            private readonly IAssetStore _assetStore;

            public SpriteSceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory, IAssetStore assetStore) : base(scene)
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
                textBlock1Transform.Translation = new Vector2(0, 225);
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
                textRenderer1.Text = "Geisha Engine provides components for rendering sprites.";

                // Create entity representing planet Earth.
                var entity = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var entityTransform = entity.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                entityTransform.Translation = new Vector2(0, 100);
                // Add SpriteRendererComponent to entity so it can show planet Earth sprite on the screen.
                var spriteRenderer = entity.CreateComponent<SpriteRendererComponent>();
                // Set sprite to be planet Earth (sprite is accessed through IAssetStore using id of asset file).
                spriteRenderer.Sprite = _assetStore.GetAsset<Sprite>(new AssetId(new Guid("d62a5ea8-73c4-44f0-9f80-26a9ad9ca74a")));
                // Add InputComponent to entity so we can handle user input.
                var inputComponent = entity.CreateComponent<InputComponent>();
                // Set input mapping so UP and DOWN keys will trigger "IncreaseOpacity" and "DecreaseOpacity" actions.
                inputComponent.InputMapping = new InputMapping
                {
                    ActionMappings =
                    {
                        new ActionMapping
                        {
                            ActionName = "IncreaseOpacity",
                            HardwareActions =
                            {
                                new HardwareAction
                                {
                                    HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Up)
                                }
                            }
                        },
                        new ActionMapping
                        {
                            ActionName = "DecreaseOpacity",
                            HardwareActions =
                            {
                                new HardwareAction
                                {
                                    HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Down)
                                }
                            }
                        }
                    }
                };
                // Bind "IncreaseOpacity" action to call opacity handling logic.
                inputComponent.BindAction("IncreaseOpacity", () => { spriteRenderer.Opacity += 0.1; });
                // Bind "DecreaseOpacity" action to call opacity handling logic.
                inputComponent.BindAction("DecreaseOpacity", () => { spriteRenderer.Opacity -= 0.1; });

                // Create entity representing second text block.
                var textBlock2 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var textBlock2Transform = textBlock2.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                textBlock2Transform.Translation = new Vector2(0, -50);
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
                textRenderer2.Text = @"You can specify opacity of sprite.
Press [UP] and [DOWN] to change opacity of sprite.";

                // Create entity representing third text block.
                var textBlock3 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var textBlock3Transform = textBlock3.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                textBlock3Transform.Translation = new Vector2(0, -325);
                // Add TextRendererComponent to entity so it can show text on the screen.
                var textRenderer3 = textBlock3.CreateComponent<TextRendererComponent>();
                // Set text properties.
                textRenderer3.Color = Color.Black;
                textRenderer3.FontSize = FontSize.FromDips(40);
                textRenderer3.TextAlignment = TextAlignment.Center;
                textRenderer3.ParagraphAlignment = ParagraphAlignment.Center;
                textRenderer3.MaxWidth = 1600;
                textRenderer3.MaxHeight = 900;
                textRenderer3.Pivot = new Vector2(800, 450);
                textRenderer3.Text = "Press [ENTER] to go to the next screen. Press [BACKSPACE] to go back.";
            }
        }
    }
}