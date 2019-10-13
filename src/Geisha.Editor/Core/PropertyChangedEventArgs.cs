using System;

namespace Geisha.Editor.Core
{
    public sealed class PropertyChangedEventArgs<T> : EventArgs
    {
        public PropertyChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public T OldValue { get; }
        public T NewValue { get; }
    }
}