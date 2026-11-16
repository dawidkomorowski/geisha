namespace Geisha.Engine.Input.Backend;

// TODO: Improve this docs?
/// <summary>
///     Defines interface of input backend used by the engine.
/// </summary>
/// <remarks>
///     Input backend provides API for reading state of input devices.
/// </remarks>
public interface IInputBackend
{
    // TODO: Expose in IInputSystem as an API for a game code?
    /// <summary>
    ///     Specifies whether to lock the cursor position at the center of the window client area. If <c>true</c> the
    ///     cursor is successively moved to the center of the window client area; otherwise it is unlocked and can move
    ///     freely.
    /// </summary>
    /// <remarks>
    ///     It is useful to lock the cursor position (and often to hide it) when mouse is used as two-directional axis
    ///     input e.g. in FPS game to move the camera.
    /// </remarks>
    bool LockCursorPosition { get; set; }

    // TODO: Rename to some better name than "capture"?
    /// <summary>
    ///     Captures current state of input devices as instance of <see cref="HardwareInput" /> class.
    /// </summary>
    /// <returns><see cref="HardwareInput" /> instance that represents captured state of input devices.</returns>
    HardwareInput Capture();

    // TODO: Add documentation.
    ControllerState GetControllerState(ControllerIndex controllerIndex);

    // TODO: Add documentation.
    void SetControllerVibration(ControllerIndex controllerIndex, Vibration vibration);
}