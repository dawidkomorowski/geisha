using System;
using Geisha.Engine.Windows;

namespace Sandbox
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            WindowsApplication.Run(new SandboxApp());
        }
    }
}