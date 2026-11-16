namespace Geisha.Engine.Input;

// TODO: Add documentation.
public readonly record struct Vibration
{
    public double LeftMotorSpeed { get; init; }
    public double RightMotorSpeed { get; init; }
}