using System.Linq;
using System.Text;
using Geisha.Demo.Common;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Demo.Screens;

internal sealed class MouseInputSceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "Screen13_MouseInput";
    private readonly CommonScreenFactory _commonScreenFactory;

    public MouseInputSceneBehaviorFactory(CommonScreenFactory commonScreenFactory)
    {
        _commonScreenFactory = commonScreenFactory;
    }

    public string BehaviorName => SceneBehaviorName;
    public SceneBehavior Create(Scene scene) => new MouseInputSceneBehavior(scene, _commonScreenFactory);

    private sealed class MouseInputSceneBehavior : SceneBehavior
    {
        private readonly CommonScreenFactory _commonScreenFactory;

        public MouseInputSceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory) : base(scene)
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

            // Create entity showing mouse input.
            var mouseInput = Scene.CreateEntity();
            // Add Transform2DComponent to entity to control its position.
            var mouseInputTransform = mouseInput.CreateComponent<Transform2DComponent>();
            mouseInputTransform.Translation = new Vector2(-650, -50);
            // Add TextRendererComponent to entity so it can show text on the screen.
            var mouseInputText = mouseInput.CreateComponent<TextRendererComponent>();
            // Set text properties.
            mouseInputText.Color = Color.FromArgb(255, 255, 0, 255);
            mouseInputText.FontSize = FontSize.FromDips(40);
            mouseInputText.TextAlignment = TextAlignment.Leading;
            mouseInputText.ParagraphAlignment = ParagraphAlignment.Center;
            mouseInputText.MaxWidth = 1200;
            mouseInputText.MaxHeight = 900;
            mouseInputText.Pivot = new Vector2(0, 450);
            // Add InputComponent to entity so we can handle user input.
            mouseInput.CreateComponent<InputComponent>();
            // Add custom component that updates text based on mouse input.
            mouseInput.CreateComponent<SetTextToMouseInputComponent>();

            // Create entity showing circle at mouse position.
            var circle = Scene.CreateEntity();
            // Add Transform2DComponent to entity to control its position.
            circle.CreateComponent<Transform2DComponent>();
            // Add EllipseRendererComponent to entity to make it look like a red circle.
            var circleRenderer = circle.CreateComponent<EllipseRendererComponent>();
            circleRenderer.Color = Color.Red;
            circleRenderer.FillInterior = true;
            circleRenderer.Radius = 10;
            // Add InputComponent to entity so we can handle user input.
            circle.CreateComponent<InputComponent>();
            // Add custom component that updates entity position based on mouse input.
            circle.CreateComponent<FollowMousePositionComponent>();

            // Create entity representing first text block.
            var textBlock1 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var textBlock1Transform = textBlock1.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            textBlock1Transform.Translation = new Vector2(-650, 300);
            // Add TextRendererComponent to entity so it can show text on the screen.
            var textRenderer1 = textBlock1.CreateComponent<TextRendererComponent>();
            // Set text properties.
            textRenderer1.Color = Color.Black;
            textRenderer1.FontSize = FontSize.FromDips(40);
            textRenderer1.MaxWidth = 1300;
            textRenderer1.MaxHeight = 500;
            textRenderer1.Text = "Geisha Engine provides InputComponent that allows to read mouse input and perform custom game logic based on that.";

            // Create entity representing second text block.
            var textBlock2 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var textBlock2Transform = textBlock2.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            textBlock2Transform.Translation = new Vector2(0, -325);
            // Add TextRendererComponent to entity so it can show text on the screen.
            var textRenderer2 = textBlock2.CreateComponent<TextRendererComponent>();
            // Set text properties.
            textRenderer2.Color = Color.Black;
            textRenderer2.FontSize = FontSize.FromDips(40);
            textRenderer2.TextAlignment = TextAlignment.Center;
            textRenderer2.ParagraphAlignment = ParagraphAlignment.Center;
            textRenderer2.MaxWidth = 1600;
            textRenderer2.MaxHeight = 900;
            textRenderer2.Pivot = new Vector2(800, 450);
            textRenderer2.Text = "Press [ENTER] to go to the next screen. Press [BACKSPACE] to go back.";
        }
    }
}

// This is implementation of custom component based on BehaviorComponent.
// Behavior components are handled by BehaviorSystem and easily allow to get custom code being run.
// This component updates text of TextRendererComponent attached to the same entity to contain information
// about mouse input read from InputComponent attached to the same entity.
internal sealed class SetTextToMouseInputComponent : BehaviorComponent
{
    private InputComponent _inputComponent = null!;
    private TextRendererComponent _textRendererComponent = null!;
    private CameraComponent _cameraComponent = null!;

