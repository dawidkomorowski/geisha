using Geisha.Engine.Windows;
using System;

namespace Geisha.Engine.E2EApp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            WindowsApplication.UnhandledExceptionHandler = UnhandledExceptionHandler;
            WindowsApplication.Run(new TestApp());
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Environment.FailFast(e.ExceptionObject.ToString());
        }
    }
}