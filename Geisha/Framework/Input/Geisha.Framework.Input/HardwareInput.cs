using System.Collections.Generic;

namespace Geisha.Framework.Input
{
    public class HardwareInput
    {
        public static HardwareInput Empty => new HardwareInput(new KeyboardInput(new Dictionary<Key, bool>()));

        public KeyboardInput KeyboardInput { get; }

        public HardwareInput(KeyboardInput keyboardInput)
        {
            KeyboardInput = keyboardInput;
        }
    }
}