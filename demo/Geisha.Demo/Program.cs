using System;
using Geisha.Engine.Windows;

namespace Geisha.Demo
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            WindowsApplication.Run(new DemoApp());
        }
    }
}