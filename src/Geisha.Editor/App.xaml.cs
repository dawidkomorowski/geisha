using System;
using System.Windows;
using Autofac;
using Geisha.Common.Logging;

namespace Geisha.Editor
{
    public partial class App : Application
    {
        private IContainer _container;
        private ILifetimeScope _lifetimeScope;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            LogFactory.ConfigureFileTarget("GeishaEditor.log");

            var log = LogFactory.Create(typeof(App));
            log.Info("Geisha Editor is being started.");

            var containerBuilder = new ContainerBuilder();

            EditorModules.RegisterAll(containerBuilder);

            _container = containerBuilder.Build();
            _lifetimeScope = _container.BeginLifetimeScope();

            var mainViewModel = _lifetimeScope.Resolve<MainViewModel>();
            var mainWindow = new MainWindow {DataContext = mainViewModel};
            mainWindow.Show();

            log.Info("Geisha Editor started successfully.");
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            var log = LogFactory.Create(typeof(App));

            log.Info("Disposing editor components.");
            _lifetimeScope?.Dispose();
            _container?.Dispose();
            log.Info("Editor components disposed.");

            log.Info("Geisha Editor is being closed.");
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