    public SetTextToMouseInputComponent(Entity entity) : base(entity)
    {
    }

    // We implement OnStart method to initialize component state.
    public override void OnStart()
    {
        // In this case we retrieve needed components from entity.
        _inputComponent = Entity.GetComponent<InputComponent>();
        _textRendererComponent = Entity.GetComponent<TextRendererComponent>();
        // And find CameraComponent across all entities.
        _cameraComponent = Scene.AllEntities.Single(e => e.HasComponent<CameraComponent>()).GetComponent<CameraComponent>();
    }

    // We implement OnUpdate method to run custom logic once per frame.
    public override void OnUpdate(GameTime gameTime)
    {
        var stringBuilder = new StringBuilder();

        // Add information about the state of the mouse.
        stringBuilder.AppendLine($"Left Mouse Button: {_inputComponent.HardwareInput.MouseInput.LeftButton}");
        stringBuilder.AppendLine($"Right Mouse Button: {_inputComponent.HardwareInput.MouseInput.RightButton}");
        stringBuilder.AppendLine($"Middle Mouse Button: {_inputComponent.HardwareInput.MouseInput.MiddleButton}");
        stringBuilder.AppendLine($"Extended 1 Mouse Button: {_inputComponent.HardwareInput.MouseInput.XButton1}");
        stringBuilder.AppendLine($"Extended 2 Mouse Button: {_inputComponent.HardwareInput.MouseInput.XButton2}");
        stringBuilder.AppendLine($"Mouse Position: {_inputComponent.HardwareInput.MouseInput.Position}");
        stringBuilder.AppendLine($"Mouse Position Delta: {_inputComponent.HardwareInput.MouseInput.PositionDelta}");
        stringBuilder.AppendLine(
            $"Mouse Position in World Space: {_cameraComponent.ScreenPointToWorld2DPoint(_inputComponent.HardwareInput.MouseInput.Position)}");
        stringBuilder.AppendLine($"Scroll Delta: {_inputComponent.HardwareInput.MouseInput.ScrollDelta}");

        _textRendererComponent.Text = stringBuilder.ToString();
    }
}

// To make component available to the engine we need to create factory for that component
// and register it in IComponentsRegistry which is done in DemoApp.cs file.
internal sealed class SetTextToMouseInputComponentFactory : ComponentFactory<SetTextToMouseInputComponent>
{
    protected override SetTextToMouseInputComponent CreateComponent(Entity entity) => new(entity);
}

// This is implementation of custom component based on BehaviorComponent.
// Behavior components are handled by BehaviorSystem and easily allow to get custom code being run.
// This component updates translation of Transform2DComponent attached to the same entity to be the same
// as position of mouse read from InputComponent attached to the same entity.
internal sealed class FollowMousePositionComponent : BehaviorComponent
{
    private Transform2DComponent _transform2DComponent = null!;
    private InputComponent _inputComponent = null!;
    private CameraComponent _cameraComponent = null!;

    public FollowMousePositionComponent(Entity entity) : base(entity)
    {
    }

    // We implement OnStart method to initialize component state.
    public override void OnStart()
    {
        // In this case we retrieve needed components from entity.
        _transform2DComponent = Entity.GetComponent<Transform2DComponent>();
        _inputComponent = Entity.GetComponent<InputComponent>();
        // And find CameraComponent across all entities.
        _cameraComponent = Scene.AllEntities.Single(e => e.HasComponent<CameraComponent>()).GetComponent<CameraComponent>();
    }

    // We implement OnUpdate method to run custom logic once per frame.
    public override void OnUpdate(GameTime gameTime)
    {
        // We read position of mouse from InputComponent,
        // then we convert it to world space using CameraComponent
        // and finally we set it as translation of Transform2DComponent.
        _transform2DComponent.Translation = _cameraComponent.ScreenPointToWorld2DPoint(_inputComponent.HardwareInput.MouseInput.Position);
    }
}

// To make component available to the engine we need to create factory for that component
// and register it in IComponentsRegistry which is done in DemoApp.cs file.
internal sealed class FollowMousePositionComponentFactory : ComponentFactory<FollowMousePositionComponent>
{
    protected override FollowMousePositionComponent CreateComponent(Entity entity) => new(entity);
}