using System;
using Geisha.Engine.Animation;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using Sandbox.Common;

namespace Sandbox.Animation;

public sealed class AnimationSandboxSceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "AnimationSandbox";

    private readonly CommonEntityFactory _commonEntityFactory;
    private readonly ITimeSystem _timeSystem;

    public AnimationSandboxSceneBehaviorFactory(CommonEntityFactory commonEntityFactory, ITimeSystem timeSystem)
    {
        _commonEntityFactory = commonEntityFactory;
        _timeSystem = timeSystem;
    }

    public string BehaviorName => SceneBehaviorName;
    public SceneBehavior Create(Scene scene) => new AnimationSandboxSceneBehavior(scene, _commonEntityFactory, _timeSystem);

    private sealed class AnimationSandboxSceneBehavior : SceneBehavior
    {
        private const double TrackWidth = 800;
        private const double TrackHeight = 40;
        private const double IndicatorSize = 44;

        private readonly CommonEntityFactory _commonEntityFactory;
        private readonly ITimeSystem _timeSystem;

        public AnimationSandboxSceneBehavior(Scene scene, CommonEntityFactory commonEntityFactory, ITimeSystem timeSystem) : base(scene)
        {
            _commonEntityFactory = commonEntityFactory;
            _timeSystem = timeSystem;
        }

        public override string Name => SceneBehaviorName;

        protected override void OnLoaded()
        {
            _timeSystem.TimeScale = 1.0;

            _commonEntityFactory.CreateCamera(Scene);
            _commonEntityFactory.CreateBasicControls(Scene);

            // Title
            CreateText(Scene, new Vector2(-780, 390),
                "IgnoreTimeScale Sandbox\n[SPACE] Toggle TimeScale 0.0 / 1.0     [ESC] Exit",
                fontSize: 30, maxWidth: 1600);

            // Normal animation (IgnoreTimeScale=false)
            CreateText(Scene, new Vector2(-400, 240), "Normal animation  (IgnoreTimeScale=false)", fontSize: 26, maxWidth: 820);
            var (normalIndicatorTransform, _) = CreateTrack(Scene, trackY: 165,
                trackColor: Color.FromArgb(255, 180, 180, 180),
                indicatorColor: Color.FromArgb(255, 50, 130, 220));

            var normalAnimEntity = Scene.CreateEntity();
            var normalAnimComponent = normalAnimEntity.CreateComponent<SpriteAnimationComponent>();
            normalAnimComponent.AddAnimation("anim", CreateDummyAnimation(TimeSpan.FromSeconds(3)));
            normalAnimComponent.PlayInLoop = true;
            normalAnimComponent.IgnoreTimeScale = false;
            normalAnimComponent.PlayAnimation("anim");

            // Real-time animation (IgnoreTimeScale=true)
            CreateText(Scene, new Vector2(-400, 50), "Real-time animation  (IgnoreTimeScale=true)", fontSize: 26, maxWidth: 820);
            var (realTimeIndicatorTransform, _) = CreateTrack(Scene, trackY: -25,
                trackColor: Color.FromArgb(255, 180, 180, 180),
                indicatorColor: Color.FromArgb(255, 220, 80, 50));

            var realTimeAnimEntity = Scene.CreateEntity();
            var realTimeAnimComponent = realTimeAnimEntity.CreateComponent<SpriteAnimationComponent>();
            realTimeAnimComponent.AddAnimation("anim", CreateDummyAnimation(TimeSpan.FromSeconds(3)));
            realTimeAnimComponent.PlayInLoop = true;
            realTimeAnimComponent.IgnoreTimeScale = true;
            realTimeAnimComponent.PlayAnimation("anim");

            // Status text
            var statusEntity = Scene.CreateEntity();
            var statusTransform = statusEntity.CreateComponent<Transform2DComponent>();
            statusTransform.Translation = new Vector2(-390, -170);
            var statusTextRenderer = statusEntity.CreateComponent<TextRendererComponent>();
            statusTextRenderer.Color = Color.Black;
            statusTextRenderer.FontSize = FontSize.FromDips(26);
            statusTextRenderer.MaxWidth = 800;
            statusTextRenderer.MaxHeight = 300;

            // Controller
            var controllerEntity = Scene.CreateEntity();
            var controller = controllerEntity.CreateComponent<AnimationControllerComponent>();
            controller.NormalAnimation = normalAnimComponent;
            controller.NormalIndicatorTransform = normalIndicatorTransform;
            controller.RealTimeAnimation = realTimeAnimComponent;
            controller.RealTimeIndicatorTransform = realTimeIndicatorTransform;
            controller.StatusText = statusTextRenderer;
        }

        private static (Transform2DComponent indicatorTransform, Entity trackEntity) CreateTrack(
            Scene scene, double trackY, Color trackColor, Color indicatorColor)
        {
            // Track background
            var trackEntity = scene.CreateEntity();
            var trackTransform = trackEntity.CreateComponent<Transform2DComponent>();
            trackTransform.Translation = new Vector2(0, trackY);
            var trackRect = trackEntity.CreateComponent<RectangleRendererComponent>();
            trackRect.Dimensions = new Vector2(TrackWidth, TrackHeight);
            trackRect.Color = trackColor;
            trackRect.FillInterior = true;

            // Indicator that slides along the track
            var indicatorEntity = scene.CreateEntity();
            var indicatorTransform = indicatorEntity.CreateComponent<Transform2DComponent>();
            indicatorTransform.Translation = new Vector2(-TrackWidth / 2, trackY);
            var indicatorRect = indicatorEntity.CreateComponent<RectangleRendererComponent>();
            indicatorRect.Dimensions = new Vector2(IndicatorSize, IndicatorSize);
            indicatorRect.Color = indicatorColor;
            indicatorRect.FillInterior = true;
            indicatorRect.OrderInLayer = 1;

            return (indicatorTransform, trackEntity);
        }

        private static Entity CreateText(Scene scene, Vector2 position, string text, int fontSize, double maxWidth)
        {
            var entity = scene.CreateEntity();
            var transform = entity.CreateComponent<Transform2DComponent>();
            transform.Translation = position;
            var textRenderer = entity.CreateComponent<TextRendererComponent>();
            textRenderer.Color = Color.Black;
            textRenderer.FontSize = FontSize.FromDips(fontSize);
            textRenderer.MaxWidth = maxWidth;
            textRenderer.MaxHeight = 200;
            textRenderer.Text = text;
            return entity;
        }

        private static SpriteAnimation CreateDummyAnimation(TimeSpan duration)
        {
            // Sprite texture is null since no SpriteRendererComponent is attached — only Position tracking is needed.
            var dummySprite = new Sprite(null!, Vector2.Zero, new Vector2(1, 1), Vector2.Zero, 1);
            var frame = new SpriteAnimationFrame(dummySprite, 1.0);
            return new SpriteAnimation(new[] { frame }, duration);
        }
    }
}
