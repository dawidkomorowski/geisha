using System.Windows;

namespace Geisha.Editor.Core
{
    public partial class App : Application
    {
        private readonly ApplicationContainer _applicationContainer;

        public App()
        {
            _applicationContainer = new ApplicationContainer();
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var mainViewModel = _applicationContainer.CreateMainViewModel();
            var mainWindow = new Views.MainWindow.MainWindow {DataContext = mainViewModel};
            mainWindow.Show();
        }
    }
}