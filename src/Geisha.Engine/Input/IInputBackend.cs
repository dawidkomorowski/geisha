namespace Geisha.Engine.Input
{
    public interface IInputBackend
    {
        IInputProvider CreateInputProvider();
    }
}