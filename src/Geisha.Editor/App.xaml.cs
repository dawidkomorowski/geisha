using System;
using System.Windows;
using Autofac;
using Geisha.Editor.Core;
using Geisha.Engine;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Audio.CSCore;
using Geisha.Engine.Core.Logging;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.DirectX;
using NLog;

namespace Geisha.Editor
{
    public partial class App : Application
    {
        private const string LogFile = "GeishaEngine.log";
        private IContainer? _container;
        private ILifetimeScope? _lifetimeScope;
        private MainWindow? _mainWindow;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            BindingErrorListener.EnableExceptionOnBindingError();

            LogHelper.ConfigureFileTarget(LogFile);

            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("Geisha Editor is being started.");

            var containerBuilder = new ContainerBuilder();

            EngineModules.RegisterAll(containerBuilder);
            EditorModules.RegisterAll(containerBuilder);

            containerBuilder.RegisterInstance(new CSCoreAudioBackend()).As<IAudioBackend>();
            containerBuilder.RegisterInstance(new DirectXRenderingBackend()).As<IRenderingBackend>();

            _container = containerBuilder.Build();
            _lifetimeScope = _container.BeginLifetimeScope();

            ViewRepository.Default.RegisterViewsFromCurrentlyLoadedAssemblies();

            var mainViewModel = _lifetimeScope.Resolve<MainViewModel>();
            _mainWindow = new MainWindow(mainViewModel);
            _mainWindow.Show();
            _mainWindow.LoadLayout();

            logger.Info("Geisha Editor started successfully.");
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            var logger = LogManager.GetCurrentClassLogger();

            _mainWindow?.SaveLayout();

            logger.Info("Disposing editor components.");
            _lifetimeScope?.Dispose();
            _container?.Dispose();
            logger.Info("Editor components disposed.");

            logger.Info("Geisha Editor is being closed.");
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var exceptionObject = unhandledExceptionEventArgs.ExceptionObject;
            var exceptionInfo = exceptionObject.ToString() ?? "No exception info.";

            var logger = LogManager.GetCurrentClassLogger();
            logger.Fatal(exceptionInfo);

            MessageBox.Show($"Fatal error occurred during editor execution. See {LogFile} file for details.", "Geisha Editor Fatal Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}