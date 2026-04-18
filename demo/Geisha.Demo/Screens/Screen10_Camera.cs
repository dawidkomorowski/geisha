using System.Collections.Immutable;
using System.Linq;
using Geisha.Demo.Common;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Demo.Screens;

internal sealed class CameraSceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "Screen10_Camera";
    private readonly CommonScreenFactory _commonScreenFactory;

    public CameraSceneBehaviorFactory(CommonScreenFactory commonScreenFactory)
    {
        _commonScreenFactory = commonScreenFactory;
    }

    public string BehaviorName => SceneBehaviorName;
    public SceneBehavior Create(Scene scene) => new CameraSceneBehavior(scene, _commonScreenFactory);

    private sealed class CameraSceneBehavior : SceneBehavior
    {
        private readonly CommonScreenFactory _commonScreenFactory;

        public CameraSceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory) : base(scene)
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
            var cameraTransform = camera.CreateComponent<Transform2DComponent>();
            // Set scale of camera to 0.5 to make camera view smaller.
            cameraTransform.Scale = new Vector2(0.5, 0.5);
            // Add CameraComponent to entity so we can control what is visible on the screen.
            var cameraComponent = camera.CreateComponent<CameraComponent>();
            // Set size of the camera to be 1600x900 units - in this case it corresponds to widow size in pixels.
            // Note: Leaving ViewRectangle at default (0, 0) would automatically use screen size as the effective view rectangle,
            // adapting to any resolution. Setting an explicit value enables logical scaling independent of screen resolution.
            cameraComponent.ViewRectangle = new Vector2(1600, 900);
            // Make menu a child of camera so it sticks to camera (is not affected by camera transformations).
            var menu = Scene.AllEntities.Single(e => e.Name == "Menu");
            menu.Parent = camera;
            // Add InputComponent to entity so we can handle user input.
            var inputComponent = camera.CreateComponent<InputComponent>();
            // Set input mapping so selected keys will trigger corresponding actions.
            inputComponent.InputMapping = new InputMapping
            {
                AxisMappings = ImmutableArray.Create
                (
                    new AxisMapping
                    {
                        AxisName = "MoveVertically",
                        HardwareAxes = ImmutableArray.Create
                        (
                            new HardwareAxis
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Up),
                                Scale = 1
                            },
                            new HardwareAxis
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Down),
                                Scale = -1
                            }
                        )
                    },
                    new AxisMapping
                    {
                        AxisName = "MoveHorizontally",
                        HardwareAxes = ImmutableArray.Create
                        (
                            new HardwareAxis
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Right),
                                Scale = 1
                            },
                            new HardwareAxis
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Left),
                                Scale = -1
                            }
                        )
                    }
                )
            };
            // Add component that handles camera movement with frame-independent speed.
            camera.CreateComponent<CameraControlComponent>();

            // Create entity representing first text block.
            var textBlock1 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var textBlock1Transform = textBlock1.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            textBlock1Transform.Translation = new Vector2(-650, 150);
            // Add TextRendererComponent to entity so it can show text on the screen.
            var textRenderer1 = textBlock1.CreateComponent<TextRendererComponent>();
            // Set text properties.
            textRenderer1.SortingLayerName = "Menu";
            textRenderer1.Color = Color.Black;
            textRenderer1.FontSize = FontSize.FromDips(40);
            textRenderer1.MaxWidth = 1300;
            textRenderer1.MaxHeight = 500;
            textRenderer1.Text = "You can use Camera component to control what part of scene is visible on the screen.";
            // Make first text block a child of camera so it sticks to camera (is not affected by camera transformations).
            textBlock1.Parent = camera;

            // Create entity representing controls info.
            var controlsInfo = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var controlsInfoTransform = controlsInfo.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            controlsInfoTransform.Translation = new Vector2(0, -50);
            // Add TextRendererComponent to entity so it can show text on the screen.
            var controlsInfoRenderer = controlsInfo.CreateComponent<TextRendererComponent>();
            // Set text properties.
            controlsInfoRenderer.SortingLayerName = "Menu";
            controlsInfoRenderer.Color = Color.Black;
            controlsInfoRenderer.FontSize = FontSize.FromDips(40);
            controlsInfoRenderer.TextAlignment = TextAlignment.Center;
            controlsInfoRenderer.ParagraphAlignment = ParagraphAlignment.Center;
            controlsInfoRenderer.MaxWidth = 1600;
            controlsInfoRenderer.MaxHeight = 900;
            controlsInfoRenderer.Pivot = new Vector2(800, 450);
            controlsInfoRenderer.Text = "Press [UP][DOWN][LEFT][RIGHT] to move camera.";
            // Make controls info a child of camera so it sticks to camera (is not affected by camera transformations).
            controlsInfo.Parent = camera;

            // Create entity representing red square.
            var entity1 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var transform1 = entity1.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            transform1.Translation = new Vector2(300, -50);
            // Add RectangleRendererComponent to entity so it can show red square on the screen.
            var renderer1 = entity1.CreateComponent<RectangleRendererComponent>();
            // Set rectangle properties.
            renderer1.Dimensions = new Vector2(200, 200);
            renderer1.Color = Color.Red;
            renderer1.FillInterior = true;

            // Create entity representing green square.
            var entity2 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var transform2 = entity2.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            transform2.Translation = new Vector2(-350, -150);
            // Add RectangleRendererComponent to entity so it can show green square on the screen.
            var renderer2 = entity2.CreateComponent<RectangleRendererComponent>();
            // Set rectangle properties.
            renderer2.Dimensions = new Vector2(100, 100);
            renderer2.Color = Color.Green;
            renderer2.FillInterior = true;

            // Create entity representing blue square.
            var entity3 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var transform3 = entity3.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            transform3.Translation = new Vector2(-225, 225);
            // Add RectangleRendererComponent to entity so it can show blue square on the screen.
            var renderer3 = entity3.CreateComponent<RectangleRendererComponent>();
            // Set rectangle properties.
            renderer3.Dimensions = new Vector2(150, 150);
            renderer3.Color = Color.Blue;
            renderer3.FillInterior = true;

            // Create entity representing second text block.
            var textBlock2 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var textBlock2Transform = textBlock2.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            textBlock2Transform.Translation = new Vector2(0, -325);
            // Add TextRendererComponent to entity so it can show text on the screen.
            var textRenderer2 = textBlock2.CreateComponent<TextRendererComponent>();
            // Set text properties.
            textRenderer2.SortingLayerName = "Menu";
            textRenderer2.Color = Color.Black;
            textRenderer2.FontSize = FontSize.FromDips(40);
            textRenderer2.TextAlignment = TextAlignment.Center;
            textRenderer2.ParagraphAlignment = ParagraphAlignment.Center;
            textRenderer2.MaxWidth = 1600;
            textRenderer2.MaxHeight = 900;
            textRenderer2.Pivot = new Vector2(800, 450);
            textRenderer2.Text = "Press [ENTER] to go to the next screen. Press [BACKSPACE] to go back.";
            // Make second text block a child of camera so it sticks to camera (is not affected by camera transformations).
            textBlock2.Parent = camera;
        }
    }
}

