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

internal sealed class RenderingEntityHierarchySceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "Screen09_RenderingEntityHierarchy";
    private readonly CommonScreenFactory _commonScreenFactory;

    public RenderingEntityHierarchySceneBehaviorFactory(CommonScreenFactory commonScreenFactory)
    {
        _commonScreenFactory = commonScreenFactory;
    }

    public string BehaviorName => SceneBehaviorName;
    public SceneBehavior Create(Scene scene) => new RenderingEntityHierarchySceneBehavior(scene, _commonScreenFactory);

    private sealed class RenderingEntityHierarchySceneBehavior : SceneBehavior
    {
        private readonly CommonScreenFactory _commonScreenFactory;

        public RenderingEntityHierarchySceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory) : base(scene)
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

            // Create entity representing first text block.
            var textBlock1 = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var textBlock1Transform = textBlock1.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            textBlock1Transform.Translation = new Vector2(-650, 350);
            // Add TextRendererComponent to entity so it can show text on the screen.
            var textRenderer1 = textBlock1.CreateComponent<TextRendererComponent>();
            // Set text properties.
            textRenderer1.Color = Color.Black;
            textRenderer1.FontSize = FontSize.FromDips(40);
            textRenderer1.MaxWidth = 1300;
            textRenderer1.MaxHeight = 500;
            textRenderer1.Text =
                "You can compose entities into a tree by making parent-child relationship between entities. Child entities derive transformations applied to their parents so you can transform group of entities together.";

            // Create entity representing controls info.
            var controlsInfo = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var controlsInfoTransform = controlsInfo.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            controlsInfoTransform.Translation = new Vector2(-300, -50);
            // Add TextRendererComponent to entity so it can show text on the screen.
            var controlsInfoRenderer = controlsInfo.CreateComponent<TextRendererComponent>();
            // Set text properties.
            controlsInfoRenderer.Color = Color.Black;
            controlsInfoRenderer.FontSize = FontSize.FromDips(35);
            controlsInfoRenderer.TextAlignment = TextAlignment.Leading;
            controlsInfoRenderer.ParagraphAlignment = ParagraphAlignment.Near;
            controlsInfoRenderer.MaxWidth = 800;
            controlsInfoRenderer.MaxHeight = 300;
            controlsInfoRenderer.Pivot = new Vector2(400, 150);
            controlsInfoRenderer.Text = @"Parent Controls

Move:
    [UP][DOWN][LEFT][RIGHT]
Rotate:
    [A][D]
