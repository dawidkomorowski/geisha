namespace Geisha.Framework.Input
{
    /// <summary>
    ///     Represents state of hardware input devices.
    /// </summary>
    public sealed class HardwareInput
    {
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