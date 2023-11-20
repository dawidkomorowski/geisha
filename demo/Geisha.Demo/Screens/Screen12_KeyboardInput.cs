using System;
using System.Text;
using Geisha.Demo.Common;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Demo.Screens;

internal sealed class KeyboardInputSceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "Screen12_KeyboardInput";
    private readonly CommonScreenFactory _commonScreenFactory;

    public KeyboardInputSceneBehaviorFactory(CommonScreenFactory commonScreenFactory)
    {
        _commonScreenFactory = commonScreenFactory;
    }

    public string BehaviorName => SceneBehaviorName;
    public SceneBehavior Create(Scene scene) => new KeyboardInputSceneBehavior(scene, _commonScreenFactory);

    private sealed class KeyboardInputSceneBehavior : SceneBehavior
    {
        private readonly CommonScreenFactory _commonScreenFactory;

        public KeyboardInputSceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory) : base(scene)
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

            // Create entity showing pressed keyboard keys.
            var keyboardInput = Scene.CreateEntity();
            // Add Transform2DComponent to entity to control its position.
            keyboardInput.CreateComponent<Transform2DComponent>();
            // Add TextRendererComponent to entity so it can show text on the screen.
            var keyboardInputText = keyboardInput.CreateComponent<TextRendererComponent>();
            // Set text properties.
            keyboardInputText.Color = Color.FromArgb(255, 255, 0, 255);
            keyboardInputText.FontSize = FontSize.FromDips(40);
            keyboardInputText.TextAlignment = TextAlignment.Center;
            keyboardInputText.ParagraphAlignment = ParagraphAlignment.Center;
            keyboardInputText.MaxWidth = 1600;
            keyboardInputText.MaxHeight = 900;
            keyboardInputText.Pivot = new Vector2(800, 450);
            // Add InputComponent to entity so we can handle user input.
            keyboardInput.CreateComponent<InputComponent>();
            // Add custom component that updates text based on keyboard input.
            keyboardInput.CreateComponent<SetTextToKeyboardInputComponent>();

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
            textRenderer1.Text = "Geisha Engine provides InputComponent that allows to read keyboard input and perform custom game logic based on that.";

            // Create entity representing second text block.
            var textBlock2 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var textBlock2Transform = textBlock2.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            textBlock2Transform.Translation = new Vector2(0, -225);
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
            textRenderer2.Text = "Press any keyboard key.";

            // Create entity representing third text block.
            var textBlock3 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var textBlock3Transform = textBlock3.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            textBlock3Transform.Translation = new Vector2(0, -325);
            // Add TextRendererComponent to entity so it can show text on the screen.
            var textRenderer3 = textBlock3.CreateComponent<TextRendererComponent>();
            // Set text properties.
            textRenderer3.Color = Color.Black;
            textRenderer3.FontSize = FontSize.FromDips(40);
            textRenderer3.TextAlignment = TextAlignment.Center;
            textRenderer3.ParagraphAlignment = ParagraphAlignment.Center;
            textRenderer3.MaxWidth = 1600;
            textRenderer3.MaxHeight = 900;
            textRenderer3.Pivot = new Vector2(800, 450);
            textRenderer3.Text = "Press [ENTER] to go to the next screen. Press [BACKSPACE] to go back.";
        }
    }
}

// This is implementation of custom component based on BehaviorComponent.
// Behavior components are handled by BehaviorSystem and easily allow to get custom code being run.
// This component updates text of TextRendererComponent attached to the same entity to contain information
// about pressed keyboard keys read from InputComponent attached to the same entity.
internal sealed class SetTextToKeyboardInputComponent : BehaviorComponent
{
    private InputComponent _inputComponent = null!;
    private TextRendererComponent _textRendererComponent = null!;

    public SetTextToKeyboardInputComponent(Entity entity) : base(entity)
    {
    }

    // We implement OnStart method to initialize component state.
    public override void OnStart()
    {
        // In this case we retrieve needed components from entity.
        _inputComponent = Entity.GetComponent<InputComponent>();
        _textRendererComponent = Entity.GetComponent<TextRendererComponent>();
    }

    // We implement OnUpdate method to run custom logic once per frame.
    public override void OnUpdate(GameTime gameTime)
    {
        var stringBuilder = new StringBuilder();

        // Check state of all keys and add information about the pressed ones.
        foreach (var key in Enum.GetValues<Key>())
        {
            if (_inputComponent.HardwareInput.KeyboardInput[key])
            {
                stringBuilder.Append($"[{key}]");
            }
        }

        _textRendererComponent.Text = stringBuilder.ToString();

        // If no keys were pressed then we present placeholder.
        if (string.IsNullOrEmpty(_textRendererComponent.Text))
        {
            _textRendererComponent.Text = "Here you will see pressed keyboard keys";
        }
    }
}

// To make component available to the engine we need to create factory for that component
// and register it in IComponentsRegistry which is done in DemoApp.cs file.
internal sealed class SetTextToKeyboardInputComponentFactory : ComponentFactory<SetTextToKeyboardInputComponent>
{
    protected override SetTextToKeyboardInputComponent CreateComponent(Entity entity) => new(entity);
}