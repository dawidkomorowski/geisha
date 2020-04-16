namespace Geisha.Engine.Input
{
    /// <summary>
    ///     Represents state of input devices.
    /// </summary>
    public sealed class HardwareInput
    {
        /// <summary>
        ///     Creates new instance of <see cref="HardwareInput" /> class with specified <see cref="KeyboardInput" /> and
        ///     <see cref="MouseInput" />.
        /// </summary>
        /// <param name="keyboardInput">Keyboard input part of the hardware input.</param>
        /// <param name="mouseInput">Mouse input part of the hardware input.</param>
        public HardwareInput(KeyboardInput keyboardInput, MouseInput mouseInput)
        {
            KeyboardInput = keyboardInput;
            MouseInput = mouseInput;
        }

        /// <summary>
        ///     Empty hardware input.
        /// </summary>
        public static HardwareInput Empty { get; } = new HardwareInput(default, default);

        /// <summary>
        ///     State of keyboard.
        /// </summary>
        public KeyboardInput KeyboardInput { get; }

        /// <summary>
        ///     State of mouse.
        /// </summary>
        public MouseInput MouseInput { get; }
    }
}