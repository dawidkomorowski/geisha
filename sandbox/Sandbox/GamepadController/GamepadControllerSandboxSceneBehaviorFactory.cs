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

            var labelsGroup = Scene.CreateEntity();
            var labelsTransform = labelsGroup.CreateComponent<Transform2DComponent>();
            labelsTransform.Translation = new Vector2(-300, 400);

            CreateText(Scene, new Vector2(0, 0), "Controller State", 30).Parent = labelsGroup;
            var connectionStateLabelEntity = CreateText(Scene, new Vector2(350, 0), "Disconnected", 30);
            connectionStateLabelEntity.Parent = labelsGroup;

            CreateText(Scene, new Vector2(0, -30), "Packet Number", 30).Parent = labelsGroup;
            var packetNumberLabelEntity = CreateText(Scene, new Vector2(350, -30), "9999", 30);
            packetNumberLabelEntity.Parent = labelsGroup;

            var controllerStateEntity = Scene.CreateEntity();
            var controllerStateComponent = controllerStateEntity.CreateComponent<ControllerStateComponent>();
            controllerStateComponent.ConnectionStateLabel = connectionStateLabelEntity.GetComponent<TextRendererComponent>();
            controllerStateComponent.PacketNumberLabel = packetNumberLabelEntity.GetComponent<TextRendererComponent>();
        }

        private static Entity CreateText(Scene scene, Vector2 position, string text, int fontSize)
        {
            var entity = scene.CreateEntity();
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
            return entity;
        }
    }
}