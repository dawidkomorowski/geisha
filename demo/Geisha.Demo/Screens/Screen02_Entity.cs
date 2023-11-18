using Geisha.Demo.Common;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering;

namespace Geisha.Demo.Screens;

internal sealed class EntitySceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "Screen02_Entity";
    private readonly CommonScreenFactory _commonScreenFactory;

    public EntitySceneBehaviorFactory(CommonScreenFactory commonScreenFactory)
    {
        _commonScreenFactory = commonScreenFactory;
    }

    public string BehaviorName => SceneBehaviorName;
    public SceneBehavior Create(Scene scene) => new EntitySceneBehavior(scene, _commonScreenFactory);

    private sealed class EntitySceneBehavior : SceneBehavior
    {
        private readonly CommonScreenFactory _commonScreenFactory;

        public EntitySceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory) : base(scene)
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
            textBlock1Transform.Translation = new Vector2(0, 325);
            // Add TextRendererComponent to entity so it can show text on the screen.
            var textRenderer1 = textBlock1.CreateComponent<TextRendererComponent>();
            // Set text properties.
            textRenderer1.Color = Color.Black;
            textRenderer1.FontSize = FontSize.FromDips(40);
            textRenderer1.TextAlignment = TextAlignment.Center;
            textRenderer1.ParagraphAlignment = ParagraphAlignment.Center;
            textRenderer1.MaxWidth = 500;
            textRenderer1.MaxHeight = 500;
            textRenderer1.Pivot = new Vector2(250, 250);
            textRenderer1.Text = "This is Entity.";

            // Create entity representing red square.
            var entity = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var entityTransform = entity.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            entityTransform.Translation = new Vector2(0, 200);
            // Add RectangleRendererComponent to entity so it can show red square on the screen.
            var rectangleRenderer = entity.CreateComponent<RectangleRendererComponent>();
            // Set rectangle properties.
            rectangleRenderer.FillInterior = true;
            rectangleRenderer.Color = Color.Red;
            rectangleRenderer.Dimensions = new Vector2(100, 100);

            // Create entity representing second text block.
            var textBlock2 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var textBlock2Transform = textBlock2.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            textBlock2Transform.Translation = new Vector2(-700, 50);
            // Add TextRendererComponent to entity so it can show text on the screen.
            var textRenderer2 = textBlock2.CreateComponent<TextRendererComponent>();
            // Set text properties.
            textRenderer2.Color = Color.Black;
            textRenderer2.FontSize = FontSize.FromDips(40);
            textRenderer2.MaxWidth = 1400;
            textRenderer2.MaxHeight = 500;
            textRenderer2.Text = @"All game objects in Geisha Engine are represented by entities. Entity by itself is not doing much. It is merely container for components. This entity is using two components.

Transform2DComponent allows to position it in the scene.
RectangleRendererComponent allows it to look like red square.";

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