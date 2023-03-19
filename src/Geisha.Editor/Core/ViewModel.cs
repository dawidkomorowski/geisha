using System;
using System.ComponentModel;

namespace Geisha.Editor.Core
{
    // TODO Consider using SourceGenerated properties.
    public interface IReadOnlyProperty<T>
    {
        string Name { get; }
        T Get();
        void Subscribe(Action<T> action);
    }

    public interface IProperty<T> : IReadOnlyProperty<T>
    {
        void Set(T value);
    }

    public interface IComputedProperty<T> : IReadOnlyProperty<T>
    {
    }

    public abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected IProperty<T> CreateProperty<T>(string name, T initialValue = default)
        {
            return new Property<T>(this, name, initialValue);
        }

        protected IComputedProperty<U> CreateComputedProperty<T, U>(string name, IReadOnlyProperty<T> sourceProperty, Func<T, U> computeFunc)
        {
            return new ComputedProperty<T, U>(this, name, (PropertyBase<T>) sourceProperty, computeFunc);
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private abstract class PropertyBase<T> : IReadOnlyProperty<T>
        {
            private readonly ViewModel _owner;
            private Action<T>? _subscription;

            protected PropertyBase(ViewModel owner, string name)
            {
                _owner = owner;
                Name = name;
            }

            public event EventHandler<ChangedEventArgs<T>>? Changed;

            public string Name { get; }

            public abstract T Get();

            public void Subscribe(Action<T> action)
            {
                _subscription = action;
            }

            protected void NotifyChanged(T value)
            {
                _subscription?.Invoke(value);
                _owner.NotifyPropertyChanged(Name);
                Changed?.Invoke(this, new ChangedEventArgs<T>(value));
            }

            public sealed class ChangedEventArgs<TArg> : EventArgs
            {
                public ChangedEventArgs(TArg newValue)
                {
                    NewValue = newValue;
                }

                public TArg NewValue { get; }
            }
        }

        private sealed class Property<T> : PropertyBase<T>, IProperty<T>
        {
            private T _value;

            public Property(ViewModel owner, string name, T initialValue) : base(owner, name)
            {
                _value = initialValue;
            }

            public override T Get()
            {
                return _value;
            }

            public void Set(T value)
            {
                if (Equals(_value, value)) return;

                _value = value;
                NotifyChanged(_value);
            }
        }

        private sealed class ComputedProperty<T, U> : PropertyBase<U>, IComputedProperty<U>
        {
            private readonly PropertyBase<T> _sourceProperty;
            private readonly Func<T, U> _computeFunc;

            public ComputedProperty(ViewModel owner, string name, PropertyBase<T> sourceProperty, Func<T, U> computeFunc) : base(owner, name)
            {
                _sourceProperty = sourceProperty;
                _computeFunc = computeFunc;

                _sourceProperty.Changed += (sender, args) => NotifyChanged(_computeFunc(args.NewValue));
            }

            public override U Get()
            {
                return _computeFunc(_sourceProperty.Get());
            }
        }
    }
}