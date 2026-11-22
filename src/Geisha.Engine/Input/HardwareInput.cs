namespace Geisha.Engine.Input;

// TODO: Revisit documentation.
/// <summary>
///     Represents state of input devices.
/// </summary>
public sealed class HardwareInput
{
    // TODO: Add documentation.
    public HardwareInput()
    {
        KeyboardInput = default;
        MouseInput = default;
        Controller1 = new ControllerState { Index = ControllerIndex.One };
        Controller2 = new ControllerState { Index = ControllerIndex.Two };
        Controller3 = new ControllerState { Index = ControllerIndex.Three };
        Controller4 = new ControllerState { Index = ControllerIndex.Four };
    }

    /// <summary>
    ///     Empty hardware input.
    /// </summary>
    public static HardwareInput Empty { get; } = new();

    /// <summary>
    ///     State of keyboard.
    /// </summary>
    public KeyboardInput KeyboardInput { get; init; }

    /// <summary>
    ///     State of mouse.
    /// </summary>
    public MouseInput MouseInput { get; init; }

    // TODO: Add documentation.
    public ControllerState Controller1 { get; init; }

    // TODO: Add documentation.
    public ControllerState Controller2 { get; init; }

    // TODO: Add documentation.
    public ControllerState Controller3 { get; init; }

    // TODO: Add documentation.
    public ControllerState Controller4 { get; init; }
}