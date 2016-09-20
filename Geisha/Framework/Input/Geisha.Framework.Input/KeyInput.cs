using System.Collections.Generic;

namespace Geisha.Framework.Input
{
    public struct KeyInput
    {
        private readonly Dictionary<Key, bool> _keyStates;

        public KeyInput(Dictionary<Key, bool> keyStates) : this()
        {
            _keyStates = keyStates;
        }

        public bool this[Key key] => _keyStates[key];
    }
}