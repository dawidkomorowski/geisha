using System.Collections.Generic;

namespace Geisha.Framework.Input
{
    public class KeyboardInput
    {
        private readonly Dictionary<Key, bool> _keyStates;

        public KeyboardInput(Dictionary<Key, bool> keyStates)
        {
            _keyStates = keyStates;
        }

        public bool this[Key key] => _keyStates[key];
    }
}