// Component that handles input-driven camera position updates with delta time scaling for frame independence.
//
// This component demonstrates:
// 1. How to implement a simple BehaviorComponent for a specific task (camera control)
// 2. How to achieve frame-independent camera movement by scaling with delta time
// 3. How to clamp camera position to constrain the view area
//
// This is a simplified version compared to TransformControllerComponent (Screen09) as it only handles
// position updates without rotation or scaling. The frame independence principle remains the same:
// multiplying movement speed by delta time ensures consistent camera movement regardless of frame rate.
internal sealed class CameraControlComponent : BehaviorComponent
{
    private InputComponent _inputComponent = null!;
    private Transform2DComponent _transform = null!;

    // Camera movement speed in units per second.
    // This is scaled by delta time to achieve frame-independent movement.
    private const double MoveSpeed = 100.0; // units per second

    // Camera position bounds constrain the camera view area.
    // These bounds prevent the camera from moving too far away from the center.
    private static readonly Vector2 MinPosition = new(-250, -250);
    private static readonly Vector2 MaxPosition = new(250, 250);

    public CameraControlComponent(Entity entity) : base(entity)
    {
    }

    // OnStart is called once when the component is first encountered by the engine during update.
    // This is where we cache references to other components on the same entity for efficient access.
    public override void OnStart()
    {
        // Retrieve the InputComponent to read axis states for camera movement input.
        _inputComponent = Entity.GetComponent<InputComponent>();
        // Retrieve the Transform2DComponent so we can modify the camera's position.
        _transform = Entity.GetComponent<Transform2DComponent>();
    }

