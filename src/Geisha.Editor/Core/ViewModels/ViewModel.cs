using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Geisha.Editor.Core.ViewModels
{
    // TODO Reconsider this property changed implementation. Maybe introduce introduce Property class?
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
    }
}