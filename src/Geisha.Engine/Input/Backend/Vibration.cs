namespace Geisha.Engine.Input.Backend;

// TODO: Add documentation.
public readonly record struct Vibration
{
    public double LeftMotorSpeed { get; init; }
    public double RightMotorSpeed { get; init; }
}