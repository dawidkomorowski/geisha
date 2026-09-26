using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using Geisha.Engine.Audio.NAudio;
using Geisha.Engine.Core.Logging;
using Geisha.Engine.Input.Windows;
using Geisha.Engine.Rendering.DirectX;
using Geisha.Engine.Windowing.Windows;
using NLog;

namespace Geisha.Engine.Windows;

/// <summary>
///     Provides default setup of Geisha Engine for Windows platform.
/// </summary>
/// <remarks>Window icon is derived from executable. To set custom window icon specify the icon for game executable.</remarks>
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
    public static void Run(Game game) => Run(game, new WindowsApplicationOptions());

    /// <summary>
    ///     Initializes Geisha Engine for specified <paramref name="game" /> with the specified platform options and starts
    ///     the game loop.
    /// </summary>
    /// <param name="game"><see cref="Game" /> instance providing custom game functionality.</param>
    /// <param name="options">Options that configure Windows- and DirectX-specific engine behavior.</param>
    /// <remarks>
    ///     Use this overload to configure platform-specific behavior that is not represented by <see cref="Configuration" />.
    ///     Passing a new <see cref="WindowsApplicationOptions" /> uses the default behavior.
    /// </remarks>
    public static void Run(Game game, WindowsApplicationOptions options)
    {
        AppDomain.CurrentDomain.UnhandledException += InternalUnhandledExceptionHandler;

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

        using var windowingBackend = new WindowsWindowingBackend();
        using var renderingBackend = new DirectXRenderingBackend(windowingBackend.Window, DriverType.Hardware);
        using var audioBackend = new NAudioAudioBackend();
        var inputBackend = new WindowsInputBackend(windowingBackend.Window);

        renderingBackend.ResizeBuffersAfterVSyncChange = options.DirectX.ResizeBuffersAfterVSyncChange;

        using var engine = new Engine(
            configuration,
            audioBackend,
            inputBackend,
            renderingBackend,
            windowingBackend,
            game
        );

        logger.Info("Engine started successfully.");

        engine.Run();

        logger.Info("Engine shutdown completed.");
    }

    private static void InternalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
    {
        var exceptionObject = unhandledExceptionEventArgs.ExceptionObject;
        var logger = LogManager.GetCurrentClassLogger();
        logger.Fatal(exceptionObject.ToString());

        UnhandledExceptionHandler(sender, unhandledExceptionEventArgs);
    }

    private static void DefaultUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
    {
        MessageBox.Show($"A fatal error has occurred while the engine was running. See {LogFile} file for details.", "Geisha Engine Fatal Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}