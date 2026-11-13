using Geisha.Engine.Input.Backend;

namespace Geisha.Engine.Input;

// TODO: Add documentation.
// TODO: Test dependency injection for this interface.
public interface IInputSystem
{
    // TODO: Add documentation.
    ControllerState GetControllerState(ControllerIndex controllerIndex);

    // TODO: Add documentation.
    void SetControllerVibration(ControllerIndex controllerIndex, Vibration vibration);
}