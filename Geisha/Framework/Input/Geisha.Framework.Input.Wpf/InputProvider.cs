using System.Collections.Generic;
using System.Windows.Input;

namespace Geisha.Framework.Input.Wpf
{
    public class InputProvider : IInputProvider
    {
        private readonly IKeyMapper _keyMapper;

        public InputProvider(IKeyMapper keyMapper)
        {
            _keyMapper = keyMapper;
        }

        public HardwareInput Capture()
        {
            var keyInput = CaptureKeyInput();
            return new HardwareInput(keyInput);
        }

        private KeyInput CaptureKeyInput()
        {
            var keyStates = new Dictionary<Key, bool>();
            foreach (var key in KeyExtensions.Enumerate())
            {
                keyStates[key] = Keyboard.IsKeyDown(_keyMapper.Map(key));
            }

            var keyInput = new KeyInput(keyStates);
            return keyInput;
        }
    }
}
