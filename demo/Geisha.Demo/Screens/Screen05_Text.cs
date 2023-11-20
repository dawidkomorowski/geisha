using Geisha.Demo.Common;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Input;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using System;

namespace Geisha.Demo.Screens;

internal sealed class TextSceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "Screen05_Text";
    private readonly CommonScreenFactory _commonScreenFactory;

    public TextSceneBehaviorFactory(CommonScreenFactory commonScreenFactory)
    {
        _commonScreenFactory = commonScreenFactory;
    }

    public string BehaviorName => SceneBehaviorName;
    public SceneBehavior Create(Scene scene) => new TextSceneBehavior(scene, _commonScreenFactory);

    private sealed class TextSceneBehavior : SceneBehavior
    {
        private readonly CommonScreenFactory _commonScreenFactory;

        public TextSceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory) : base(scene)
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
            textRenderer1.Pivot = new Vector2(800, 450);
            textRenderer1.Text = "Geisha Engine provides components for rendering text.";

            // Create entity representing second text block.
            var textBlock2 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var textBlock2Transform = textBlock2.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            textBlock2Transform.Translation = new Vector2(0, 125);
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
            textRenderer2.Text = "Press [SPACE] to cycle through different text rendering features.";

            // Create entity representing text background.
            var background = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var backgroundTransform = background.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            backgroundTransform.Translation = new Vector2(0, -100);
            // Add RectangleRendererComponent to entity.
            var rectangleRenderer = background.CreateComponent<RectangleRendererComponent>();
            // Set rectangle properties.
            rectangleRenderer.OrderInLayer = -1;
            rectangleRenderer.FillInterior = true;
            rectangleRenderer.Color = Color.FromArgb(255, 50, 50, 50);
            rectangleRenderer.Dimensions = new Vector2(400, 300);

            // Create entity representing text block.
            var textBlockEntity = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var textBlockTransform = textBlockEntity.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            textBlockTransform.Translation = new Vector2(0, -100);
            // Add TextRendererComponent to entity so it can show text on the screen.
            var textRenderer = textBlockEntity.CreateComponent<TextRendererComponent>();
            // Set text properties.
            textRenderer.MaxWidth = 380;
            textRenderer.MaxHeight = 280;
            textRenderer.Pivot = new Vector2(190, 140);
            // Add InputComponent to entity so we can handle user input.
            var inputComponent = textBlockEntity.CreateComponent<InputComponent>();
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
                CycleTextRenderingFeatures(textBlockEntity);
                UpdateTextRenderer(textBlockEntity);
            });
            // We use entity name to keep track of current text rendering feature so we need to set initial state.
            textBlockEntity.Name = "Preview";
            // Update text renderer component of entity.
            UpdateTextRenderer(textBlockEntity);

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

        // Function cycling through sequence of text rendering features. Based on current entity
        // name it assigns new name to entity which represents next text rendering feature.
        private static void CycleTextRenderingFeatures(Entity entity)
        {
            entity.Name = entity.Name switch
            {
                "Preview" => "Font1",
                "Font1" => "Font2",
                "Font2" => "FontSize1",
                "FontSize1" => "FontSize2",
                "FontSize2" => "Wrapping",
                "Wrapping" => "Color1",
                "Color1" => "Color2",
                "Color2" => "TextAlignment_Left",
                "TextAlignment_Left" => "TextAlignment_Right",
                "TextAlignment_Right" => "TextAlignment_Justified",
                "TextAlignment_Justified" => "TextAlignment_Center",
                "TextAlignment_Center" => "ParagraphAlignment_Top",
                "ParagraphAlignment_Top" => "ParagraphAlignment_Bottom",
                "ParagraphAlignment_Bottom" => "ParagraphAlignment_Center",
                "ParagraphAlignment_Center" => "TextAlignment_Center_ParagraphAlignment_Center",
                "TextAlignment_Center_ParagraphAlignment_Center" => "Clipping",
                "Clipping" => "Preview",
                _ => throw new ArgumentOutOfRangeException(nameof(entity.Name))
            };
        }

        // Update text renderer component of entity so it represents correct text rendering feature based on entity name.
        private static void UpdateTextRenderer(Entity entity)
        {
            // Get TextRendererComponent from entity.
            var textRenderer = entity.GetComponent<TextRendererComponent>();

            // Based on entity name update TextRendererComponent.
            switch (entity.Name)
            {
                case "Preview":
                {
                    textRenderer.FontFamilyName = "Consolas";
                    textRenderer.FontSize = FontSize.FromDips(20);
                    textRenderer.Color = Color.White;
                    textRenderer.TextAlignment = TextAlignment.Leading;
                    textRenderer.ParagraphAlignment = ParagraphAlignment.Near;
                    textRenderer.ClipToLayoutBox = false;
                    textRenderer.Text = "Preview of text rendering feature.";
                    break;
                }
                case "Font1":
                {
                    textRenderer.FontFamilyName = "Calibri";
                    textRenderer.FontSize = FontSize.FromDips(20);
                    textRenderer.Color = Color.White;
                    textRenderer.TextAlignment = TextAlignment.Leading;
                    textRenderer.ParagraphAlignment = ParagraphAlignment.Near;
                    textRenderer.ClipToLayoutBox = false;
                    textRenderer.Text = "This text uses Calibri font.";
                    break;
                }
                case "Font2":
                {
                    textRenderer.FontFamilyName = "Comic Sans MS";
                    textRenderer.FontSize = FontSize.FromDips(20);
                    textRenderer.Color = Color.White;
                    textRenderer.TextAlignment = TextAlignment.Leading;
                    textRenderer.ParagraphAlignment = ParagraphAlignment.Near;
                    textRenderer.ClipToLayoutBox = false;
                    textRenderer.Text = "This text uses Comic Sans MS font.";
                    break;
                }
                case "FontSize1":
                {
                    textRenderer.FontFamilyName = "Calibri";
                    textRenderer.FontSize = FontSize.FromDips(15);
                    textRenderer.Color = Color.White;
                    textRenderer.TextAlignment = TextAlignment.Leading;
                    textRenderer.ParagraphAlignment = ParagraphAlignment.Near;
                    textRenderer.ClipToLayoutBox = false;
                    textRenderer.Text = "This text uses font size of 15 pixels.";
                    break;
                }
                case "FontSize2":
                {
                    textRenderer.FontFamilyName = "Calibri";
                    textRenderer.FontSize = FontSize.FromDips(35);
                    textRenderer.Color = Color.White;
                    textRenderer.TextAlignment = TextAlignment.Leading;
                    textRenderer.ParagraphAlignment = ParagraphAlignment.Near;
                    textRenderer.ClipToLayoutBox = false;
                    textRenderer.Text = "This text uses font size of 35 pixels.";
                    break;
                }
                case "Wrapping":
                {
                    textRenderer.FontFamilyName = "Calibri";
                    textRenderer.FontSize = FontSize.FromDips(20);
                    textRenderer.Color = Color.White;
                    textRenderer.TextAlignment = TextAlignment.Leading;
                    textRenderer.ParagraphAlignment = ParagraphAlignment.Near;
                    textRenderer.ClipToLayoutBox = false;
                    textRenderer.Text =
                        "Long texts are automatically wrapped to fit them into specified layout box. Therefore it is quite easy to present variety of text contents in different scenarios.";
                    break;
                }
                case "Color1":
                {
                    textRenderer.FontFamilyName = "Calibri";
                    textRenderer.FontSize = FontSize.FromDips(20);
                    textRenderer.Color = Color.Red;
                    textRenderer.TextAlignment = TextAlignment.Leading;
                    textRenderer.ParagraphAlignment = ParagraphAlignment.Near;
                    textRenderer.ClipToLayoutBox = false;
                    textRenderer.Text = "This text is red.";
                    break;
                }
                case "Color2":
                {
                    textRenderer.FontFamilyName = "Calibri";
                    textRenderer.FontSize = FontSize.FromDips(20);
                    textRenderer.Color = Color.Blue;
                    textRenderer.TextAlignment = TextAlignment.Leading;
                    textRenderer.ParagraphAlignment = ParagraphAlignment.Near;
                    textRenderer.ClipToLayoutBox = false;
                    textRenderer.Text = "This text is blue.";
                    break;
                }
                case "TextAlignment_Left":
                {
                    textRenderer.FontFamilyName = "Calibri";
                    textRenderer.FontSize = FontSize.FromDips(20);
                    textRenderer.Color = Color.White;
                    textRenderer.TextAlignment = TextAlignment.Leading;
                    textRenderer.ParagraphAlignment = ParagraphAlignment.Near;
                    textRenderer.ClipToLayoutBox = false;
                    textRenderer.Text = @"This text is aligned to the left.

Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla malesuada pharetra mattis. Donec finibus id mi sed congue. Aenean scelerisque a nulla et sollicitudin. Quisque vitae neque laoreet, dapibus mi sed, vulputate ipsum.";
                    break;
                }
                case "TextAlignment_Right":
                {
                    textRenderer.FontFamilyName = "Calibri";
                    textRenderer.FontSize = FontSize.FromDips(20);
                    textRenderer.Color = Color.White;
                    textRenderer.TextAlignment = TextAlignment.Trailing;
                    textRenderer.ParagraphAlignment = ParagraphAlignment.Near;
                    textRenderer.ClipToLayoutBox = false;
                    textRenderer.Text = @"This text is aligned to the right.

Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla malesuada pharetra mattis. Donec finibus id mi sed congue. Aenean scelerisque a nulla et sollicitudin. Quisque vitae neque laoreet, dapibus mi sed, vulputate ipsum.";
                    break;
                }
                case "TextAlignment_Justified":
                {
                    textRenderer.FontFamilyName = "Calibri";
                    textRenderer.FontSize = FontSize.FromDips(20);
                    textRenderer.Color = Color.White;
                    textRenderer.TextAlignment = TextAlignment.Justified;
                    textRenderer.ParagraphAlignment = ParagraphAlignment.Near;
                    textRenderer.ClipToLayoutBox = false;
                    textRenderer.Text = @"This text is justified.

Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla malesuada pharetra mattis. Donec finibus id mi sed congue. Aenean scelerisque a nulla et sollicitudin. Quisque vitae neque laoreet, dapibus mi sed, vulputate ipsum.";
                    break;
                }
                case "TextAlignment_Center":
                {
                    textRenderer.FontFamilyName = "Calibri";
                    textRenderer.FontSize = FontSize.FromDips(20);
                    textRenderer.Color = Color.White;
                    textRenderer.TextAlignment = TextAlignment.Center;
                    textRenderer.ParagraphAlignment = ParagraphAlignment.Near;
                    textRenderer.ClipToLayoutBox = false;
                    textRenderer.Text = @"This text is centered.

Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla malesuada pharetra mattis. Donec finibus id mi sed congue. Aenean scelerisque a nulla et sollicitudin. Quisque vitae neque laoreet, dapibus mi sed, vulputate ipsum.";
                    break;
                }
                case "ParagraphAlignment_Top":
                {
                    textRenderer.FontFamilyName = "Calibri";
                    textRenderer.FontSize = FontSize.FromDips(20);
                    textRenderer.Color = Color.White;
                    textRenderer.TextAlignment = TextAlignment.Leading;
                    textRenderer.ParagraphAlignment = ParagraphAlignment.Near;
                    textRenderer.ClipToLayoutBox = false;
                    textRenderer.Text = @"This paragraph is aligned to top.

Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla malesuada pharetra mattis. Donec finibus id mi sed congue. Aenean scelerisque a nulla et sollicitudin. Quisque vitae neque laoreet, dapibus mi sed, vulputate ipsum.";
                    break;
                }
                case "ParagraphAlignment_Bottom":
                {
                    textRenderer.FontFamilyName = "Calibri";
                    textRenderer.FontSize = FontSize.FromDips(20);
                    textRenderer.Color = Color.White;
                    textRenderer.TextAlignment = TextAlignment.Leading;
                    textRenderer.ParagraphAlignment = ParagraphAlignment.Far;
                    textRenderer.ClipToLayoutBox = false;
                    textRenderer.Text = @"This paragraph is aligned to bottom.

Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla malesuada pharetra mattis. Donec finibus id mi sed congue. Aenean scelerisque a nulla et sollicitudin. Quisque vitae neque laoreet, dapibus mi sed, vulputate ipsum.";
                    break;
                }
                case "ParagraphAlignment_Center":
                {
                    textRenderer.FontFamilyName = "Calibri";
                    textRenderer.FontSize = FontSize.FromDips(20);
                    textRenderer.Color = Color.White;
                    textRenderer.TextAlignment = TextAlignment.Leading;
                    textRenderer.ParagraphAlignment = ParagraphAlignment.Center;
                    textRenderer.ClipToLayoutBox = false;
                    textRenderer.Text = @"This paragraph is centered.

Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla malesuada pharetra mattis. Donec finibus id mi sed congue. Aenean scelerisque a nulla et sollicitudin. Quisque vitae neque laoreet, dapibus mi sed, vulputate ipsum.";
                    break;
                }
                case "TextAlignment_Center_ParagraphAlignment_Center":
                {
                    textRenderer.FontFamilyName = "Calibri";
                    textRenderer.FontSize = FontSize.FromDips(20);
                    textRenderer.Color = Color.White;
                    textRenderer.TextAlignment = TextAlignment.Center;
                    textRenderer.ParagraphAlignment = ParagraphAlignment.Center;
                    textRenderer.ClipToLayoutBox = false;
                    textRenderer.Text = @"This text is centered
horizontally and vertically.";
                    break;
                }
                case "Clipping":
                {
                    textRenderer.FontFamilyName = "Calibri";
                    textRenderer.FontSize = FontSize.FromDips(30);
                    textRenderer.Color = Color.White;
                    textRenderer.TextAlignment = TextAlignment.Leading;
                    textRenderer.ParagraphAlignment = ParagraphAlignment.Near;
                    textRenderer.ClipToLayoutBox = true;
                    textRenderer.Text = @"If the text is too long for specified layout box you can enable clipping.

Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla malesuada pharetra mattis. Donec finibus id mi sed congue. Aenean scelerisque a nulla et sollicitudin. Quisque vitae neque laoreet, dapibus mi sed, vulputate ipsum.";
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(entity.Name));
            }
        }
    }
}