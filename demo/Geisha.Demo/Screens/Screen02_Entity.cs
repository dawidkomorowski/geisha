using Geisha.Demo.Common;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering;

namespace Geisha.Demo.Screens
{
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

                // Create entity representing first line of text.
                var line1 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var line1Transform = line1.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                line1Transform.Translation = new Vector2(-150, 350);
                // Add TextRendererComponent to entity so it can show text on the screen.
                var line1TextRenderer = line1.CreateComponent<TextRendererComponent>();
                // Set text properties.
                line1TextRenderer.Color = Color.FromArgb(255, 0, 0, 0);
                line1TextRenderer.FontSize = FontSize.FromDips(40);
                line1TextRenderer.Text = "This is Entity.";

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
                rectangleRenderer.Color = Color.FromArgb(255, 255, 0, 0);
                rectangleRenderer.Dimension = new Vector2(100, 100);

                // Create entity representing second line of text.
                var line2 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var line2Transform = line2.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                line2Transform.Translation = new Vector2(-650, 50);
                // Add TextRendererComponent to entity so it can show text on the screen.
                var line2TextRenderer = line2.CreateComponent<TextRendererComponent>();
                // Set text properties.
                line2TextRenderer.Color = Color.FromArgb(255, 0, 0, 0);
                line2TextRenderer.FontSize = FontSize.FromDips(40);
                line2TextRenderer.Text = "All game objects in Geisha Engine are represented by entities.";

                // Create entity representing third line of text.
                var line3 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var line3Transform = line3.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                line3Transform.Translation = new Vector2(-650, 0);
                // Add TextRendererComponent to entity so it can show text on the screen.
                var line3TextRenderer = line3.CreateComponent<TextRendererComponent>();
                // Set text properties.
                line3TextRenderer.Color = Color.FromArgb(255, 0, 0, 0);
                line3TextRenderer.FontSize = FontSize.FromDips(40);
                line3TextRenderer.Text = "Entity by itself is not doing much. It is merely container for";

                // Create entity representing fourth line of text.
                var line4 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var line4Transform = line4.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                line4Transform.Translation = new Vector2(-650, -50);
                // Add TextRendererComponent to entity so it can show text on the screen.
                var line4TextRenderer = line4.CreateComponent<TextRendererComponent>();
                // Set text properties.
                line4TextRenderer.Color = Color.FromArgb(255, 0, 0, 0);
                line4TextRenderer.FontSize = FontSize.FromDips(40);
                line4TextRenderer.Text = "components. This entity is using two components.";

                // Create entity representing fifth line of text.
                var line5 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var line5Transform = line5.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                line5Transform.Translation = new Vector2(-650, -150);
                // Add TextRendererComponent to entity so it can show text on the screen.
                var line5TextRenderer = line5.CreateComponent<TextRendererComponent>();
                // Set text properties.
                line5TextRenderer.Color = Color.FromArgb(255, 0, 0, 0);
                line5TextRenderer.FontSize = FontSize.FromDips(40);
                line5TextRenderer.Text = "Transform2DComponent allows to position it in the scene.";

                // Create entity representing sixth line of text.
                var line6 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var line6Transform = line6.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                line6Transform.Translation = new Vector2(-650, -200);
                // Add TextRendererComponent to entity so it can show text on the screen.
                var line6TextRenderer = line6.CreateComponent<TextRendererComponent>();
                // Set text properties.
                line6TextRenderer.Color = Color.FromArgb(255, 0, 0, 0);
                line6TextRenderer.FontSize = FontSize.FromDips(40);
                line6TextRenderer.Text = "RectangleRendererComponent allows it to look like red square.";

                // Create entity representing seventh line of text.
                var line7 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var line7Transform = line7.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                line7Transform.Translation = new Vector2(-750, -300);
                // Add TextRendererComponent to entity so it can show text on the screen.
                var line7TextRenderer = line7.CreateComponent<TextRendererComponent>();
                // Set text properties.
                line7TextRenderer.Color = Color.FromArgb(255, 0, 0, 0);
                line7TextRenderer.FontSize = FontSize.FromDips(40);
                line7TextRenderer.Text = "Press [ENTER] to go to the next screen. Press [BACKSPACE] to go back.";
            }
        }
    }
}