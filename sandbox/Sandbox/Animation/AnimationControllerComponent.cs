using System.Collections.Immutable;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Rendering.Components;

namespace Sandbox.Animation;

internal sealed class AnimationControllerComponent : BehaviorComponent
{
    private const double TrackHalfWidth = 400;

    private readonly ITimeSystem _timeSystem;
    private SpriteAnimationComponent _normalAnimation = null!;
    private Transform2DComponent _normalIndicatorTransform = null!;
    private SpriteAnimationComponent _realTimeAnimation = null!;
    private Transform2DComponent _realTimeIndicatorTransform = null!;
    private TextRendererComponent _statusText = null!;

    public AnimationControllerComponent(Entity entity, ITimeSystem timeSystem) : base(entity)
    {
        _timeSystem = timeSystem;
    }

    public SpriteAnimationComponent NormalAnimation
    {
        set => _normalAnimation = value;
    }

    public Transform2DComponent NormalIndicatorTransform
    {
        set => _normalIndicatorTransform = value;
    }

    public SpriteAnimationComponent RealTimeAnimation
    {
        set => _realTimeAnimation = value;
    }

    public Transform2DComponent RealTimeIndicatorTransform
    {
        set => _realTimeIndicatorTransform = value;
    }

    public TextRendererComponent StatusText
    {
        set => _statusText = value;
    }

    public override void OnStart()
    {
        var inputComponent = Entity.CreateComponent<InputComponent>();
        inputComponent.InputMapping = new InputMapping
        {
            ActionMappings = ImmutableArray.Create(
                new ActionMapping
                {
                    ActionName = "TogglePause",
                    HardwareActions = ImmutableArray.Create(
                        new HardwareAction
                        {
                            HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Space)
                        }
                    )
                }
            )
        };
        inputComponent.BindAction("TogglePause", TogglePause);
    }

    public override void OnUpdate(in TimeStep timeStep)
    {
        _normalIndicatorTransform.Translation = new Vector2(
            -TrackHalfWidth + _normalAnimation.Position * 2 * TrackHalfWidth,
            _normalIndicatorTransform.Translation.Y
        );

        _realTimeIndicatorTransform.Translation = new Vector2(
            -TrackHalfWidth + _realTimeAnimation.Position * 2 * TrackHalfWidth,
            _realTimeIndicatorTransform.Translation.Y
        );

        _statusText.Text =
            $"TimeScale: {_timeSystem.TimeScale:F1}\n\n" +
            $"Normal (IgnoreTimeScale=false):\n" +
            $"  Position: {_normalAnimation.Position:F3}    IsPlaying: {_normalAnimation.IsPlaying}\n\n" +
            $"Real-time (IgnoreTimeScale=true):\n" +
            $"  Position: {_realTimeAnimation.Position:F3}    IsPlaying: {_realTimeAnimation.IsPlaying}";
    }

    private void TogglePause()
    {
        _timeSystem.TimeScale = _timeSystem.TimeScale == 0.0 ? 1.0 : 0.0;
    }
}

internal sealed class AnimationControllerComponentFactory : ComponentFactory<AnimationControllerComponent>
{
    private readonly ITimeSystem _timeSystem;

    public AnimationControllerComponentFactory(ITimeSystem timeSystem)
    {
        _timeSystem = timeSystem;
    }

    protected override AnimationControllerComponent CreateComponent(Entity entity) =>
        new AnimationControllerComponent(entity, _timeSystem);
}
