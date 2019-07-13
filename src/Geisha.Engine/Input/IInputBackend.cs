namespace Geisha.Engine.Input
{
    // TODO Add xml docs.
    public interface IInputBackend
    {
        IInputProvider CreateInputProvider();
    }
}