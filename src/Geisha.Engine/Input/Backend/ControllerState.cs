namespace Geisha.Engine.Input.Backend;

// TODO: Add documentation.
// TODO: Should it be in Backend or in Input namespace?
// TODO: Other types use name "input" for similar state concept like KeyboardInput, MouseInput. Is it valid naming?
public readonly record struct ControllerState
{
    public ControllerIndex Index { get; init; }
    public bool IsConnected { get; init; }
    public int PacketNumber { get; init; }
}