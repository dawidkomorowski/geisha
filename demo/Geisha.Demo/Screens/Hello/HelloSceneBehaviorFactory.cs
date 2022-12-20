using Geisha.Demo.Common;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Demo.Screens.Hello
{
    internal sealed class HelloSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "Hello";
        private readonly CommonScreenFactory _commonScreenFactory;

        public HelloSceneBehaviorFactory(CommonScreenFactory commonScreenFactory)
        {
            _commonScreenFactory = commonScreenFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new HelloSceneBehavior(scene, _commonScreenFactory);

        private sealed class HelloSceneBehavior : SceneBehavior
        {
            private readonly CommonScreenFactory _commonScreenFactory;

            public HelloSceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory) : base(scene)
            {
                _commonScreenFactory = commonScreenFactory;
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                // Create background and menu.
                _commonScreenFactory.CreateCommonScreen(Scene,
                    "https://github.com/dawidkomorowski/geisha/blob/master/demo/Geisha.Demo/Screens/Hello/HelloSceneBehaviorFactory.cs");

                // Create entity representing first line of text.
                var line1 = Scene.CreateEntity();
                // Add Transform2DComponent to entity.
                var line1Transform = line1.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                line1Transform.Translation = new Vector2(-250, 150);
                // Add TextRendererComponent to entity.
                var line1TextRenderer = line1.CreateComponent<TextRendererComponent>();
                // Set text properties.
                line1TextRenderer.Color = Color.FromArgb(255, 0, 0, 0);
                line1TextRenderer.FontSize = FontSize.FromDips(40);
                line1TextRenderer.Text = "Welcome to Geisha Engine.";

                // Create entity representing second line of text.
                var line2 = Scene.CreateEntity();
                // Add Transform2DComponent to entity.
                var line2Transform = line2.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                line2Transform.Translation = new Vector2(-750, 100);
                // Add TextRendererComponent to entity.
                var line2TextRenderer = line2.CreateComponent<TextRendererComponent>();
                // Set text properties.
                line2TextRenderer.Color = Color.FromArgb(255, 0, 0, 0);
                line2TextRenderer.FontSize = FontSize.FromDips(40);
                line2TextRenderer.Text = "This is a quick demo of engine features and what can be done with it.";

                // Create entity representing third line of text.
                var line3 = Scene.CreateEntity();
                // Add Transform2DComponent to entity.
                var line3Transform = line3.CreateComponent<Transform2DComponent>();
                // Set position of the entity.
                line3Transform.Translation = new Vector2(-400, 0);
                // Add TextRendererComponent to entity.
                var line3TextRenderer = line3.CreateComponent<TextRendererComponent>();
                // Set text properties.
                line3TextRenderer.Color = Color.FromArgb(255, 0, 0, 0);
                line3TextRenderer.FontSize = FontSize.FromDips(40);
                line3TextRenderer.Text = "Press [ENTER] to go to the next screen.";
            }
        }
    }
}