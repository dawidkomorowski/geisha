using System;
using System.Drawing;
using System.Windows.Forms;
using Geisha.Common.Logging;
using Geisha.Engine.Audio.CSCore;
using Geisha.Engine.Input.Windows;
using Geisha.Engine.Rendering.DirectX;
using SharpDX.Windows;

namespace Geisha.Engine.Host.Windows
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

            using (var form = new RenderForm($"Geisha Engine {Application.ProductVersion}")
            {
                ClientSize = new Size(1280, 720),
                AllowUserResizing = false
            })
            {
                var engineBuilder = new EngineBuilder()
                    .UseAudioBackend(new CSCoreAudioBackend())
                    .UseInputBackend(new WindowsInputBackend(form))
                    .UseRenderingBackend(new DirectXRenderingBackend(form));

                using var engine = engineBuilder.Build();

                RenderLoop.Run(form, () =>
                {
                    // ReSharper disable AccessToDisposedClosure
                    engine.Update();

                    if (engine.IsScheduledForShutdown) form.Close();
                    // ReSharper restore AccessToDisposedClosure
                });
            }

            log.Info("Application is being closed.");
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var exceptionObject = unhandledExceptionEventArgs.ExceptionObject;
            var log = LogFactory.Create(typeof(Program));
            log.Fatal(exceptionObject.ToString() ?? "No exception info.");

            MessageBox.Show("Fatal error occured during engine execution. See GeishaEngine.log file for details.", "Geisha Engine Fatal Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}