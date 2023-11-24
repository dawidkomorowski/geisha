using Geisha.Demo.Common;
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

    public AudioSceneBehaviorFactory(CommonScreenFactory commonScreenFactory)
    {
        _commonScreenFactory = commonScreenFactory;
    }

    public string BehaviorName => SceneBehaviorName;
    public SceneBehavior Create(Scene scene) => new AudioSceneBehavior(scene, _commonScreenFactory);

    private sealed class AudioSceneBehavior : SceneBehavior
    {
        private readonly CommonScreenFactory _commonScreenFactory;

        public AudioSceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory) : base(scene)
        {
            _commonScreenFactory = commonScreenFactory;
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
                        ActionName = "SwitchKeyMap",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Tab)
                            }
                        }
                    },
                    new ActionMapping
                    {
                        ActionName = "Jump",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.W)
                            }
                        }
                    },
                    new ActionMapping
                    {
                        ActionName = "Attack",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Space)
                            }
                        }
                    },
                    new ActionMapping
                    {
                        ActionName = "Use",
                        HardwareActions =
                        {
                            new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.LeftShift)
                            }
                        }
                    }
                }
            };

            // Bind "SwitchKeyMap" action to call SwitchKeyMap function.
            //inputComponent.BindAction("SwitchKeyMap", SwitchKeyMap);

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