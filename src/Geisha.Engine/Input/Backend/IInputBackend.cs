namespace Geisha.Engine.Input.Backend
{
    /// <summary>
    ///     Defines interface of input backend used by the engine.
    /// </summary>
    /// <remarks>
    ///     Input backend provides services for reading state of input devices available on certain platform.
    /// </remarks>
    public interface IInputBackend
    {
        /// <summary>
        ///     Creates input provider suitable for current platform.
        /// </summary>
        /// <returns>New instance of <see cref="IInputProvider" /> suitable for current platform.</returns>
        IInputProvider CreateInputProvider();
    }
}