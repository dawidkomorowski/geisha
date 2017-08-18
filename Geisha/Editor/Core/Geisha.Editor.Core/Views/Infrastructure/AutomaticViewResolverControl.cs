using System.Windows;
using System.Windows.Controls;
using Geisha.Editor.Core.ViewModels.Infrastructure;

namespace Geisha.Editor.Core.Views.Infrastructure
{
    public class AutomaticViewResolverControl : ContentControl
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(ViewModel), typeof(AutomaticViewResolverControl),
                new FrameworkPropertyMetadata(null, ViewModelPropertyChangedCallback));

        public ViewModel ViewModel
        {
            get { return (ViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
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
            }
        }
    }
}