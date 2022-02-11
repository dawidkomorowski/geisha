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
            log.Info("Starting engine.");

            log.Info("Loading configuration from file.");
            var configuration = Configuration.LoadFromFile("game.json");

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            using (var form = new RenderForm(game.WindowTitle)
                   {
                       ClientSize = new Size(configuration.Rendering.ScreenWidth, configuration.Rendering.ScreenHeight),
                       AllowUserResizing = false
                   })
            {
                using var engine = new Engine(
                    configuration,
                    new CSCoreAudioBackend(),
                    new WindowsInputBackend(form),
                    new DirectXRenderingBackend(form, DriverType.Hardware),
                    game
                );

                log.Info("Engine started successfully.");

                RenderLoop.Run(form, () =>
                {
                    // ReSharper disable AccessToDisposedClosure
                    engine.Update();

                    if (engine.IsScheduledForShutdown) form.Close();
                    // ReSharper restore AccessToDisposedClosure
                });
            }

            log.Info("Engine shutdown completed.");
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var exceptionObject = unhandledExceptionEventArgs.ExceptionObject;
            var log = LogFactory.Create(typeof(GeishaEngineForWindows));
            log.Fatal(exceptionObject.ToString() ?? "No exception info.");

            MessageBox.Show("A fatal error has occurred while the engine was running. See GeishaEngine.log file for details.", "Geisha Engine Fatal Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}