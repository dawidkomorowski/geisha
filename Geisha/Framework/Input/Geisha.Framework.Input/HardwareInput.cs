namespace Geisha.Framework.Input
{
    /// <summary>
    ///     Represents state of hardware input devices.
    /// </summary>
    public sealed class HardwareInput
    {
        /// <summary>
        ///     Creates new instance of <see cref="HardwareInput" /> class with specified <see cref="KeyboardInput" />.
        /// </summary>
        /// <param name="keyboardInput"></param>
        public HardwareInput(KeyboardInput keyboardInput)
        {
            KeyboardInput = keyboardInput;
        }

        /// <summary>
        ///     Empty hardware input.
        /// </summary>
        public static HardwareInput Empty { get; } = new HardwareInput(default(KeyboardInput));

        /// <summary>
        ///     State of keyboard.
        /// </summary>
        public KeyboardInput KeyboardInput { get; }
    }
}