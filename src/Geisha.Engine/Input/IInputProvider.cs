namespace Geisha.Engine.Input
{
    /// <summary>
    ///     Specifies API for capturing current state of input devices.
    /// </summary>
    public interface IInputProvider
    {
        /// <summary>
        ///     Captures current state of input devices as instance of <see cref="HardwareInput" /> class.
        /// </summary>
        /// <returns><see cref="HardwareInput" /> instance that represents captured state of input devices.</returns>
        HardwareInput Capture();
    }
}