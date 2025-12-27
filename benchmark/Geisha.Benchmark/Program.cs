using System;
using System.Diagnostics;
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
            var p = Process.GetCurrentProcess();
            p.PriorityClass = ProcessPriorityClass.High; // elevate scheduling priority
            p.ProcessorAffinity = (IntPtr)0b_0101; // pin to CPU cores

            WindowsApplication.UnhandledExceptionHandler = UnhandledExceptionHandler;
            WindowsApplication.Run(new BenchmarkApp());
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Environment.FailFast(e.ExceptionObject.ToString());
        }
    }
}