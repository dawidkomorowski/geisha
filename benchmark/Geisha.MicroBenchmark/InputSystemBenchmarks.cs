using BenchmarkDotNet.Attributes;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Backend;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Input.Systems;
using Geisha.TestUtils;

namespace Geisha.MicroBenchmark;

[MemoryDiagnoser]
public class InputSystemBenchmarks
{
    private Scene _scene = null!;
    private InputSystem _inputSystem = null!;
    private StubInputProvider _inputProvider = null!;

    // Pre-computed input sequence covering all code paths: keyboard actions, mouse actions,
    // keyboard axes, mouse axes, multiple hardware actions per mapping, state transitions.
    private static readonly HardwareInput[] InputSequence = CreateInputSequence();

    private static HardwareInput[] CreateInputSequence()
    {
        return new[]
        {
            // No input (all released)
            new HardwareInput(default, default),
            // Right key pressed - triggers MoveRight action binding (false -> true transition)
            new HardwareInput(new KeyboardInputBuilder { Right = true }.Build(), default),
            // Right key still held - no new action trigger (true -> true, no transition)
            new HardwareInput(new KeyboardInputBuilder { Right = true }.Build(), default),
            // Left + Up keys pressed - triggers MoveLeft and Jump (via Up key, first hardware action)
            new HardwareInput(new KeyboardInputBuilder { Left = true, Up = true }.Build(), default),
            // Space key pressed - triggers Jump via Space (second hardware action in mapping)
            new HardwareInput(new KeyboardInputBuilder { Space = true }.Build(), default),
            // Mouse fire + camera movement - triggers Fire action, produces LookX/LookY axis values
            new HardwareInput(default, new MouseInputBuilder { LeftButton = true, PositionDelta = new Vector2(5, 3) }.Build()),
            // Mouse alt-fire + camera movement
            new HardwareInput(default, new MouseInputBuilder { RightButton = true, PositionDelta = new Vector2(-3, 7) }.Build()),
            // Mouse zoom + melee (XButton1 triggers Melee via its first hardware action)
            new HardwareInput(default, new MouseInputBuilder { MiddleButton = true, XButton1 = true }.Build()),
            // Combined keyboard + mouse: MoveRight, Jump, Fire, camera look
            new HardwareInput(new KeyboardInputBuilder { Right = true, Up = true }.Build(),
                new MouseInputBuilder { LeftButton = true, PositionDelta = new Vector2(2, -5) }.Build()),
            // All released
            new HardwareInput(default, default),
        };
    }

    private void InitializeInputSystem()
    {
        _inputProvider = new StubInputProvider();
        _inputSystem = new InputSystem(new StubInputBackend(_inputProvider));
        _scene = TestSceneFactory.Create();
        _scene.AddObserver(_inputSystem);
    }

    private void CleanupInputSystem()
    {
        _scene.RemoveObserver(_inputSystem);
        _inputSystem = null!;
        _scene = null!;
    }

