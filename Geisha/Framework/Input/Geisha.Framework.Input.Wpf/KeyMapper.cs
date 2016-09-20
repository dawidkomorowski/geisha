using System;

namespace Geisha.Framework.Input.Wpf
{
    public class KeyMapper : IKeyMapper
    {
        public System.Windows.Input.Key Map(Key key)
        {
            switch (key)
            {
                case Key.Down:
                    return System.Windows.Input.Key.Down;
                case Key.Left:
                    return System.Windows.Input.Key.Left;
                case Key.Right:
                    return System.Windows.Input.Key.Right;
                case Key.Up:
                    return System.Windows.Input.Key.Up;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }
        }
    }
}