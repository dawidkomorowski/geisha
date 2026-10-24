using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using Sandbox.Common;

namespace Sandbox.CustomFont;

public sealed class CustomFontSandboxSceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "CustomFontSandbox";

    private readonly CommonEntityFactory _commonEntityFactory;

    public CustomFontSandboxSceneBehaviorFactory(CommonEntityFactory commonEntityFactory)
    {
        _commonEntityFactory = commonEntityFactory;
    }


    public string BehaviorName => SceneBehaviorName;
    public SceneBehavior Create(Scene scene) => new CustomFontSandboxSceneBehavior(scene, _commonEntityFactory);

    private sealed class CustomFontSandboxSceneBehavior : SceneBehavior
    {
        private readonly CommonEntityFactory _commonEntityFactory;

        public CustomFontSandboxSceneBehavior(Scene scene, CommonEntityFactory commonEntityFactory) : base(scene)
        {
            _commonEntityFactory = commonEntityFactory;
        }

        public override string Name => SceneBehaviorName;

        protected override void OnLoaded()
        {
            _commonEntityFactory.CreateCamera(Scene);
            _commonEntityFactory.CreateBasicControls(Scene);

            CreateText(Scene, new Vector2(0, 105), "Font: Consolas", 60, "Consolas");
            CreateText(Scene, new Vector2(0, 35), "Font: Algerian", 60, "Algerian");
            CreateText(Scene, new Vector2(0, -35), "Font: Kenney Pixel", 60, "Kenney Pixel");
            CreateText(Scene, new Vector2(0, -105), "Font: Kenney Blocks", 60, "Kenney Blocks");
        }

        private static Entity CreateText(Scene scene, Vector2 position, string text, int fontSize, string fontFamilyName)
        {
            var entity = scene.CreateEntity();
            var transform = entity.CreateComponent<Transform2DComponent>();
            transform.Translation = position;
            var textRenderer = entity.CreateComponent<TextRendererComponent>();
            textRenderer.Color = Color.Black;
            textRenderer.FontSize = FontSize.FromDips(fontSize);
            textRenderer.MaxWidth = 1000;
            textRenderer.MaxHeight = 1000;
            textRenderer.Pivot = new Vector2(500, 500);
            textRenderer.TextAlignment = TextAlignment.Center;
            textRenderer.ParagraphAlignment = ParagraphAlignment.Center;
            textRenderer.Text = text;
            textRenderer.FontFamilyName = fontFamilyName;
            return entity;
        }
    }
}