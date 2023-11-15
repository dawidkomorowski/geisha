using Geisha.Demo.Common;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Input;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Core.Assets;
using System;
using Geisha.Engine.Animation;

namespace Geisha.Demo.Screens
{
    internal sealed class SpriteAnimationSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "Screen11_SpriteAnimation";
        private readonly CommonScreenFactory _commonScreenFactory;
        private readonly IAssetStore _assetStore;

        public SpriteAnimationSceneBehaviorFactory(CommonScreenFactory commonScreenFactory, IAssetStore assetStore)
        {
            _commonScreenFactory = commonScreenFactory;
            _assetStore = assetStore;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new SpriteAnimationSceneBehavior(scene, _commonScreenFactory, _assetStore);

        private sealed class SpriteAnimationSceneBehavior : SceneBehavior
        {
            private readonly CommonScreenFactory _commonScreenFactory;
            private readonly IAssetStore _assetStore;

            public SpriteAnimationSceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory, IAssetStore assetStore) : base(scene)
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
                var cameraTransform = camera.CreateComponent<Transform2DComponent>();
                // Add CameraComponent to entity so we can control what is visible on the screen.
                var cameraComponent = camera.CreateComponent<CameraComponent>();
                // Set size of the camera to be 1600x900 units - in this case it corresponds to widow size in pixels.
                cameraComponent.ViewRectangle = new Vector2(1600, 900);


                var character = Scene.CreateEntity();
                var characterTransform = character.CreateComponent<Transform2DComponent>();
                characterTransform.Scale = new Vector2(10, 10);
                var spriteRendererComponent = character.CreateComponent<SpriteRendererComponent>();
                var spriteAnimationComponent = character.CreateComponent<SpriteAnimationComponent>();
                spriteAnimationComponent.AddAnimation("Idle",
                    _assetStore.GetAsset<SpriteAnimation>(new AssetId(new Guid("5ef754d7-ba6c-44cb-8ca5-588ba575600b"))));
                spriteAnimationComponent.AddAnimation("Walk",
                    _assetStore.GetAsset<SpriteAnimation>(new AssetId(new Guid("c47ae60d-5d0a-4d12-901a-a5e468b8f40b"))));
                spriteAnimationComponent.AddAnimation("Fire",
                    _assetStore.GetAsset<SpriteAnimation>(new AssetId(new Guid("ae6d28cf-5f73-44b4-a7fc-825ee382c340"))));
                spriteAnimationComponent.AddAnimation("Defeat",
                    _assetStore.GetAsset<SpriteAnimation>(new AssetId(new Guid("15a034af-2009-4963-9d2c-94048fb07dc6"))));
                spriteAnimationComponent.PlayInLoop = true;
                spriteAnimationComponent.PlayAnimation("Idle");

                // Add InputComponent to entity so we can handle user input.
                var inputComponent = character.CreateComponent<InputComponent>();
                // Set input mapping so selected keys will trigger corresponding actions.
                inputComponent.InputMapping = new InputMapping
                {
                    ActionMappings =
                    {
                        new ActionMapping
                        {
                            ActionName = "Cycle",
                            HardwareActions =
                            {
                                new HardwareAction
                                {
                                    HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Space)
                                }
                            }
                        }
                    }
                };
                // Bind "Cycle" action to call our cycle logic.
                inputComponent.BindAction("Cycle", () => { CycleAnimations(character); });


                // Create entity representing first text block.
                var textBlock1 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var textBlock1Transform = textBlock1.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                textBlock1Transform.Translation = new Vector2(-650, 300);
                // Add TextRendererComponent to entity so it can show text on the screen.
                var textRenderer1 = textBlock1.CreateComponent<TextRendererComponent>();
                // Set text properties.
                textRenderer1.SortingLayerName = "Menu";
                textRenderer1.Color = Color.Black;
                textRenderer1.FontSize = FontSize.FromDips(40);
                textRenderer1.MaxWidth = 1300;
                textRenderer1.MaxHeight = 500;
                textRenderer1.Text = "Geisha Engine provides components for animating sprites. It supports basic animation based on sprites sequence.";

                // Create entity representing controls info.
                var textBlock2 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var textBlock2Transform = textBlock2.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                textBlock2Transform.Translation = new Vector2(0, -225);
                // Add TextRendererComponent to entity so it can show text on the screen.
                var textRenderer2 = textBlock2.CreateComponent<TextRendererComponent>();
                // Set text properties.
                textRenderer2.SortingLayerName = "Menu";
                textRenderer2.Color = Color.Black;
                textRenderer2.FontSize = FontSize.FromDips(40);
                textRenderer2.TextAlignment = TextAlignment.Center;
                textRenderer2.ParagraphAlignment = ParagraphAlignment.Center;
                textRenderer2.MaxWidth = 1600;
                textRenderer2.MaxHeight = 900;
                textRenderer2.Pivot = new Vector2(800, 450);
                textRenderer2.Text = "Press [SPACE] to cycle through different animations.";

                // Create entity representing second text block.
                var textBlock3 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var textBlock3Transform = textBlock3.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                textBlock3Transform.Translation = new Vector2(0, -325);
                // Add TextRendererComponent to entity so it can show text on the screen.
                var textRenderer3 = textBlock3.CreateComponent<TextRendererComponent>();
                // Set text properties.
                textRenderer3.SortingLayerName = "Menu";
                textRenderer3.Color = Color.Black;
                textRenderer3.FontSize = FontSize.FromDips(40);
                textRenderer3.TextAlignment = TextAlignment.Center;
                textRenderer3.ParagraphAlignment = ParagraphAlignment.Center;
                textRenderer3.MaxWidth = 1600;
                textRenderer3.MaxHeight = 900;
                textRenderer3.Pivot = new Vector2(800, 450);
                textRenderer3.Text = "Press [ENTER] to go to the next screen. Press [BACKSPACE] to go back.";
            }

            // Function cycling through sequence of primitives. Based on current entity
            // name it assigns new name to entity which represents next primitive shape.
            private static void CycleAnimations(Entity entity)
            {
                var spriteAnimationComponent = entity.GetComponent<SpriteAnimationComponent>();
                var animationName = spriteAnimationComponent.CurrentAnimation?.Name;

                animationName = animationName switch
                {
                    "Idle" => "Walk",
                    "Walk" => "Fire",
                    "Fire" => "Defeat",
                    "Defeat" => "Idle",
                    _ => throw new ArgumentOutOfRangeException(nameof(animationName))
                };

                spriteAnimationComponent.PlayAnimation(animationName);
            }
        }
    }
}