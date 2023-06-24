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

                // Create entity representing text block.
                var textBlock = Scene.CreateEntity();
                // Add Transform2DComponent to entity so we can control its position.
                textBlock.CreateComponent<Transform2DComponent>();
                // Add TextRendererComponent to entity so it can show text on the screen.
                var textRenderer = textBlock.CreateComponent<TextRendererComponent>();
                // Set text properties.
                textRenderer.Color = Color.Black;
                textRenderer.FontSize = FontSize.FromDips(40);
                textRenderer.TextAlignment = TextAlignment.Center;
                textRenderer.ParagraphAlignment = ParagraphAlignment.Center;
                textRenderer.MaxWidth = 1600;
                textRenderer.MaxHeight = 900;
                textRenderer.Pivot = new Vector2(800, 450);
                textRenderer.Text = @"This demo is designed to present engine features in very simple way.

Each example is self-contained within a single file
linked at the bottom left. Press [F1] to go to URL.

Press [ENTER] to go to the next screen. Press [BACKSPACE] to go back.";
            }
        }
    }
}