    // OnUpdate is called once per frame. The timeStep parameter contains delta time information
    // needed to implement frame-independent camera movement.
    public override void OnUpdate(in TimeStep timeStep)
    {
        // Extract delta time in seconds. This is the time elapsed since the last frame.
        // By multiplying movement speed by this value, we ensure camera movement is frame-rate independent.
        var deltaSeconds = timeStep.DeltaTimeSeconds;

        // Handle vertical camera movement using UP/DOWN keys.
        // GetAxisState returns the axis value from the InputMapping configured for this entity.
        // The value is non-zero if the axis is triggered and can be positive or negative.
        var moveVertical = _inputComponent.GetAxisState("MoveVertically");
        if (moveVertical != 0)
        {
            // Calculate new camera position by adding movement along Y axis.
            // The formula: speed * axisValue * deltaTime gives us frame-independent movement.
            var newPosition = _transform.Translation + Vector2.UnitY * MoveSpeed * moveVertical * deltaSeconds;
            // Clamp the new position within boundaries to keep the camera view within acceptable limits.
            _transform.Translation = Vector2.Max(Vector2.Min(newPosition, MaxPosition), MinPosition);
        }

        // Handle horizontal camera movement using LEFT/RIGHT keys.
        var moveHorizontal = _inputComponent.GetAxisState("MoveHorizontally");
        if (moveHorizontal != 0)
        {
            // Calculate new camera position by adding movement along X axis.
            var newPosition = _transform.Translation + Vector2.UnitX * MoveSpeed * moveHorizontal * deltaSeconds;
            // Clamp the new position within boundaries.
            _transform.Translation = Vector2.Max(Vector2.Min(newPosition, MaxPosition), MinPosition);
        }
    }
}

// Factory class for CameraControlComponent.
//
// In the Geisha Engine, components must be registered through factories before they can be used.
// The engine uses component factories to create instances of components when needed, rather than
// instantiating components directly. This allows the engine to:
//
// 1. Manage component creation and initialization (with support for dependency injection if needed)
// 2. Support component persistence (serialization/deserialization)
// 3. Maintain type safety and component registration
// 4. Enable the engine to dynamically create components based on configuration or scene data
//
// To make CameraControlComponent available to the engine:
// 1. Create this factory class inheriting from ComponentFactory<TComponent>
// 2. Override CreateComponent to return a new instance of the component
// 3. Register the factory in Game.RegisterComponents() using IComponentsRegistry.RegisterComponentFactory<TFactory>()
//    (This is done in DemoApp.cs)
//
// The ComponentFactory<TComponent> base class automatically handles ComponentType and ComponentId generation,
// so we only need to implement CreateComponent to specify how instances are created.
internal sealed class CameraControlComponentFactory : ComponentFactory<CameraControlComponent>
{
    // Override CreateComponent to specify how new instances of CameraControlComponent are created.
    // The entity parameter is the entity to which the component will be attached.
    protected override CameraControlComponent CreateComponent(Entity entity) => new(entity);
}