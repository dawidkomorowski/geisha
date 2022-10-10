using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using NLog;

namespace Geisha.Editor.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RegisterViewForAttribute : Attribute
    {
        public RegisterViewForAttribute(Type viewModelType)
        {
            ViewModelType = viewModelType;
        }

        public Type ViewModelType { get; }
    }

    public interface IViewRepository
    {
        Control CreateView(ViewModel viewModel);
        void RegisterView(Type viewType, Type viewModelType);
        void RegisterViewsFromCurrentlyLoadedAssemblies();
    }

    public sealed class ViewRepository : IViewRepository
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly ViewRepository DefaultInstance = new();
        private readonly Dictionary<Type, Type> _registeredViews = new();
        public static IViewRepository Default => DefaultInstance;

        public Control CreateView(ViewModel viewModel)
        {
            if (_registeredViews.TryGetValue(viewModel.GetType(), out var viewType))
            {
                var view = Activator.CreateInstance(viewType) ??
                           throw new InvalidOperationException(
                               $"Could not create view for specified view model. Type of view model: {viewModel.GetType().FullName}");

                return (Control)view;
            }
            else
            {
                throw new ArgumentException($"There is no view registered for specified view model. Type of view model: {viewModel.GetType().FullName}",
                    nameof(viewModel));
            }
        }

        public void RegisterView(Type viewType, Type viewModelType)
        {
            if (!typeof(Control).IsAssignableFrom(viewType))
            {
                throw new ArgumentException($"View of specified type must inherit from Control class. Type of view: {viewType.FullName}", nameof(viewType));
            }

            if (!typeof(ViewModel).IsAssignableFrom(viewModelType))
            {
                throw new ArgumentException($"View model of specified type must inherit from ViewModel class. Type of view model: {viewModelType.FullName}",
                    nameof(viewModelType));
            }

            _registeredViews.Add(viewModelType, viewType);
            Logger.Info("Registered view {0} for view model {1}.", viewType.FullName, viewModelType.FullName);
        }

        public void RegisterViewsFromCurrentlyLoadedAssemblies()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var viewTypes = loadedAssemblies.SelectMany(a => a.GetTypes()).Where(t => t.GetCustomAttribute<RegisterViewForAttribute>() != null);
            foreach (var viewType in viewTypes)
            {
                var registerViewForAttribute = viewType.GetCustomAttribute<RegisterViewForAttribute>();
                Debug.Assert(registerViewForAttribute != null, nameof(registerViewForAttribute) + " != null");
                var viewModelType = registerViewForAttribute.ViewModelType;
                RegisterView(viewType, viewModelType);
            }
        }
    }
}