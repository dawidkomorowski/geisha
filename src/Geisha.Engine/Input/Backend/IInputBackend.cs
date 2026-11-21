namespace Geisha.Engine.Input.Backend;

// TODO: Improve these docs?
/// <summary>
///     Defines interface of input backend used by the engine.
/// </summary>
/// <remarks>
///     Input backend provides API for reading state of input devices.
/// </remarks>
public interface IInputBackend
{
    // TODO: HardwareInput type is getting bigger with each new device being supported and the current implementation allocates per frame.
    //       If the HardwareInput is meant to nicely hold snapshot of direct input state of all devices for InputComponents then maybe it should
    //       be managed by Input System - Input System performs pooling of HardwareInput or explicitly mutates single instance (???) and it calls
    //       input backend only for per device state (lower level API)?
    // TODO: Validate that Input System micro benchmarks still work correctly.
    // TODO: Add documentation.
    KeyboardInput GetKeyboardInput();

    // TODO: Add documentation.
    MouseInput GetMouseInput();

    // TODO: Add documentation.
    // TODO: Document reconnect interval.
    ControllerState GetControllerState(ControllerIndex controllerIndex);

    // TODO: Add documentation.
    // TODO: Document vibration command is lost when controller is disconnected.
    void SetControllerVibration(ControllerIndex controllerIndex, Vibration vibration);
}