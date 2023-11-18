using Geisha.Demo.Common;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Demo.Screens;

internal sealed class RenderingSortingLayersSceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "Screen07_RenderingSortingLayers";
    private readonly CommonScreenFactory _commonScreenFactory;

    public RenderingSortingLayersSceneBehaviorFactory(CommonScreenFactory commonScreenFactory)
    {
        _commonScreenFactory = commonScreenFactory;
    }

    public string BehaviorName => SceneBehaviorName;
    public SceneBehavior Create(Scene scene) => new RenderingSortingLayersSceneBehavior(scene, _commonScreenFactory);

    private sealed class RenderingSortingLayersSceneBehavior : SceneBehavior
    {
        private readonly CommonScreenFactory _commonScreenFactory;

        public RenderingSortingLayersSceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory) : base(scene)
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
            textRenderer1.Text = "You can use named layers to control order of rendering.";

            // Create entity representing red square.
            var entity1 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var entity1Transform = entity1.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            entity1Transform.Translation = new Vector2(-100, 50);
            // Add RectangleRendererComponent to entity so it can show red square on the screen.
            var rectangleRenderer1 = entity1.CreateComponent<RectangleRendererComponent>();
            // Set rectangle properties.
            rectangleRenderer1.Dimensions = new Vector2(200, 200);
            rectangleRenderer1.Color = Color.Red;
            rectangleRenderer1.FillInterior = true;
            // Set sorting layer for entity so it follows rendering order defined by Rendering.SortingLayersOrder in file "engine-config.json".
            rectangleRenderer1.SortingLayerName = "Background";

            // Create entity representing label.
            var label1 = Scene.CreateEntity();
            var label1Transform = label1.CreateComponent<Transform2DComponent>();
            label1Transform.Translation = new Vector2(-100, 50);
            var label1Text = label1.CreateComponent<TextRendererComponent>();
            label1Text.Color = Color.Black;
            label1Text.FontSize = FontSize.FromDips(20);
            label1Text.TextAlignment = TextAlignment.Center;
            label1Text.MaxWidth = 180;
            label1Text.MaxHeight = 180;
            label1Text.Pivot = new Vector2(90, 90);
            label1Text.Text = "Background";

            // Create entity representing green square.
            var entity2 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var entity2Transform = entity2.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            entity2Transform.Translation = new Vector2(0, -50);
            // Add RectangleRendererComponent to entity so it can show green square on the screen.
            var rectangleRenderer2 = entity2.CreateComponent<RectangleRendererComponent>();
            // Set rectangle properties.
            rectangleRenderer2.Dimensions = new Vector2(200, 200);
            rectangleRenderer2.Color = Color.Green;
            rectangleRenderer2.FillInterior = true;
            // Set sorting layer for entity so it follows rendering order defined by Rendering.SortingLayersOrder in file "engine-config.json".
            rectangleRenderer2.SortingLayerName = "Middleground";

            // Create entity representing label.
            var label2 = Scene.CreateEntity();
            var label2Transform = label2.CreateComponent<Transform2DComponent>();
            label2Transform.Translation = new Vector2(0, -50);
            var label2Text = label2.CreateComponent<TextRendererComponent>();
            label2Text.Color = Color.Black;
            label2Text.FontSize = FontSize.FromDips(20);
            label2Text.TextAlignment = TextAlignment.Center;
            label2Text.MaxWidth = 180;
            label2Text.MaxHeight = 180;
            label2Text.Pivot = new Vector2(90, 90);
            label2Text.Text = "Middleground";

            // Create entity representing blue square.
            var entity3 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var entity3Transform = entity3.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            entity3Transform.Translation = new Vector2(100, -150);
            // Add RectangleRendererComponent to entity so it can show blue square on the screen.
            var rectangleRenderer3 = entity3.CreateComponent<RectangleRendererComponent>();
            // Set rectangle properties.
            rectangleRenderer3.Dimensions = new Vector2(200, 200);
            rectangleRenderer3.Color = Color.Blue;
            rectangleRenderer3.FillInterior = true;
            // Set sorting layer for entity so it follows rendering order defined by Rendering.SortingLayersOrder in file "engine-config.json".
            rectangleRenderer3.SortingLayerName = "Foreground";

            // Create entity representing label.
            var label3 = Scene.CreateEntity();
            var label3Transform = label3.CreateComponent<Transform2DComponent>();
            label3Transform.Translation = new Vector2(100, -150);
            var label3Text = label3.CreateComponent<TextRendererComponent>();
            label3Text.Color = Color.Black;
            label3Text.FontSize = FontSize.FromDips(20);
            label3Text.TextAlignment = TextAlignment.Center;
            label3Text.MaxWidth = 180;
            label3Text.MaxHeight = 180;
            label3Text.Pivot = new Vector2(90, 90);
            label3Text.Text = "Foreground";

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