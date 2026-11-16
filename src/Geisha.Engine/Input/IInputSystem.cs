namespace Geisha.Engine.Input;

// TODO: Add documentation.
// TODO: Test dependency injection for this interface.
public interface IInputSystem
{
    // TODO: Should this API perform a new READ or return cached value captured on input READ time by the input system?
    //       Cached value benefits from consistency for all calls during single frame. Direct calls to lower level APIs will most likely be infrequent game code.
    // TODO: Add documentation.
    ControllerState GetControllerState(ControllerIndex controllerIndex);

    // TODO: Add documentation.
    void SetControllerVibration(ControllerIndex controllerIndex, Vibration vibration);
}