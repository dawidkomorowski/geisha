using Geisha.Engine.Core.Assets;
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
    private readonly IAssetStore _assetStore;

    public GamepadControllerSandboxSceneBehaviorFactory(CommonEntityFactory commonEntityFactory, IAssetStore assetStore)
    {
        _commonEntityFactory = commonEntityFactory;
        _assetStore = assetStore;
    }

    public string BehaviorName => SceneBehaviorName;

    public SceneBehavior Create(Scene scene) => new GamepadControllerSandboxSceneBehavior(scene, _commonEntityFactory, _assetStore);

    private sealed class GamepadControllerSandboxSceneBehavior : SceneBehavior
    {
        private readonly CommonEntityFactory _commonEntityFactory;
        private readonly IAssetStore _assetStore;

        public GamepadControllerSandboxSceneBehavior(Scene scene, CommonEntityFactory commonEntityFactory, IAssetStore assetStore) : base(scene)
        {
            _commonEntityFactory = commonEntityFactory;
            _assetStore = assetStore;
        }

        public override string Name => SceneBehaviorName;

        protected override void OnLoaded()
        {
            _commonEntityFactory.CreateCamera(Scene);
            _commonEntityFactory.CreateBasicControls(Scene);

            var background = Scene.CreateEntity();
            background.CreateComponent<Transform2DComponent>();
            var backgroundRenderer = background.CreateComponent<RectangleRendererComponent>();
            backgroundRenderer.Dimensions = new Vector2(2000, 2000);
            backgroundRenderer.Color = Color.FromArgb(255, 64, 64, 64);
            backgroundRenderer.FillInterior = true;
            backgroundRenderer.OrderInLayer = -100;

            // Text labels
            var labelGroup = Scene.CreateEntity();
            var labelsTransform = labelGroup.CreateComponent<Transform2DComponent>();
            labelsTransform.Translation = new Vector2(-300, 400);

            CreateText(labelGroup, new Vector2(0, 0), "Controller State", 30);
            var connectionStateLabel = CreateText(labelGroup, new Vector2(350, 0), string.Empty, 30);

            CreateText(labelGroup, new Vector2(0, -30), "Packet Number", 30);
            var packetNumberLabel = CreateText(labelGroup, new Vector2(350, -30), string.Empty, 30);

            // Gamepad images
            var imageGroup = Scene.CreateEntity();

            var xboxGuideImage = CreateImage(imageGroup, new Vector2(0, 75), "1d1770d0-9a2f-4be6-b722-9af88e4b17c9");

            var abxyGroup = imageGroup.CreateChildEntity();
            var abxyGroupTransform = abxyGroup.CreateComponent<Transform2DComponent>();
            abxyGroupTransform.Translation = new Vector2(200, 0);

            var buttonAImage = CreateImage(abxyGroup, new Vector2(0, -50));
            var buttonBImage = CreateImage(abxyGroup, new Vector2(50, 0));
            var buttonXImage = CreateImage(abxyGroup, new Vector2(-50, 0));
            var buttonYImage = CreateImage(abxyGroup, new Vector2(0, 50));

            var dPadGroup = imageGroup.CreateChildEntity();
            var dPadGroupTransform = dPadGroup.CreateComponent<Transform2DComponent>();
            dPadGroupTransform.Translation = new Vector2(-100, -100);

            var dPadBackgroundImage = CreateImage(dPadGroup, "cf503821-093f-4524-9b6f-ff0146baecb0");
            dPadBackgroundImage.OrderInLayer = -10;

            var dPadUpImage = CreateImage(dPadGroup, "4db3b33c-0c9e-49f6-8005-b57b737d2518");
            var dPadDownImage = CreateImage(dPadGroup, "5a017db9-c0fa-4e0d-b62e-699f064d8b29");
            var dPadLeftImage = CreateImage(dPadGroup, "dac2b91d-4835-43bd-8bc8-47c55ccabcaf");
            var dPadRightImage = CreateImage(dPadGroup, "d581d762-caaf-4358-8659-a400722ed196");

            var startBackGroup = imageGroup.CreateChildEntity();
            var startBackGroupTransform = startBackGroup.CreateComponent<Transform2DComponent>();
            startBackGroupTransform.Translation = new Vector2();

            var startImage = CreateImage(startBackGroup, new Vector2(50, 0));
            var backImage = CreateImage(startBackGroup, new Vector2(-50, 0));

            var leftStickRoot = imageGroup.CreateChildEntity();
            var leftStickRootTransform = leftStickRoot.CreateComponent<Transform2DComponent>();
            leftStickRootTransform.Translation = new Vector2(-200, 0);

            var leftStickLimit = leftStickRoot.CreateChildEntity();
            leftStickLimit.CreateComponent<Transform2DComponent>();
            var leftStickLimitRenderer = leftStickLimit.CreateComponent<EllipseRendererComponent>();
            leftStickLimitRenderer.Color = Color.White;
            leftStickLimitRenderer.Radius = 50;

            var leftStickImage = CreateImage(leftStickRoot, Vector2.Zero);
            var leftStickTransform = leftStickImage.Entity.GetComponent<Transform2DComponent>();

            var rightStickRoot = imageGroup.CreateChildEntity();
            var rightStickRootTransform = rightStickRoot.CreateComponent<Transform2DComponent>();
            rightStickRootTransform.Translation = new Vector2(100, -100);

            var rightStickLimit = rightStickRoot.CreateChildEntity();
            rightStickLimit.CreateComponent<Transform2DComponent>();
            var rightStickLimitRenderer = rightStickLimit.CreateComponent<EllipseRendererComponent>();
            rightStickLimitRenderer.Color = Color.White;
            rightStickLimitRenderer.Radius = 50;

            var rightStickImage = CreateImage(rightStickRoot, Vector2.Zero);
            var rightStickTransform = rightStickImage.Entity.GetComponent<Transform2DComponent>();

            var leftBumperImage = CreateImage(imageGroup, new Vector2(-200, 125));
            var rightBumperImage = CreateImage(imageGroup, new Vector2(200, 125));

            var leftTriggerGroup = imageGroup.CreateChildEntity();
            var leftTriggerTransform = leftTriggerGroup.CreateComponent<Transform2DComponent>();
            leftTriggerTransform.Translation = new Vector2(-200, 175);
            var leftTriggerHighImage = CreateImage(leftTriggerGroup, "a5376b3d-4fe0-4f58-9e94-9a09e6948627");
            var leftTriggerLowImage = CreateImage(leftTriggerGroup, "c6f870e0-94fb-4135-a606-ad4274eda311");

            var rightTriggerGroup = imageGroup.CreateChildEntity();
            var rightTriggerTransform = rightTriggerGroup.CreateComponent<Transform2DComponent>();
            rightTriggerTransform.Translation = new Vector2(200, 175);
            var rightTriggerHighImage = CreateImage(rightTriggerGroup, "0ca74082-9e42-4a64-8ecd-58d3facc05ed");
            var rightTriggerLowImage = CreateImage(rightTriggerGroup, "cfd9d3e9-2f04-4785-97d8-8835b448c8c3");

            // Behavior component setup
            var controllerStateEntity = Scene.CreateEntity();
            var controllerStateComponent = controllerStateEntity.CreateComponent<ControllerStateComponent>();

            controllerStateComponent.ConnectionStateLabel = connectionStateLabel;
            controllerStateComponent.PacketNumberLabel = packetNumberLabel;

            controllerStateComponent.XboxGuideImage = xboxGuideImage;

            controllerStateComponent.ButonAImage = buttonAImage;
            controllerStateComponent.ButonBImage = buttonBImage;
            controllerStateComponent.ButonXImage = buttonXImage;
            controllerStateComponent.ButonYImage = buttonYImage;

            controllerStateComponent.DPadUpImage = dPadUpImage;
            controllerStateComponent.DPadDownImage = dPadDownImage;
            controllerStateComponent.DPadLeftImage = dPadLeftImage;
            controllerStateComponent.DPadRightImage = dPadRightImage;

            controllerStateComponent.StartImage = startImage;
            controllerStateComponent.BackImage = backImage;

            controllerStateComponent.LeftStrickImage = leftStickImage;
            controllerStateComponent.LeftStickTransform = leftStickTransform;

            controllerStateComponent.RightStickImage = rightStickImage;
            controllerStateComponent.RightStickTransform = rightStickTransform;

            controllerStateComponent.LeftBumperImage = leftBumperImage;
            controllerStateComponent.RightBumperImage = rightBumperImage;

            controllerStateComponent.LeftTriggerHighImage = leftTriggerHighImage;
            controllerStateComponent.LeftTriggerLowImage = leftTriggerLowImage;

            controllerStateComponent.RightTriggerHighImage = rightTriggerHighImage;
            controllerStateComponent.RightTriggerLowImage = rightTriggerLowImage;
        }

        private static TextRendererComponent CreateText(Entity parent, Vector2 position, string text, int fontSize)
        {
            var entity = parent.CreateChildEntity();
            var transform = entity.CreateComponent<Transform2DComponent>();
            transform.Translation = position;
            var textRenderer = entity.CreateComponent<TextRendererComponent>();
            textRenderer.Color = Color.White;
            textRenderer.FontSize = FontSize.FromDips(fontSize);
            textRenderer.MaxWidth = 1000;
            textRenderer.MaxHeight = 1000;
            textRenderer.Pivot = new Vector2(0, 0);
            textRenderer.TextAlignment = TextAlignment.Leading;
            textRenderer.ParagraphAlignment = ParagraphAlignment.Near;
            textRenderer.Text = text;
            return textRenderer;
        }

        private SpriteRendererComponent CreateImage(Entity parent, Vector2 position) => CreateImage(parent, position, null);
        private SpriteRendererComponent CreateImage(Entity parent, string assetId) => CreateImage(parent, Vector2.Zero, assetId);

        private SpriteRendererComponent CreateImage(Entity parent, Vector2 position, string? assetId)
        {
            var entity = parent.CreateChildEntity();
            var transform = entity.CreateComponent<Transform2DComponent>();
            transform.Translation = position;
            var spriteRenderer = entity.CreateComponent<SpriteRendererComponent>();

            if (!string.IsNullOrEmpty(assetId))
            {
                spriteRenderer.Sprite = _assetStore.GetAsset<Sprite>(AssetId.Parse(assetId));
            }

            return spriteRenderer;
        }
    }
}