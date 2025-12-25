using System.Linq;
using Geisha.Demo.Common;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Input;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Demo.Screens;

internal sealed class CameraSceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "Screen10_Camera";
    private readonly CommonScreenFactory _commonScreenFactory;

    public CameraSceneBehaviorFactory(CommonScreenFactory commonScreenFactory)
    {
        _commonScreenFactory = commonScreenFactory;
    }

    public string BehaviorName => SceneBehaviorName;
    public SceneBehavior Create(Scene scene) => new CameraSceneBehavior(scene, _commonScreenFactory);

    private sealed class CameraSceneBehavior : SceneBehavior
    {
        private readonly CommonScreenFactory _commonScreenFactory;

        public CameraSceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory) : base(scene)
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
            var cameraTransform = camera.CreateComponent<Transform2DComponent>();
            // Set scale of camera to 0.5 to make camera view smaller.
            cameraTransform.Scale = new Vector2(0.5, 0.5);
            // Add CameraComponent to entity so we can control what is visible on the screen.
            var cameraComponent = camera.CreateComponent<CameraComponent>();
            // Set size of the camera to be 1600x900 units - in this case it corresponds to widow size in pixels.
            // Note: Leaving ViewRectangle at default (0, 0) would automatically use screen size as the effective view rectangle,
            // adapting to any resolution. Setting an explicit value enables logical scaling independent of screen resolution.
            cameraComponent.ViewRectangle = new Vector2(1600, 900);
            // Make menu a child of camera so it sticks to camera (is not affected by camera transformations).
            var menu = Scene.AllEntities.Single(e => e.Name == "Menu");
            menu.Parent = camera;
            // Add InputComponent to entity so we can handle user input.
            var inputComponent = camera.CreateComponent<InputComponent>();
            // Set input mapping so selected keys will trigger corresponding actions.
            inputComponent.InputMapping = new InputMapping
            {
                AxisMappings =
                {
                    new AxisMapping
                    {
                        AxisName = "MoveVertically",
                        HardwareAxes =
                        {
                            new HardwareAxis
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Up),
                                Scale = 1
                            },
                            new HardwareAxis
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Down),
                                Scale = -1
                            }
                        }
                    },
                    new AxisMapping
                    {
                        AxisName = "MoveHorizontally",
                        HardwareAxes =
                        {
                            new HardwareAxis
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Right),
                                Scale = 1
                            },
                            new HardwareAxis
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Left),
                                Scale = -1
                            }
                        }
                    }
                }
            };
            // Bind "MoveVertically" axis to call vertical camera movement logic.
            inputComponent.BindAxis("MoveVertically", value =>
            {
                var newTranslation = cameraTransform.Translation + Vector2.UnitY * 10 * value;
                cameraTransform.Translation = Vector2.Max(Vector2.Min(newTranslation, new Vector2(250, 250)), new Vector2(-250, -250));
            });
            // Bind "MoveHorizontally" axis to call horizontal camera movement logic.
            inputComponent.BindAxis("MoveHorizontally", value =>
            {
                var newTranslation = cameraTransform.Translation + Vector2.UnitX * 10 * value;
                cameraTransform.Translation = Vector2.Max(Vector2.Min(newTranslation, new Vector2(250, 250)), new Vector2(-250, -250));
            });

            // Create entity representing first text block.
            var textBlock1 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var textBlock1Transform = textBlock1.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            textBlock1Transform.Translation = new Vector2(-650, 150);
            // Add TextRendererComponent to entity so it can show text on the screen.
            var textRenderer1 = textBlock1.CreateComponent<TextRendererComponent>();
            // Set text properties.
            textRenderer1.SortingLayerName = "Menu";
            textRenderer1.Color = Color.Black;
            textRenderer1.FontSize = FontSize.FromDips(40);
            textRenderer1.MaxWidth = 1300;
            textRenderer1.MaxHeight = 500;
            textRenderer1.Text = "You can use Camera component to control what part of scene is visible on the screen.";
            // Make first text block a child of camera so it sticks to camera (is not affected by camera transformations).
            textBlock1.Parent = camera;

            // Create entity representing controls info.
            var controlsInfo = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var controlsInfoTransform = controlsInfo.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            controlsInfoTransform.Translation = new Vector2(0, -50);
            // Add TextRendererComponent to entity so it can show text on the screen.
            var controlsInfoRenderer = controlsInfo.CreateComponent<TextRendererComponent>();
            // Set text properties.
            controlsInfoRenderer.SortingLayerName = "Menu";
            controlsInfoRenderer.Color = Color.Black;
            controlsInfoRenderer.FontSize = FontSize.FromDips(40);
            controlsInfoRenderer.TextAlignment = TextAlignment.Center;
            controlsInfoRenderer.ParagraphAlignment = ParagraphAlignment.Center;
            controlsInfoRenderer.MaxWidth = 1600;
            controlsInfoRenderer.MaxHeight = 900;
            controlsInfoRenderer.Pivot = new Vector2(800, 450);
            controlsInfoRenderer.Text = "Press [UP][DOWN][LEFT][RIGHT] to move camera.";
            // Make controls info a child of camera so it sticks to camera (is not affected by camera transformations).
            controlsInfo.Parent = camera;

            // Create entity representing red square.
            var entity1 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var transform1 = entity1.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            transform1.Translation = new Vector2(300, -50);
            // Add RectangleRendererComponent to entity so it can show red square on the screen.
            var renderer1 = entity1.CreateComponent<RectangleRendererComponent>();
            // Set rectangle properties.
            renderer1.Dimensions = new Vector2(200, 200);
            renderer1.Color = Color.Red;
            renderer1.FillInterior = true;

            // Create entity representing green square.
            var entity2 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var transform2 = entity2.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            transform2.Translation = new Vector2(-350, -150);
            // Add RectangleRendererComponent to entity so it can show green square on the screen.
            var renderer2 = entity2.CreateComponent<RectangleRendererComponent>();
            // Set rectangle properties.
            renderer2.Dimensions = new Vector2(100, 100);
            renderer2.Color = Color.Green;
            renderer2.FillInterior = true;

            // Create entity representing blue square.
            var entity3 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var transform3 = entity3.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            transform3.Translation = new Vector2(-225, 225);
            // Add RectangleRendererComponent to entity so it can show blue square on the screen.
            var renderer3 = entity3.CreateComponent<RectangleRendererComponent>();
            // Set rectangle properties.
            renderer3.Dimensions = new Vector2(150, 150);
            renderer3.Color = Color.Blue;
            renderer3.FillInterior = true;

            // Create entity representing second text block.
            var textBlock2 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var textBlock2Transform = textBlock2.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            textBlock2Transform.Translation = new Vector2(0, -325);
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
            textRenderer2.Text = "Press [ENTER] to go to the next screen. Press [BACKSPACE] to go back.";
            // Make second text block a child of camera so it sticks to camera (is not affected by camera transformations).
            textBlock2.Parent = camera;
        }
    }
}