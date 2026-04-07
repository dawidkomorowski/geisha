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

/// <summary>
/// Component that handles input-driven transformation updates with delta time scaling for frame independence.
/// </summary>
internal sealed class TransformControllerComponent : BehaviorComponent
{
    private InputComponent _inputComponent = null!;
    private Transform2DComponent _transform = null!;

    // Movement parameters
    private const double MoveSpeed = 200.0; // units per second
    private const double RotateSpeed = 2.5; // radians per second
    private const double ScaleSpeed = 0.6; // scale units per second
    private const double MinScale = 0.3;
    private const double MaxScale = 1.0;
    private static readonly Vector2 MinTranslation = new(100, -150);
    private static readonly Vector2 MaxTranslation = new(500, 50);

    public TransformControllerComponent(Entity entity) : base(entity)
    {
    }

    public override void OnStart()
    {
        _inputComponent = Entity.GetComponent<InputComponent>();
        _transform = Entity.GetComponent<Transform2DComponent>();
    }

    public override void OnUpdate(in TimeStep timeStep)
    {
        var deltaSeconds = timeStep.DeltaTimeSeconds;

        // Handle vertical movement
        var moveVertical = _inputComponent.GetAxisState("MoveVertically");
        if (moveVertical != 0)
        {
            var newTranslation = _transform.Translation + Vector2.UnitY * MoveSpeed * moveVertical * deltaSeconds;
            _transform.Translation = Vector2.Max(Vector2.Min(newTranslation, MaxTranslation), MinTranslation);
        }

        // Handle horizontal movement
        var moveHorizontal = _inputComponent.GetAxisState("MoveHorizontally");
        if (moveHorizontal != 0)
        {
            var newTranslation = _transform.Translation + Vector2.UnitX * MoveSpeed * moveHorizontal * deltaSeconds;
            _transform.Translation = Vector2.Max(Vector2.Min(newTranslation, MaxTranslation), MinTranslation);
        }

        // Handle rotation
        var rotateRight = _inputComponent.GetAxisState("RotateRight");
        if (rotateRight != 0)
        {
            _transform.Rotation -= rotateRight * RotateSpeed * deltaSeconds;
        }

        // Handle scaling
        var scaleUp = _inputComponent.GetAxisState("ScaleUp");
        if (scaleUp != 0)
        {
            var newScale = _transform.Scale + Vector2.One * ScaleSpeed * scaleUp * deltaSeconds;
            _transform.Scale = Vector2.Max(Vector2.Min(newScale, new Vector2(MaxScale, MaxScale)), new Vector2(MinScale, MinScale));
        }
    }
}

internal sealed class TransformControllerComponentFactory : ComponentFactory<TransformControllerComponent>
{
    protected override TransformControllerComponent CreateComponent(Entity entity) => new(entity);
}