using System;
using System.Windows.Forms;
using Geisha.Common.Logging;

namespace Geisha.Engine.Launcher.WindowsForms
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            LogFactory.ConfigureFileTarget("GeishaEngine.log");
            var log = LogFactory.Create(typeof(Program));
            log.Info("Application is being started.");

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new GameWindow());

            log.Info("Application is being closed.");
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var exceptionObject = unhandledExceptionEventArgs.ExceptionObject;
            var log = LogFactory.Create(typeof(Program));
            log.Fatal(exceptionObject.ToString());
        }
    }
}