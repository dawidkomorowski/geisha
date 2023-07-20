using System;
using Geisha.Demo.Common;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Input;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Demo.Screens
{
    internal sealed class RenderingEntityHierarchySceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "Screen09_RenderingEntityHierarchy";
        private readonly CommonScreenFactory _commonScreenFactory;

        public RenderingEntityHierarchySceneBehaviorFactory(CommonScreenFactory commonScreenFactory)
        {
            _commonScreenFactory = commonScreenFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new RenderingEntityHierarchySceneBehavior(scene, _commonScreenFactory);

        private sealed class RenderingEntityHierarchySceneBehavior : SceneBehavior
        {
            private readonly CommonScreenFactory _commonScreenFactory;

            public RenderingEntityHierarchySceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory) : base(scene)
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
                textRenderer1.Pivot = new Vector2(800, 425);
                textRenderer1.Text = @"TODO You can control order of rendering within a layer.
Press [UP] and [DOWN] to change the order of rendering.";

                // Create entity representing red square.
                var entity1 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var entity1Transform = entity1.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                entity1Transform.Translation = new Vector2(-50, 0);
                // Add RectangleRendererComponent to entity so it can show red square on the screen.
                var rectangleRenderer1 = entity1.CreateComponent<RectangleRendererComponent>();
                // Set rectangle properties.
                rectangleRenderer1.Dimension = new Vector2(200, 200);
                rectangleRenderer1.Color = Color.Red;
                rectangleRenderer1.FillInterior = true;
                // Set sorting layer for entity so it follows rendering order defined by Rendering.SortingLayersOrder in file "engine-config.json".
                rectangleRenderer1.SortingLayerName = "Default";
                rectangleRenderer1.OrderInLayer = 1;

                // Create entity representing green square.
                var entity2 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var entity2Transform = entity2.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                entity2Transform.Translation = new Vector2(0, -50);
                // Add RectangleRendererComponent to entity so it can show green square on the screen.
                var rectangleRenderer2 = entity2.CreateComponent<RectangleRendererComponent>();
                // Set rectangle properties.
                rectangleRenderer2.Dimension = new Vector2(200, 200);
                rectangleRenderer2.Color = Color.Green;
                rectangleRenderer2.FillInterior = true;
                // Set sorting layer for entity so it follows rendering order defined by Rendering.SortingLayersOrder in file "engine-config.json".
                rectangleRenderer2.SortingLayerName = "Default";
                rectangleRenderer2.OrderInLayer = 2;
                // Add InputComponent to entity so we can handle user input.
                var inputComponent = entity2.CreateComponent<InputComponent>();
                // Set input mapping so UP and DOWN keys will trigger "PushUp" and "PullDown" actions.
                inputComponent.InputMapping = new InputMapping
                {
                    ActionMappings =
                    {
                        new ActionMapping
                        {
                            ActionName = "PushUp",
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
                            ActionName = "PullDown",
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
                // Bind "PushUp" action to call order in layer handling logic.
                inputComponent.BindAction("PushUp", () => { rectangleRenderer2.OrderInLayer = Math.Min(4, rectangleRenderer2.OrderInLayer + 2); });
                // Bind "PullDown" action to call order in layer handling logic.
                inputComponent.BindAction("PullDown", () => { rectangleRenderer2.OrderInLayer = Math.Max(0, rectangleRenderer2.OrderInLayer - 2); });

                // Create entity representing blue square.
                var entity3 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var entity3Transform = entity3.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                entity3Transform.Translation = new Vector2(50, -100);
                // Add RectangleRendererComponent to entity so it can show blue square on the screen.
                var rectangleRenderer3 = entity3.CreateComponent<RectangleRendererComponent>();
                // Set rectangle properties.
                rectangleRenderer3.Dimension = new Vector2(200, 200);
                rectangleRenderer3.Color = Color.Blue;
                rectangleRenderer3.FillInterior = true;
                // Set sorting layer for entity so it follows rendering order defined by Rendering.SortingLayersOrder in file "engine-config.json".
                rectangleRenderer3.SortingLayerName = "Default";
                rectangleRenderer3.OrderInLayer = 3;

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