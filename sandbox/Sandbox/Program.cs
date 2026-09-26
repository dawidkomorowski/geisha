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
            var options = new WindowsApplicationOptions
            {
                DirectX = new DirectXOptions
                {
                    ResizeBuffersAfterVSyncChange = true
                }
            };

            WindowsApplication.Run(new SandboxApp(), options);
        }
    }
}