using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Geisha.Editor.Core.ViewModels
{
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
        private readonly IDictionary<string, Action> _subscriptions = new Dictionary<string, Action>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void Set<T>(ref T propertyBackingField, T value, [CallerMemberName] string propertyName = null)
        {
            if ((propertyBackingField != null || value == null) && (propertyBackingField == null || propertyBackingField.Equals(value))) return;

            propertyBackingField = value;
            OnPropertyChanged(propertyName);
        }

        protected void Subscribe(string propertyName, Action action)
        {
            _subscriptions[propertyName] = action;
        }

        protected IProperty<T> CreateProperty<T>(string name, T initialValue = default)
        {
            return new Property<T>(this, name, initialValue);
        }

        protected IComputedProperty<U> CreateComputedProperty<T, U>(string name, IReadOnlyProperty<T> sourceProperty, Func<T, U> computeFunc)
        {
            return new ComputedProperty<T, U>(this, name, (PropertyBase<T>) sourceProperty, computeFunc);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            var dependentProperties = GetType().GetProperties().Where(pi => pi.GetCustomAttributes<DependsOnPropertyAttribute>(false)
                .Any(a => a.PropertyName == propertyName));

            foreach (var dependentProperty in dependentProperties)
            {
                OnPropertyChanged(dependentProperty.Name);
            }

            if (_subscriptions.TryGetValue(propertyName, out var action))
            {
                action();
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private abstract class PropertyBase<T> : IReadOnlyProperty<T>
        {
            private readonly ViewModel _owner;
            private Action<T> _subscription;

            protected PropertyBase(ViewModel owner, string name)
            {
                _owner = owner;
                Name = name;
            }

            public event EventHandler<ChangedEventArgs<T>> Changed;

            public string Name { get; }

            public abstract T Get();

            public void Subscribe(Action<T> action)
            {
                _subscription = action;
            }

            protected void NotifyChanged(T value)
            {
                _subscription?.Invoke(value);
                _owner.RaisePropertyChanged(Name);
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