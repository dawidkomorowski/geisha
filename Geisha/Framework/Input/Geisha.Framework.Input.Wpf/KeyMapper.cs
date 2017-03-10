using System;
using System.ComponentModel.Composition;

namespace Geisha.Framework.Input.Wpf
{
    [Export(typeof(IKeyMapper))]
    public class KeyMapper : IKeyMapper
    {
        public System.Windows.Input.Key Map(Key key)
        {
            switch (key)
            {
                case Key.Up:
                    return System.Windows.Input.Key.Up;
                case Key.Down:
                    return System.Windows.Input.Key.Down;
                case Key.Right:
                    return System.Windows.Input.Key.Right;
                case Key.Left:
                    return System.Windows.Input.Key.Left;
                case Key.Space:
                    return System.Windows.Input.Key.Space;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }
        }
    }
}