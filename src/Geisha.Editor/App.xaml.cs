using System;
using System.Windows;
using Geisha.Common.Extensibility;
using Geisha.Common.Logging;
using Geisha.Editor.Core.ViewModels.MainWindow;
using Geisha.Editor.Core.Views.MainWindow;

namespace Geisha.Editor
{
    public partial class App : Application
    {
        private ExtensionsHostContainer<MainViewModel> _extensionsHostContainer;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            LogFactory.ConfigureFileTarget("GeishaEditor.log");

            var log = LogFactory.Create(typeof(App));
            log.Info("Application is being started.");

            _extensionsHostContainer = new ExtensionsHostContainer<MainViewModel>(new HostServices());
            var mainViewModel = _extensionsHostContainer.CompositionRoot;
            var mainWindow = new MainWindow {DataContext = mainViewModel};
            mainWindow.Show();

            log.Info("Application started successfully.");
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            _extensionsHostContainer.Dispose();

            var log = LogFactory.Create(typeof(App));
            log.Info("Application is being closed.");
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var exceptionObject = unhandledExceptionEventArgs.ExceptionObject;
            var log = LogFactory.Create(typeof(App));
            log.Fatal(exceptionObject.ToString());

            MessageBox.Show("Fatal error occured during editor execution. See GeishaEditor.log file for details.", "Geisha Editor Fatal Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}