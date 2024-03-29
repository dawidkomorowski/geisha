using System;
using Geisha.Engine.Windows;

namespace Geisha.Benchmark
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
            WindowsApplication.Run(new BenchmarkApp());
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Environment.FailFast(e.ExceptionObject.ToString());
        }
    }
}