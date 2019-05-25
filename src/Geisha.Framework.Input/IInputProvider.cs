namespace Geisha.Framework.Input
{
    public interface IInputProvider
    {
        HardwareInput Capture();
    }
}
