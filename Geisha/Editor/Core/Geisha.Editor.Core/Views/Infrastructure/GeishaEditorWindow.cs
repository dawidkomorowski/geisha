using System.Windows;
using Geisha.Editor.Core.ViewModels.Infrastructure;

namespace Geisha.Editor.Core.Views.Infrastructure
{
    public class GeishaEditorWindow : Window, IWindow
    {
        protected GeishaEditorWindow()
        {
            DataContextChanged += OnDataContextChanged;
        }

        public void ShowModalChildWindow(ViewModel dataContext)
        {
            var window = ViewResolver.ResolveWindowForViewModel(dataContext);
            window.DataContext = dataContext;
            window.Owner = this;
            window.ShowDialog();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var windowContext = DataContext as IWindowContext;
            if (windowContext != null)
            {
                windowContext.Window = this;
            }
        }
    }
}