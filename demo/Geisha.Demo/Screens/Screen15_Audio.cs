using System;
using Geisha.Demo.Common;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Demo.Screens;

internal sealed class AudioSceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "Screen15_Audio";
    private readonly CommonScreenFactory _commonScreenFactory;
    private readonly IAudioPlayer _audioPlayer;
    private readonly IAssetStore _assetStore;

    public AudioSceneBehaviorFactory(CommonScreenFactory commonScreenFactory, IAudioBackend audioBackend, IAssetStore assetStore)
    {
        _commonScreenFactory = commonScreenFactory;
        _assetStore = assetStore;
        _audioPlayer = audioBackend.AudioPlayer;
    }

    public string BehaviorName => SceneBehaviorName;
    public SceneBehavior Create(Scene scene) => new AudioSceneBehavior(scene, _commonScreenFactory, _audioPlayer, _assetStore);

    private sealed class AudioSceneBehavior : SceneBehavior
    {
        private readonly CommonScreenFactory _commonScreenFactory;
        private readonly IAudioPlayer _audioPlayer;
        private readonly IAssetStore _assetStore;

        public AudioSceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory, IAudioPlayer audioPlayer, IAssetStore assetStore) : base(scene)
        {
            _commonScreenFactory = commonScreenFactory;
            _audioPlayer = audioPlayer;
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

            // Create entity showing active actions.
            var inputEntity = Scene.CreateEntity();
            // Add Transform2DComponent to entity to control its position.
            var inputTransform = inputEntity.CreateComponent<Transform2DComponent>();
            inputTransform.Translation = new Vector2(-500, -50);
            // Add TextRendererComponent to entity so it can show text on the screen.
            var inputText = inputEntity.CreateComponent<TextRendererComponent>();
            // Set text properties.
            inputText.Color = Color.FromArgb(255, 255, 0, 255);
            inputText.FontSize = FontSize.FromDips(40);
            inputText.TextAlignment = TextAlignment.Leading;
            inputText.ParagraphAlignment = ParagraphAlignment.Center;
            inputText.MaxWidth = 1600;
            inputText.MaxHeight = 900;
            inputText.Pivot = new Vector2(0, 450);
            // Add InputComponent to entity so we can handle user input.
            var inputComponent = inputEntity.CreateComponent<InputComponent>();
            // Set input mapping so selected keys will trigger corresponding actions.
            inputComponent.InputMapping = new InputMapping
            {
                ActionMappings =
                {
                    new ActionMapping
                    {
                        ActionName = "PlaySound1",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.D1)
                            }
                        }
                    },
                    new ActionMapping
                    {
                        ActionName = "PlaySound2",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.D2)
                            }
                        }
                    },
                    new ActionMapping
                    {
                        ActionName = "PlaySound3",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.D3)
                            }
                        }
                    },
                    new ActionMapping
                    {
                        ActionName = "IncreaseVolume",
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
                        ActionName = "DecreaseVolume",
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

            var sound1 = _assetStore.GetAsset<ISound>(new AssetId(new Guid("0a72baa4-22dd-41f1-b2c3-2f254501697a")));
            var sound2 = _assetStore.GetAsset<ISound>(new AssetId(new Guid("18e7fcdf-c857-44db-8b02-0186ec5d01ed")));
            var sound3 = _assetStore.GetAsset<ISound>(new AssetId(new Guid("521f3eea-7c3c-4a5a-8509-ae3c3c93bad3")));
            var volume = 1d;

            // Bind "SwitchKeyMap" action to call SwitchKeyMap function.
            inputComponent.BindAction("PlaySound1", () => _audioPlayer.PlayOnce(sound1, Math.Pow(volume, 2)));
            inputComponent.BindAction("PlaySound2", () => _audioPlayer.PlayOnce(sound2, Math.Pow(volume, 2)));
            inputComponent.BindAction("PlaySound3", () => _audioPlayer.PlayOnce(sound3, Math.Pow(volume, 2)));
            inputComponent.BindAction("IncreaseVolume", () =>
            {
                volume = Math.Min(volume + 0.1, 1d);
                inputText.Text = $"Volume {(int)(volume * 100)}%";
            });
            inputComponent.BindAction("DecreaseVolume", () =>
            {
                volume = Math.Max(volume - 0.1, 0d);
                inputText.Text = $"Volume {(int)(volume * 100)}%";
            });

            // Create entity representing first text block.
            var textBlock1 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var textBlock1Transform = textBlock1.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            textBlock1Transform.Translation = new Vector2(-650, 300);
            // Add TextRendererComponent to entity so it can show text on the screen.
            var textRenderer1 = textBlock1.CreateComponent<TextRendererComponent>();
            // Set text properties.
            textRenderer1.Color = Color.Black;
            textRenderer1.FontSize = FontSize.FromDips(40);
            textRenderer1.MaxWidth = 1300;
            textRenderer1.MaxHeight = 500;
            textRenderer1.Text = "InputComponent provides a way to map physical hardware input to abstract actions. " +
                                 "This allows to easily change mapping or trigger an action by multiple hardware inputs.";

            // Create entity representing second text block.
            var textBlock2 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var textBlock2Transform = textBlock2.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            textBlock2Transform.Translation = new Vector2(0, -225);
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
            textRenderer2.Text = "Press [TAB] to change input mapping.";

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