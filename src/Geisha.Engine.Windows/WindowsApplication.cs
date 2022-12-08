using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using Geisha.Engine.Audio.NAudio;
using Geisha.Engine.Core.Logging;
using Geisha.Engine.Input.Windows;
using Geisha.Engine.Rendering.DirectX;
using NLog;
using SharpDX.Windows;

namespace Geisha.Engine.Windows
{
    /// <summary>
    ///     Provides default setup of Geisha Engine for Windows platform.
    /// </summary>
    public static class WindowsApplication
    {
        private const string EngineConfigFile = "engine-config.json";
        private const string LogFile = "GeishaEngine.log";

        /// <summary>
        ///     Gets or sets handler to be used for unhandled exceptions.
        /// </summary>
        /// <remarks>Default handler shows the message box with information about fatal error and points to log file for details.</remarks>
        public static UnhandledExceptionEventHandler UnhandledExceptionHandler { get; set; } = DefaultUnhandledExceptionHandler;

        /// <summary>
        ///     Initializes Geisha Engine for specified <paramref name="game" /> and starts the game loop.
        /// </summary>
        /// <param name="game"><see cref="Game" /> instance providing custom game functionality.</param>
        public static void Run(Game game)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            LogHelper.ConfigureFileTarget(LogFile);

            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("Starting engine.");

            var configuration = Configuration.CreateDefault();
            if (File.Exists(EngineConfigFile))
            {
                logger.Info("Loading configuration from file.");
                configuration = Configuration.LoadFromFile(EngineConfigFile);
            }
            else
            {
                logger.Info("Configuration file does not exist. Using default configuration.");
            }

            configuration = configuration.Overwrite(game);

            LogHelper.SetLogLevel(configuration.Core.LogLevel);

            logger.Debug("Effective configuration:{0}{1}",
                Environment.NewLine,
                JsonSerializer.Serialize(configuration, new JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter() } }));

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            using (var form = new RenderForm(game.WindowTitle)
                   {
                       ClientSize = new Size(configuration.Rendering.ScreenWidth, configuration.Rendering.ScreenHeight),
                       AllowUserResizing = false
                   })
            {
                using var engine = new Engine(
                    configuration,
                    new NAudioAudioBackend(),
                    new WindowsInputBackend(form),
                    new DirectXRenderingBackend(form, DriverType.Hardware),
                    game
                );

                logger.Info("Engine started successfully.");

                RenderLoop.Run(form, () =>
                {
                    // ReSharper disable AccessToDisposedClosure
                    engine.Update();

                    if (engine.IsScheduledForShutdown) form.Close();
                    // ReSharper restore AccessToDisposedClosure
                });
            }

            logger.Info("Engine shutdown completed.");
        }

        private static void DefaultUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var exceptionObject = unhandledExceptionEventArgs.ExceptionObject;
            var logger = LogManager.GetCurrentClassLogger();
            logger.Fatal(exceptionObject.ToString());

            MessageBox.Show($"A fatal error has occurred while the engine was running. See {LogFile} file for details.", "Geisha Engine Fatal Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}