using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Input.Backend;

// TODO: Add documentation.
// TODO: Should it be in Backend or in Input namespace?
// TODO: Other types use name "input" for similar state concept like KeyboardInput, MouseInput. Is it valid naming?
// TODO: Should the naming be taken directly from XInput or be more generic/familiar e.g. Shoulder -> Bumper.
public readonly record struct ControllerState
{
    public ControllerIndex Index { get; init; }
    public bool IsConnected { get; init; }
    public int PacketNumber { get; init; }
    public ControllerButtons Buttons { get; init; }
    public double LeftTrigger { get; init; }
    public double RightTrigger { get; init; }
    public Vector2 LeftThumb { get; init; }
    public Vector2 RightThumb { get; init; }

    // TODO: Should the names be prefixed with Button? Or Maybe XPressed? Or something other?
    public bool ButtonDPadUp => (Buttons & ControllerButtons.DPadUp) != ControllerButtons.None;
    public bool ButtonDPadDown => (Buttons & ControllerButtons.DPadDown) != ControllerButtons.None;
    public bool ButtonDPadLeft => (Buttons & ControllerButtons.DPadLeft) != ControllerButtons.None;
    public bool ButtonDPadRight => (Buttons & ControllerButtons.DPadRight) != ControllerButtons.None;
    public bool ButtonStart => (Buttons & ControllerButtons.Start) != ControllerButtons.None;
    public bool ButtonBack => (Buttons & ControllerButtons.Back) != ControllerButtons.None;
    public bool ButtonLeftThumb => (Buttons & ControllerButtons.LeftThumb) != ControllerButtons.None;
    public bool ButtonRightThumb => (Buttons & ControllerButtons.RightThumb) != ControllerButtons.None;
    public bool ButtonLeftShoulder => (Buttons & ControllerButtons.LeftShoulder) != ControllerButtons.None;
    public bool ButtonRightShoulder => (Buttons & ControllerButtons.RightShoulder) != ControllerButtons.None;
    public bool ButtonA => (Buttons & ControllerButtons.A) != ControllerButtons.None;
    public bool ButtonB => (Buttons & ControllerButtons.B) != ControllerButtons.None;
    public bool ButtonX => (Buttons & ControllerButtons.X) != ControllerButtons.None;
    public bool ButtonY => (Buttons & ControllerButtons.Y) != ControllerButtons.None;
}