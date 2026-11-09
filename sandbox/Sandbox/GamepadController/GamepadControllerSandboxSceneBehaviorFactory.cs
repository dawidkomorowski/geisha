using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using Sandbox.Common;

namespace Sandbox.GamepadController;

internal sealed class GamepadControllerSandboxSceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "GamepadControllerSandbox";

    private readonly CommonEntityFactory _commonEntityFactory;

    public GamepadControllerSandboxSceneBehaviorFactory(CommonEntityFactory commonEntityFactory)
    {
        _commonEntityFactory = commonEntityFactory;
    }

    public string BehaviorName => SceneBehaviorName;

    public SceneBehavior Create(Scene scene) => new GamepadControllerSandboxSceneBehavior(scene, _commonEntityFactory);

    private sealed class GamepadControllerSandboxSceneBehavior : SceneBehavior
    {
        private readonly CommonEntityFactory _commonEntityFactory;

        public GamepadControllerSandboxSceneBehavior(Scene scene, CommonEntityFactory commonEntityFactory) : base(scene)
        {
            _commonEntityFactory = commonEntityFactory;
        }

        public override string Name => SceneBehaviorName;

        protected override void OnLoaded()
        {
            _commonEntityFactory.CreateCamera(Scene);
            _commonEntityFactory.CreateBasicControls(Scene);

            var labelGroup = Scene.CreateEntity();
            var labelsTransform = labelGroup.CreateComponent<Transform2DComponent>();
            labelsTransform.Translation = new Vector2(-300, 400);

            CreateText(labelGroup, new Vector2(0, 0), "Controller State", 30);
            var connectionStateLabel = CreateText(labelGroup, new Vector2(350, 0), string.Empty, 30);

            CreateText(labelGroup, new Vector2(0, -30), "Packet Number", 30);
            var packetNumberLabel = CreateText(labelGroup, new Vector2(350, -30), string.Empty, 30);

            var imageGroup = Scene.CreateEntity();

            var buttonAImage = CreateImage(imageGroup, new Vector2(0, 0));

            var controllerStateEntity = Scene.CreateEntity();
            var controllerStateComponent = controllerStateEntity.CreateComponent<ControllerStateComponent>();

            controllerStateComponent.ConnectionStateLabel = connectionStateLabel;
            controllerStateComponent.PacketNumberLabel = packetNumberLabel;

            controllerStateComponent.ButonAImage = buttonAImage;
        }

        private static TextRendererComponent CreateText(Entity parent, Vector2 position, string text, int fontSize)
        {
            var entity = parent.CreateChildEntity();
            var transform = entity.CreateComponent<Transform2DComponent>();
            transform.Translation = position;
            var textRenderer = entity.CreateComponent<TextRendererComponent>();
            textRenderer.Color = Color.Black;
            textRenderer.FontSize = FontSize.FromDips(fontSize);
            textRenderer.MaxWidth = 1000;
            textRenderer.MaxHeight = 1000;
            textRenderer.Pivot = new Vector2(0, 0);
            textRenderer.TextAlignment = TextAlignment.Leading;
            textRenderer.ParagraphAlignment = ParagraphAlignment.Near;
            textRenderer.Text = text;
            return textRenderer;
        }

        private SpriteRendererComponent CreateImage(Entity parent, Vector2 position)
        {
            var entity = parent.CreateChildEntity();
            var transform = entity.CreateComponent<Transform2DComponent>();
            transform.Translation = position;
            var spriteRenderer = entity.CreateComponent<SpriteRendererComponent>();
            return spriteRenderer;
        }
    }
}