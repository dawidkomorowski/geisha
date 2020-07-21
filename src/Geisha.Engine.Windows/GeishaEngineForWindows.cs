using System;
using System.Drawing;
using System.Windows.Forms;
using Geisha.Common.Logging;
using Geisha.Engine.Audio.CSCore;
using Geisha.Engine.Input.Windows;
using Geisha.Engine.Rendering.DirectX;
using SharpDX.Windows;

namespace Geisha.Engine.Windows
{
    /// <summary>
    ///     Provides default setup of Geisha Engine for Windows platform.
    /// </summary>
    public static class GeishaEngineForWindows
    {
        public static void Run(IGame game)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            LogFactory.ConfigureFileTarget("GeishaEngine.log");

            var log = LogFactory.Create(typeof(GeishaEngineForWindows));
            log.Info("Application is being started.");

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            using (var form = new RenderForm(game.WindowTitle)
            {
                //ClientSize = new Size(1280, 720),
                ClientSize = new Size(2560, 1440),
                AllowUserResizing = false
            })
            {
                using var engine = new Engine(
                    new CSCoreAudioBackend(),
                    new WindowsInputBackend(form),
                    new DirectXRenderingBackend(form),
                    game
                );

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
            var log = LogFactory.Create(typeof(GeishaEngineForWindows));
            log.Fatal(exceptionObject.ToString() ?? "No exception info.");

            MessageBox.Show("Fatal error occured during engine execution. See GeishaEngine.log file for details.", "Geisha Engine Fatal Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}