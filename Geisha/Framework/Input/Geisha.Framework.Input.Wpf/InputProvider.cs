using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;

namespace Geisha.Framework.Input.Wpf
{
    [Export(typeof(IInputProvider))]
    public class InputProvider : IInputProvider
    {
        private readonly IKeyMapper _keyMapper;

        [ImportingConstructor]
        public InputProvider(IKeyMapper keyMapper)
        {
            _keyMapper = keyMapper;
        }

        public HardwareInput Capture()
        {
            var keyInput = CaptureKeyInput();
            return new HardwareInput(keyInput);
        }

        private KeyboardInput CaptureKeyInput()
        {
            var keyStates = Enum.GetValues(typeof(Key)).Cast<Key>().ToDictionary(k => k, k => Keyboard.IsKeyDown(_keyMapper.Map(k)));
            var keyInput = new KeyboardInput(keyStates);
            return keyInput;
        }
    }
}