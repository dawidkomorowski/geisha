using System.Windows;
using Geisha.Editor.Core.ViewModels;

namespace Geisha.Editor.Core.Views
{
    public class GeishaEditorWindow : Window, IWindow
    {
        protected GeishaEditorWindow()
        {
        }

        public void ShowModalChildWindow(ViewModel dataContext)
        {
            var window = ViewResolver.ResolveWindowForViewModel(dataContext);
            window.DataContext = dataContext;
            window.Owner = this;
            window.ShowDialog();
        }
    }
}