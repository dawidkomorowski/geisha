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

            // Create entity that shows sound volume and triggers sound playback.
            var soundEntity = Scene.CreateEntity();
            // Add Transform2DComponent to entity to control its position.
            var volumeTextTransform = soundEntity.CreateComponent<Transform2DComponent>();
            volumeTextTransform.Translation = new Vector2(0, -225);
            // Add TextRendererComponent to entity so it can show text on the screen.
            var soundVolumeText = soundEntity.CreateComponent<TextRendererComponent>();
            // Set text properties.
            soundVolumeText.Color = Color.Black;
            soundVolumeText.FontSize = FontSize.FromDips(40);
            soundVolumeText.TextAlignment = TextAlignment.Center;
            soundVolumeText.ParagraphAlignment = ParagraphAlignment.Center;
            soundVolumeText.MaxWidth = 1600;
            soundVolumeText.MaxHeight = 900;
            soundVolumeText.Pivot = new Vector2(800, 450);
            // Add InputComponent to entity so we can handle user input.
            var inputComponent = soundEntity.CreateComponent<InputComponent>();
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
            // Get sound assets from IAssetStore.
            var sound1 = _assetStore.GetAsset<ISound>(new AssetId(new Guid("0a72baa4-22dd-41f1-b2c3-2f254501697a")));
            var sound2 = _assetStore.GetAsset<ISound>(new AssetId(new Guid("18e7fcdf-c857-44db-8b02-0186ec5d01ed")));
            var sound3 = _assetStore.GetAsset<ISound>(new AssetId(new Guid("521f3eea-7c3c-4a5a-8509-ae3c3c93bad3")));
            // Define variable for sound volume. It is in range [0,100] for on screen display.
            var volume = 100;
            soundVolumeText.Text = $"Volume {volume}% - Press [UP][DOWN]";
            // Bind "PlaySound#" actions to call functions that trigger playback of sound #.
            // Volume is normalized to be in range [0,1] and then squared to make the change more audible.
            inputComponent.BindAction("PlaySound1", () => _audioPlayer.PlayOnce(sound1, Math.Pow(volume / 100d, 2)));
            inputComponent.BindAction("PlaySound2", () => _audioPlayer.PlayOnce(sound2, Math.Pow(volume / 100d, 2)));
            inputComponent.BindAction("PlaySound3", () => _audioPlayer.PlayOnce(sound3, Math.Pow(volume / 100d, 2)));
            // Bind "IncreaseVolume"/"DecreaseVolume" actions to call functions that handle volume change and update on screen display.
            inputComponent.BindAction("IncreaseVolume", () =>
            {
                volume = Math.Min(volume + 10, 100);
                soundVolumeText.Text = $"Volume {volume}% - Press [UP][DOWN]";
            });
            inputComponent.BindAction("DecreaseVolume", () =>
            {
                volume = Math.Max(volume - 10, 0);
                soundVolumeText.Text = $"Volume {volume}% - Press [UP][DOWN]";
            });

            // TODO

            var sprite1 = _assetStore.GetAsset<Sprite>(new AssetId(new Guid("dafaa5ee-2f22-4756-9694-fb2300166bdf")));
            var sprite2 = _assetStore.GetAsset<Sprite>(new AssetId(new Guid("71b16835-65e1-4f6b-ab94-2448ff068083")));
            var sprite3 = _assetStore.GetAsset<Sprite>(new AssetId(new Guid("9ac8d30b-39e6-4f03-9219-ef8e7fc0037d")));
            AddSpriteWithLabel(sprite1, "Press [1]", -300, 0, Angle.Deg2Rad(-0));
            AddSpriteWithLabel(sprite2, "Press [2]", 0, 0);
            AddSpriteWithLabel(sprite3, "Press [3]", 300, 0);

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
            textRenderer1.Text = "Geisha Engine provides support for basic sound playback. " +
                                 "You can play, pause and stop the sound, make it play in a loop, and control its volume.";

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

        private void AddSpriteWithLabel(Sprite sprite, string label, double x, double y, double rotation = 0d)
        {
            var entity = Scene.CreateEntity();
            var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
            transform2DComponent.Translation = new Vector2(x, y);
            transform2DComponent.Rotation = rotation;
            var spriteRendererComponent = entity.CreateComponent<SpriteRendererComponent>();
            spriteRendererComponent.Sprite = sprite;

            var childEntity = entity.CreateChildEntity();
            var childTransform = childEntity.CreateComponent<Transform2DComponent>();
            childTransform.Translation = new Vector2(0, -100);
            var textRendererComponent = childEntity.CreateComponent<TextRendererComponent>();
            textRendererComponent.Color = Color.Black;
            textRendererComponent.FontSize = FontSize.FromDips(40);
            textRendererComponent.TextAlignment = TextAlignment.Center;
            textRendererComponent.ParagraphAlignment = ParagraphAlignment.Center;
            textRendererComponent.MaxWidth = 400;
            textRendererComponent.MaxHeight = 400;
            textRendererComponent.Pivot = new Vector2(200, 200);
            textRendererComponent.Text = label;
        }
    }
}