Scale:
    [W][S]";

            // Create entity representing parent.
            var parent = Scene.CreateEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var parentTransform = parent.CreateComponent<Transform2DComponent>();
            // Set position of the entity.
            parentTransform.Translation = new Vector2(300, -50);
            // Add RectangleRendererComponent to entity so it can show red square on the screen.
            var parentRenderer = parent.CreateComponent<RectangleRendererComponent>();
            // Set rectangle properties.
            parentRenderer.Dimensions = new Vector2(300, 300);
            parentRenderer.Color = Color.Red;
            parentRenderer.FillInterior = true;
            parentRenderer.OrderInLayer = 1;
            // Add InputComponent to entity so we can handle user input.
            var inputComponent = parent.CreateComponent<InputComponent>();
            // Set input mapping so selected keys will trigger corresponding actions.
            inputComponent.InputMapping = new InputMapping
            {
                AxisMappings =
                {
                    new AxisMapping
                    {
                        AxisName = "MoveVertically",
                        HardwareAxes =
                        {
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
                        }
                    },
                    new AxisMapping
                    {
                        AxisName = "MoveHorizontally",
                        HardwareAxes =
                        {
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
                        }
                    },
                    new AxisMapping
                    {
                        AxisName = "RotateRight",
                        HardwareAxes =
                        {
                            new HardwareAxis
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.D),
                                Scale = 1
                            },
                            new HardwareAxis
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.A),
                                Scale = -1
                            }
                        }
                    },
                    new AxisMapping
                    {
                        AxisName = "ScaleUp",
                        HardwareAxes =
                        {
                            new HardwareAxis
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.W),
                                Scale = 1
                            },
                            new HardwareAxis
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.S),
                                Scale = -1
                            }
                        }
                    }
                }
            };
            // Add component that handles transform updates based on input.
            parent.CreateComponent<TransformControllerComponent>();

            // Create entity representing first child of parent entity.
            var child1 = parent.CreateChildEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var child1Transform = child1.CreateComponent<Transform2DComponent>();
            // Set position of the entity relative to the parent.
            child1Transform.Translation = new Vector2(50, -50);
            // Add RectangleRendererComponent to entity so it can show green square on the screen.
            var child1Renderer = child1.CreateComponent<RectangleRendererComponent>();
            // Set rectangle properties.
            child1Renderer.Dimensions = new Vector2(100, 100);
            child1Renderer.Color = Color.Green;
            child1Renderer.FillInterior = true;
            child1Renderer.OrderInLayer = 2;

            // Create entity representing second child of parent entity.
            var child2 = parent.CreateChildEntity();
            // Add Transform2DComponent to entity so we can control its position.
            var child2Transform = child2.CreateComponent<Transform2DComponent>();
            // Set position of the entity relative to the parent.
            child2Transform.Translation = new Vector2(0, 0);
            // Add TextRendererComponent to entity so it can show text on the screen.
            var child2Renderer = child2.CreateComponent<TextRendererComponent>();
            // Set text properties.
            child2Renderer.Color = Color.White;
            child2Renderer.FontSize = FontSize.FromDips(25);
            child2Renderer.FontFamilyName = "Calibri";
            child2Renderer.TextAlignment = TextAlignment.Justified;
            child2Renderer.ParagraphAlignment = ParagraphAlignment.Near;
            child2Renderer.MaxWidth = 280;
            child2Renderer.MaxHeight = 280;
            child2Renderer.Pivot = new Vector2(140, 140);
            child2Renderer.Text = "Red square is a parent entity. It has two children, a green square and this text block.";
            child2Renderer.OrderInLayer = 3;

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

// Component that handles input-driven transformation updates with delta time scaling for frame independence.
//
// This component demonstrates:
// 1. How to extend BehaviorComponent to implement custom game logic
// 2. How to use InputComponent to read mapped input axes
// 3. How to achieve frame-independent movement by scaling with delta time
// 4. How to clamp transform values to maintain boundaries
//
// Frame independence is achieved by multiplying movement speeds by delta time (DeltaTimeSeconds).
// This ensures that movement speed is consistent regardless of frame rate. For example, if MoveSpeed
// is 200 units per second and delta time is 0.016 seconds (60 FPS), the entity moves 3.2 units per frame.
// At 30 FPS with delta time of 0.033 seconds, the entity still moves ~6.6 units per frame (twice the distance,
// but covering twice the time), maintaining the same 200 units per second speed.
internal sealed class TransformControllerComponent : BehaviorComponent
{
    private InputComponent _inputComponent = null!;
    private Transform2DComponent _transform = null!;

    // Movement parameters define the speed of transformations in units/seconds or radians/seconds.
    // These values are then scaled by delta time in OnUpdate to achieve frame-independent behavior.
    private const double MoveSpeed = 200.0; // units per second - controls translation speed
    private const double RotateSpeed = 2.5; // radians per second - controls rotation speed

    private const double ScaleSpeed = 0.6; // scale units per second - controls scaling speed

    // Scale bounds prevent the entity from becoming too large or too small.
    private const double MinScale = 0.3;

    private const double MaxScale = 1.0;

    // Translation bounds keep the parent entity within the visible camera area.
    private static readonly Vector2 MinTranslation = new(100, -150);
    private static readonly Vector2 MaxTranslation = new(500, 50);

    public TransformControllerComponent(Entity entity) : base(entity)
    {
    }

