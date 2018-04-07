using System.Collections.Generic;

namespace Geisha.Framework.Input
{
    /// <summary>
    ///     Represents state of hardware input devices.
    /// </summary>
    public class HardwareInput
    {
        public HardwareInput(KeyboardInput keyboardInput)
        {
            KeyboardInput = keyboardInput;
        }

        // TODO what should empty be? Shouldn't it be a false for all keys and so on?
        /// <summary>
        ///     Empty hardware input.
        /// </summary>
        public static HardwareInput Empty => new HardwareInput(new KeyboardInput(new Dictionary<Key, bool>()));

        /// <summary>
        ///     State of keyboard.
        /// </summary>
        public KeyboardInput KeyboardInput { get; }
    }
}