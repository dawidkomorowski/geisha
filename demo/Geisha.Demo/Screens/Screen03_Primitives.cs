using System;
using Geisha.Demo.Common;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering;

namespace Geisha.Demo.Screens
{
    internal sealed class PrimitivesSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "Screen03_Primitives";
        private readonly CommonScreenFactory _commonScreenFactory;

        public PrimitivesSceneBehaviorFactory(CommonScreenFactory commonScreenFactory)
        {
            _commonScreenFactory = commonScreenFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new PrimitivesSceneBehavior(scene, _commonScreenFactory);

        private sealed class PrimitivesSceneBehavior : SceneBehavior
        {
            private readonly CommonScreenFactory _commonScreenFactory;

            public PrimitivesSceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory) : base(scene)
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

                // Create entity representing first text block.
                var textBlock1 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var textBlock1Transform = textBlock1.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                textBlock1Transform.Translation = new Vector2(-650, 250);
                // Add TextRendererComponent to entity so it can show text on the screen.
                var textRenderer1 = textBlock1.CreateComponent<TextRendererComponent>();
                // Set text properties.
                textRenderer1.Color = Color.Black;
                textRenderer1.FontSize = FontSize.FromDips(40);
                textRenderer1.MaxWidth = 1300;
                textRenderer1.MaxHeight = 500;
                textRenderer1.Text = "Geisha Engine provides components for rendering different primitive shapes.";

                // Create entity representing second text block.
                var textBlock2 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var textBlock2Transform = textBlock2.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                textBlock2Transform.Translation = new Vector2(0, 25);
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
                textRenderer2.Text = "Press [SPACE] to cycle through different primitives.";

                // Create entity representing primitive shape.
                var entity = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var entityTransform = entity.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                entityTransform.Translation = new Vector2(0, -150);
                // Add InputComponent to entity so we can handle user input.
                var inputComponent = entity.CreateComponent<InputComponent>();
                // Set input mapping so SPACE key will trigger "Cycle" action.
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
                inputComponent.BindAction("Cycle", () =>
                {
                    CyclePrimitives(entity);
                    UpdatePrimitiveRenderer(entity);
                });
                // We use entity name to keep track of current primitive shape so we need to set initial state.
                entity.Name = "Square";
                // Update entity to use correct renderer component.
                UpdatePrimitiveRenderer(entity);

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

            // Function cycling through sequence of primitives. Based on current entity
            // name it assigns new name to entity which represents next primitive shape.
            private static void CyclePrimitives(Entity entity)
            {
                entity.Name = entity.Name switch
                {
                    "Square" => "Rectangle",
                    "Rectangle" => "Rectangle Outline",
                    "Rectangle Outline" => "Circle",
                    "Circle" => "Ellipse",
                    "Ellipse" => "Ellipse Outline",
                    "Ellipse Outline" => "Square",
                    _ => throw new ArgumentOutOfRangeException(nameof(entity.Name))
                };
            }

            // Update renderer component of entity so it represents correct shape based on entity name.
            private static void UpdatePrimitiveRenderer(Entity entity)
            {
                // Check if entity has RectangleRendererComponent.
                if (entity.HasComponent<RectangleRendererComponent>())
                {
                    // If entity has RectangleRendererComponent we remove it from entity.
                    var component = entity.GetComponent<RectangleRendererComponent>();
                    entity.RemoveComponent(component);
                }

                // Check if entity has EllipseRendererComponent.
                if (entity.HasComponent<EllipseRendererComponent>())
                {
                    // If entity has EllipseRendererComponent we remove it from entity.
                    var component = entity.GetComponent<EllipseRendererComponent>();
                    entity.RemoveComponent(component);
                }

                // Based on entity name add correct renderer component.
                switch (entity.Name)
                {
                    case "Square":
                    {
                        // Add RectangleRendererComponent to entity.
                        var rectangleRenderer = entity.CreateComponent<RectangleRendererComponent>();
                        // Set rectangle properties.
                        rectangleRenderer.FillInterior = true;
                        rectangleRenderer.Color = Color.Red;
                        rectangleRenderer.Dimensions = new Vector2(100, 100);
                        break;
                    }
                    case "Rectangle":
                    {
                        // Add RectangleRendererComponent to entity.
                        var rectangleRenderer = entity.CreateComponent<RectangleRendererComponent>();
                        // Set rectangle properties.
                        rectangleRenderer.FillInterior = true;
                        rectangleRenderer.Color = Color.FromArgb(255, 255, 255, 0);
                        rectangleRenderer.Dimensions = new Vector2(200, 100);
                        break;
                    }
                    case "Rectangle Outline":
                    {
                        // Add RectangleRendererComponent to entity.
                        var rectangleRenderer = entity.CreateComponent<RectangleRendererComponent>();
                        // Set rectangle properties.
                        rectangleRenderer.FillInterior = false;
                        rectangleRenderer.Color = Color.Green;
                        rectangleRenderer.Dimensions = new Vector2(200, 100);
                        break;
                    }
                    case "Circle":
                    {
                        // Add EllipseRendererComponent to entity.
                        var ellipseRenderer = entity.CreateComponent<EllipseRendererComponent>();
                        // Set ellipse properties.
                        ellipseRenderer.FillInterior = true;
                        ellipseRenderer.Color = Color.FromArgb(255, 0, 255, 255);
                        ellipseRenderer.Radius = 50;
                        break;
                    }
                    case "Ellipse":
                    {
                        // Add EllipseRendererComponent to entity.
                        var ellipseRenderer = entity.CreateComponent<EllipseRendererComponent>();
                        // Set ellipse properties.
                        ellipseRenderer.FillInterior = true;
                        ellipseRenderer.Color = Color.Blue;
                        ellipseRenderer.RadiusX = 100;
                        ellipseRenderer.RadiusY = 50;
                        break;
                    }
                    case "Ellipse Outline":
                    {
                        // Add EllipseRendererComponent to entity.
                        var ellipseRenderer = entity.CreateComponent<EllipseRendererComponent>();
                        // Set ellipse properties.
                        ellipseRenderer.FillInterior = false;
                        ellipseRenderer.Color = Color.FromArgb(255, 255, 0, 255);
                        ellipseRenderer.RadiusX = 100;
                        ellipseRenderer.RadiusY = 50;
                        break;
                    }
                    default: throw new ArgumentOutOfRangeException(nameof(entity.Name));
                }
            }
        }
    }
}