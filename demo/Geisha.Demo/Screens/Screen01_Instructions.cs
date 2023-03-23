using Geisha.Demo.Common;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering;

namespace Geisha.Demo.Screens
{
    internal sealed class InstructionsSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "Screen01_Instructions";
        private readonly CommonScreenFactory _commonScreenFactory;

        public InstructionsSceneBehaviorFactory(CommonScreenFactory commonScreenFactory)
        {
            _commonScreenFactory = commonScreenFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new InstructionsSceneBehavior(scene, _commonScreenFactory);

        private sealed class InstructionsSceneBehavior : SceneBehavior
        {
            private readonly CommonScreenFactory _commonScreenFactory;

            public InstructionsSceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory) : base(scene)
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
                line1Transform.Translation = new Vector2(-750, 150);
                // Add TextRendererComponent to entity so it can show text on the screen.
                var line1TextRenderer = line1.CreateComponent<TextRendererComponent>();
                // Set text properties.
                line1TextRenderer.Color = Color.FromArgb(255, 0, 0, 0);
                line1TextRenderer.FontSize = FontSize.FromDips(40);
                line1TextRenderer.Text = "This demo is designed to present engine features in very simple way.";

                // Create entity representing second line of text.
                var line2 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var line2Transform = line2.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                line2Transform.Translation = new Vector2(-550, 50);
                // Add TextRendererComponent to entity so it can show text on the screen.
                var line2TextRenderer = line2.CreateComponent<TextRendererComponent>();
                // Set text properties.
                line2TextRenderer.Color = Color.FromArgb(255, 0, 0, 0);
                line2TextRenderer.FontSize = FontSize.FromDips(40);
                line2TextRenderer.Text = "Each example is self-contained within a single file";

                // Create entity representing third line of text.
                var line3 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var line3Transform = line3.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                line3Transform.Translation = new Vector2(-550, 0);
                // Add TextRendererComponent to entity so it can show text on the screen.
                var line3TextRenderer = line3.CreateComponent<TextRendererComponent>();
                // Set text properties.
                line3TextRenderer.Color = Color.FromArgb(255, 0, 0, 0);
                line3TextRenderer.FontSize = FontSize.FromDips(40);
                line3TextRenderer.Text = "linked at the bottom left. Press [F1] to go to URL.";

                // Create entity representing fourth line of text.
                var line4 = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                var line4Transform = line4.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                line4Transform.Translation = new Vector2(-750, -100);
                // Add TextRendererComponent to entity so it can show text on the screen.
                var line4TextRenderer = line4.CreateComponent<TextRendererComponent>();
                // Set text properties.
                line4TextRenderer.Color = Color.FromArgb(255, 0, 0, 0);
                line4TextRenderer.FontSize = FontSize.FromDips(40);
                line4TextRenderer.Text = "Press [ENTER] to go to the next screen. Press [BACKSPACE] to go back.";
            }
        }
    }
}