    // OnStart is called once when the component is first encountered by the engine during update.
    // This is where we cache references to other components on the same entity for efficient access.
    public override void OnStart()
    {
        // Retrieve the InputComponent to read axis states for movement input.
        _inputComponent = Entity.GetComponent<InputComponent>();
        // Retrieve the Transform2DComponent so we can modify the entity's position, rotation, and scale.
        _transform = Entity.GetComponent<Transform2DComponent>();
    }

    // OnUpdate is called once per frame. The timeStep parameter contains delta time information
    // needed to implement frame-independent movement.
    public override void OnUpdate(in TimeStep timeStep)
    {
        // Extract delta time in seconds. This is the time elapsed since the last frame.
        // By multiplying movement speeds by this value, we ensure movement is frame-rate independent.
        var deltaSeconds = timeStep.DeltaTimeSeconds;

        // Handle vertical movement using UP/DOWN keys.
        // GetAxisState returns the axis value from the InputMapping configured for this entity.
        // The value is non-zero if the axis is triggered and can be positive or negative.
        var moveVertical = _inputComponent.GetAxisState("MoveVertically");
        if (moveVertical != 0)
        {
            // Calculate new translation by adding movement along Y axis.
            // The formula: speed * axisValue * deltaTime gives us frame-independent movement.
            var newTranslation = _transform.Translation + Vector2.UnitY * MoveSpeed * moveVertical * deltaSeconds;
            // Clamp the new position within boundaries to keep the entity visible on screen.
            _transform.Translation = Vector2.Max(Vector2.Min(newTranslation, MaxTranslation), MinTranslation);
        }

        // Handle horizontal movement using LEFT/RIGHT keys.
        var moveHorizontal = _inputComponent.GetAxisState("MoveHorizontally");
        if (moveHorizontal != 0)
        {
            // Calculate new translation by adding movement along X axis.
            var newTranslation = _transform.Translation + Vector2.UnitX * MoveSpeed * moveHorizontal * deltaSeconds;
            // Clamp the new position within boundaries.
            _transform.Translation = Vector2.Max(Vector2.Min(newTranslation, MaxTranslation), MinTranslation);
        }

        // Handle rotation using A/D keys.
        // Rotation is applied directly without clamping since rotation can wrap around.
        var rotateRight = _inputComponent.GetAxisState("RotateRight");
        if (rotateRight != 0)
        {
            // Apply rotation change. The negative sign inverts the rotation direction.
            // RotateSpeed * deltaTime ensures rotation speed is frame-independent in radians per second.
            _transform.Rotation -= rotateRight * RotateSpeed * deltaSeconds;
        }

        // Handle scaling using W/S keys.
        // Scaling adjusts the entity size while maintaining aspect ratio.
        var scaleUp = _inputComponent.GetAxisState("ScaleUp");
        if (scaleUp != 0)
        {
            // Calculate new scale by adding uniform scaling along both X and Y axes.
            // Vector2.One represents (1, 1) to apply the same scale change to both dimensions.
            var newScale = _transform.Scale + Vector2.One * ScaleSpeed * scaleUp * deltaSeconds;
            // Clamp the scale to prevent the entity from becoming too large or disappearing.
            _transform.Scale = Vector2.Max(Vector2.Min(newScale, new Vector2(MaxScale, MaxScale)), new Vector2(MinScale, MinScale));
        }
    }
}

// Factory class for TransformControllerComponent.
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
// To make TransformControllerComponent available to the engine:
// 1. Create this factory class inheriting from ComponentFactory<TComponent>
// 2. Override CreateComponent to return a new instance of the component
// 3. Register the factory in Game.RegisterComponents() using IComponentsRegistry.RegisterComponentFactory<TFactory>()
//    (This is done in DemoApp.cs)
//
// The ComponentFactory<TComponent> base class automatically handles ComponentType and ComponentId generation,
// so we only need to implement CreateComponent to specify how instances are created.
internal sealed class TransformControllerComponentFactory : ComponentFactory<TransformControllerComponent>
{
    // Override CreateComponent to specify how new instances of TransformControllerComponent are created.
    // The entity parameter is the entity to which the component will be attached.
    protected override TransformControllerComponent CreateComponent(Entity entity) => new(entity);
}