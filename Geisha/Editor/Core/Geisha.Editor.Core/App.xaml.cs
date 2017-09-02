using System;
using System.Windows;
using Geisha.Common.Logging;

namespace Geisha.Editor.Core
{
    public partial class App : Application
    {
        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var exceptionObject = unhandledExceptionEventArgs.ExceptionObject;
            var log = LogFactory.Create(typeof(App));
            log.Fatal(exceptionObject.ToString());
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            LogFactory.ConfigureFileTarget("GeishaEditor.log");

            var log = LogFactory.Create(typeof(App));
            log.Info("Creating application container.");

            var applicationContainer = new ApplicationContainer();
            applicationContainer.Start();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            var log = LogFactory.Create(typeof(App));
            log.Info("Application is being closed.");
        }
    }
}