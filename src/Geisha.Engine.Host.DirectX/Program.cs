﻿using System;
using System.Drawing;
using System.Windows.Forms;
using Geisha.Common.Logging;
using Geisha.Engine.Input.Windows;
using SharpDX.Windows;

namespace Geisha.Engine.Host.DirectX
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            LogFactory.ConfigureFileTarget("GeishaEngine.log");

            var log = LogFactory.Create(typeof(Program));
            log.Info("Application is being started.");

            using (var form = new RenderForm($"Geisha Engine {Application.ProductVersion}") {ClientSize = new Size(1280, 720)})
            {
                var window = new Window(form);
                var hostServices = new HostServices(window);

                var engineBuilder = new EngineBuilder();
                engineBuilder.UseInputBackend(new WindowsInputBackend(form));
                engineBuilder.UseHostServices(hostServices);

                using (var engine = engineBuilder.Build())
                {
                    RenderLoop.Run(form, () =>
                    {
                        engine.Update();

                        if (engine.IsScheduledForShutdown) form.Close();
                    });
                }
            }

            log.Info("Application is being closed.");
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var exceptionObject = unhandledExceptionEventArgs.ExceptionObject;
            var log = LogFactory.Create(typeof(Program));
            log.Fatal(exceptionObject.ToString());

            MessageBox.Show("Fatal error occured during engine execution. See GeishaEngine.log file for details.", "Geisha Engine Fatal Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}