    private void SetupEntities()
    {
        // Entity 1: Keyboard action mappings + keyboard axis mappings + all bindings
        // Covers: keyboard actions (single hardware action), keyboard axes (multi hardware axis),
        //         multi hardware action mapping (Jump: Up OR Space), all bindings registered.
        {
            var inputMapping = InputMapping.CreateBuilder()
                .MapAction("MoveRight", Key.Right)
                .MapAction("MoveLeft", Key.Left)
                .MapAction("Jump", Key.Up)
                .MapAction("Jump", Key.Space)
                .MapAxis("MoveX", Key.Right, 1.0)
                .MapAxis("MoveX", Key.Left, -1.0)
                .MapAxis("MoveY", Key.Up, 1.0)
                .MapAxis("MoveY", Key.Down, -1.0)
                .Build();

            var entity = _scene.CreateEntity();
            var inputComponent = entity.CreateComponent<InputComponent>();
            inputComponent.InputMapping = inputMapping;
            inputComponent.BindAction("MoveRight", () => { });
            inputComponent.BindAction("MoveLeft", () => { });
            inputComponent.BindAction("Jump", () => { });
            inputComponent.BindAxis("MoveX", _ => { });
            inputComponent.BindAxis("MoveY", _ => { });
        }

        // Entity 2: Mouse action mappings
        // Covers: mouse button actions, multi hardware action mapping (Melee: XButton1 OR XButton2),
        //         mouse axis (AxisX, AxisY with negation), all bindings registered.
        {
            var inputMapping = InputMapping.CreateBuilder()
                .MapAction("Fire", MouseButton.Left)
                .MapAction("AltFire", MouseButton.Right)
                .MapAction("Zoom", MouseButton.Middle)
                .MapAction("Melee", MouseButton.XButton1)
                .MapAction("Melee", MouseButton.XButton2)
                .MapAxis("LookX", MouseAxis.X, 1.0)
                .MapAxis("LookY", MouseAxis.Y, 1.0)
                .Build();

            var entity = _scene.CreateEntity();
            var inputComponent = entity.CreateComponent<InputComponent>();
            inputComponent.InputMapping = inputMapping;
            inputComponent.BindAction("Fire", () => { });
            inputComponent.BindAction("AltFire", () => { });
            inputComponent.BindAction("Zoom", () => { });
            inputComponent.BindAction("Melee", () => { });
            inputComponent.BindAxis("LookX", _ => { });
            inputComponent.BindAxis("LookY", _ => { });
        }

        // Entity 3: Mixed keyboard + mouse mappings, partial bindings
        // Covers: mixed input source actions, no-binding code path for some actions.
        {
            var inputMapping = InputMapping.CreateBuilder()
                .MapAction("Dash", Key.LeftShift)
                .MapAction("Crouch", Key.LeftCtrl)
                .MapAction("PrimaryFire", MouseButton.Left)
                .MapAxis("Throttle", Key.Up, 1.0)
                .MapAxis("Throttle", Key.Down, -1.0)
                .Build();

            var entity = _scene.CreateEntity();
            var inputComponent = entity.CreateComponent<InputComponent>();
            inputComponent.InputMapping = inputMapping;
            inputComponent.BindAction("Dash", () => { });
            // "Crouch" and "PrimaryFire" intentionally have no bindings - exercises no-binding code path
            inputComponent.BindAxis("Throttle", _ => { });
        }

        // Entity 4: Mixed keyboard + mouse axes, multi-axis accumulation, no bindings
        // Covers: multi-source axis accumulation, pure state tracking (no bindings registered).
        {
            var inputMapping = InputMapping.CreateBuilder()
                .MapAction("Action", Key.Space)
                .MapAxis("CombinedAxis", Key.Right, 1.0)
                .MapAxis("CombinedAxis", Key.Left, -1.0)
                .MapAxis("CombinedAxis", MouseAxis.X, 0.5)
                .Build();

            var entity = _scene.CreateEntity();
            var inputComponent = entity.CreateComponent<InputComponent>();
            inputComponent.InputMapping = inputMapping;
            // No bindings registered
        }

        // Entity 5: Duplicate of Entity 1 - simulates a second active entity with full input processing
        {
            var inputMapping = InputMapping.CreateBuilder()
                .MapAction("MoveRight", Key.Right)
                .MapAction("MoveLeft", Key.Left)
                .MapAction("Jump", Key.Up)
                .MapAction("Jump", Key.Space)
                .MapAxis("MoveX", Key.Right, 1.0)
                .MapAxis("MoveX", Key.Left, -1.0)
                .MapAxis("MoveY", Key.Up, 1.0)
                .MapAxis("MoveY", Key.Down, -1.0)
                .Build();

            var entity = _scene.CreateEntity();
            var inputComponent = entity.CreateComponent<InputComponent>();
            inputComponent.InputMapping = inputMapping;
            inputComponent.BindAction("MoveRight", () => { });
            inputComponent.BindAction("MoveLeft", () => { });
            inputComponent.BindAction("Jump", () => { });
            inputComponent.BindAxis("MoveX", _ => { });
            inputComponent.BindAxis("MoveY", _ => { });
        }
    }

    [IterationSetup]
    public void IterationSetup()
    {
        InitializeInputSystem();
        SetupEntities();

        // Process one frame with empty input to initialize action states (HasActionStatesInitialized = false path).
        // This is setup work and not part of the benchmark.
        _inputProvider.CurrentInput = new HardwareInput(default, default);
        _inputSystem.ProcessInput();
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        CleanupInputSystem();
    }

    [Benchmark]
    public void ProcessInput_5Entities_216_000Frames()
    {
        // Assuming 60 FPS this simulates 1h (3600 seconds) of input processing.
        for (var i = 0; i < 216_000; i++)
        {
            _inputProvider.CurrentInput = InputSequence[i % InputSequence.Length];
            _inputSystem.ProcessInput();
        }
    }

    private sealed class StubInputProvider : IInputProvider
    {
        public bool LockCursorPosition { get; set; }
        public HardwareInput CurrentInput { get; set; } = HardwareInput.Empty;
        public HardwareInput Capture() => CurrentInput;
    }

    private sealed class StubInputBackend : IInputBackend
    {
        private readonly StubInputProvider _inputProvider;

        public StubInputBackend(StubInputProvider inputProvider)
        {
            _inputProvider = inputProvider;
        }

        public IInputProvider CreateInputProvider() => _inputProvider;
    }
}
