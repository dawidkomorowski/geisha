using System.Collections.Generic;

namespace Geisha.Framework.Input
{
    /// <summary>
    ///     Represents state of keyboard.
    /// </summary>
    public class KeyboardInput
    {
        private readonly Dictionary<Key, bool> _keyStates;

        public KeyboardInput(Dictionary<Key, bool> keyStates)
        {
            _keyStates = keyStates;
        }

        /// <summary>
        ///     Returns state of single keyboard key.
        /// </summary>
        /// <param name="key">Key whose state is requested.</param>
        /// <returns>True, if key is pressed; otherwise false.</returns>
        public bool this[Key key] => _keyStates[key];
    }
}