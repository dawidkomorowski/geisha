using System.Collections.Generic;

namespace Geisha.Framework.Input
{
    public enum Key
    {
        Down,
        Left,
        Right,
        Up
    }

    public static class KeyExtensions
    {
        public static IEnumerable<Key> Enumerate()
        {
            return new[] {Key.Down, Key.Left, Key.Right, Key.Up};
        }
    }
}