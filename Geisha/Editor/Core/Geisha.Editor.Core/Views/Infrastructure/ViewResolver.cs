using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Geisha.Editor.Core.ViewModels.Infrastructure;

namespace Geisha.Editor.Core.Views.Infrastructure
{
    public static class ViewResolver
    {
        public static Window ResolveWindowForViewModel(ViewModel viewModel)
        {
            var windowType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(typeof(Window)))
                .SingleOrDefault(t => t.GetCustomAttribute<ViewModelAttribute>() != null &&
                                      t.GetCustomAttribute<ViewModelAttribute>().ViewModelType.IsInstanceOfType(viewModel));

            if (windowType == null) throw new ViewNotFoundForViewModelException(viewModel);

            return (Window) Activator.CreateInstance(windowType);
        }

        public static Control ResolveControlForViewModel(ViewModel viewModel)
        {
            var controlType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(typeof(Control)))
                .SingleOrDefault(t => t.GetCustomAttribute<ViewModelAttribute>() != null &&
                                      t.GetCustomAttribute<ViewModelAttribute>().ViewModelType.IsInstanceOfType(viewModel));

            if (controlType == null) throw new ViewNotFoundForViewModelException(viewModel);

            return (Control) Activator.CreateInstance(controlType);
        }

        private class ViewNotFoundForViewModelException : GeishaEditorException
        {
            public ViewNotFoundForViewModelException(ViewModel viewModel) : base(
                $"No view class found for view-model of type: {viewModel.GetType()}. Check for missing attribute {nameof(ViewModelAttribute)} for given view-model type on appropriate view class.")
            {
            }
        }
    }
}