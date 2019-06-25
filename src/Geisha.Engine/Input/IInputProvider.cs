namespace Geisha.Engine.Input
{
    public interface IInputProvider
    {
        HardwareInput Capture();
    }
}
