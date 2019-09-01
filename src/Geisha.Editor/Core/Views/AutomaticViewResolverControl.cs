using System.Windows;
using System.Windows.Controls;
using Geisha.Editor.Core.ViewModels;

namespace Geisha.Editor.Core.Views
{
    public class AutomaticViewResolverControl : ContentControl
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(ViewModel), typeof(AutomaticViewResolverControl),
                new FrameworkPropertyMetadata(null, ViewModelPropertyChangedCallback));

        public ViewModel ViewModel
        {
            get => (ViewModel) GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        private static void ViewModelPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var resolver = (AutomaticViewResolverControl) dependencyObject;
            var viewModel = (ViewModel) dependencyPropertyChangedEventArgs.NewValue;

            if (viewModel != null)
            {
                var control = ViewResolver.ResolveControlForViewModel(viewModel);
                control.DataContext = viewModel;
                resolver.Content = control;

                if (viewModel is IWindowContext windowContext)
                {
                    var parentWindow = ViewResolver.ResolveParentWindowForControl(resolver);
                    windowContext.Window = parentWindow;
                }
            }
        }
    }
}