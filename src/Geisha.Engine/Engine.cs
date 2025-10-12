using System;
using System.Runtime.InteropServices;
using Autofac;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Backend;
using Geisha.Engine.Physics;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using NLog;

namespace Geisha.Engine
{
    /// <summary>
    ///     Main engine class. It configures engine systems and components and initializes game loop.
    /// </summary>
    /// <remarks>
    ///     This class should not be directly used unless you are creating custom entry point to your application, and you
    ///     need full control over window initialization and backend implementations being used.
    /// </remarks>
    public sealed class Engine : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Configuration _configuration;
        private readonly IContainer _container;

        private readonly IGameLoop _gameLoop;
        private readonly IEngineManager _engineManager;

        /// <summary>
        ///     Creates new instance of <see cref="Engine" /> with specified configuration.
        /// </summary>
        /// <param name="configuration">Engine configuration.</param>
        /// <param name="audioBackend">Audio backend to be used by engine.</param>
        /// <param name="inputBackend">Input backend to be used by engine.</param>
        /// <param name="renderingBackend">Rendering backend to be used by engine.</param>
        /// <param name="game"><see cref="Game" /> instance to be run by engine.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of parameters is null.</exception>
        public Engine(
            Configuration configuration,
            IAudioBackend audioBackend,
            IInputBackend inputBackend,
            IRenderingBackend renderingBackend,
            Game game)
        {
            if (audioBackend == null) throw new ArgumentNullException(nameof(audioBackend));
            if (inputBackend == null) throw new ArgumentNullException(nameof(inputBackend));
            if (renderingBackend == null) throw new ArgumentNullException(nameof(renderingBackend));
            if (game == null) throw new ArgumentNullException(nameof(game));

            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            Logger.Info("Initializing engine components.");

            if (_configuration.Core.EnableGCLogging)
            {
                GCEventListener.Start();
            }

            LogEnvironmentInfo(renderingBackend.Info);

            var containerBuilder = new ContainerBuilder();

            EngineModules.RegisterAll(containerBuilder);

            containerBuilder.RegisterInstance(_configuration.Audio).As<AudioConfiguration>().SingleInstance();
            containerBuilder.RegisterInstance(_configuration.Core).As<CoreConfiguration>().SingleInstance();
            containerBuilder.RegisterInstance(_configuration.Physics).As<PhysicsConfiguration>().SingleInstance();
            containerBuilder.RegisterInstance(_configuration.Rendering).As<RenderingConfiguration>().SingleInstance();

            containerBuilder.RegisterInstance(audioBackend).As<IAudioBackend>().SingleInstance();
            containerBuilder.RegisterInstance(inputBackend).As<IInputBackend>().SingleInstance();
            containerBuilder.RegisterInstance(renderingBackend).As<IRenderingBackend>().SingleInstance();

            var componentsRegistry = new ComponentsRegistry(containerBuilder);
            game.RegisterComponents(componentsRegistry);

            _container = containerBuilder.Build();

            ConfigureAudioBackend();
            RegisterAssets();
            LoadStartUpScene();

            _gameLoop = _container.Resolve<IGameLoop>();
            _engineManager = _container.Resolve<IEngineManager>();
            Logger.Info("Engine components initialized.");
        }

        /// <summary>
        ///     True if engine is scheduled for shutdown, otherwise false.
        /// </summary>
        public bool IsScheduledForShutdown => _engineManager.IsEngineScheduledForShutdown;

        /// <summary>
        ///     Executes one frame of game loop.
        /// </summary>
        public void Update()
        {
            _gameLoop.Update();
        }

        /// <summary>
        ///     Disposes engine systems and components.
        /// </summary>
        public void Dispose()
        {
            Logger.Info("Disposing engine components.");
            _container.Dispose();

            if (_configuration.Core.EnableGCLogging)
            {
                GCEventListener.Stop();
            }

            Logger.Info("Engine components disposed.");
        }

        private void ConfigureAudioBackend()
        {
            var audioBackend = _container.Resolve<IAudioBackend>();

            audioBackend.AudioPlayer.EnableSound = _configuration.Audio.EnableSound;
            audioBackend.AudioPlayer.Volume = _configuration.Audio.Volume;
        }

        private void RegisterAssets()
        {
            var assetStore = _container.Resolve<IAssetStore>();

            assetStore.RegisterAssets(_configuration.Core.AssetsRootDirectoryPath);
        }

        private void LoadStartUpScene()
        {
            var sceneManager = _container.Resolve<ISceneManager>();

            if (_configuration.Core.StartUpScene != string.Empty)
            {
                sceneManager.LoadScene(_configuration.Core.StartUpScene);
            }
            else
            {
                sceneManager.LoadEmptyScene(_configuration.Core.StartUpSceneBehavior);
            }
        }

        private static void LogEnvironmentInfo(RenderingBackendInfo renderingBackendInfo)
        {
            Logger.Info("Environment Info:");
            Logger.Info("  {0,-22} {1}", "Operating System:", Environment.OSVersion);
            Logger.Info("  {0,-22} {1}", ".NET Version:", Environment.Version);
            Logger.Info("  {0,-22} {1}", "Machine Name:", Environment.MachineName);
            Logger.Info("  {0,-22} {1}", "OS Architecture:", RuntimeInformation.OSArchitecture);
            Logger.Info("  {0,-22} {1}", "Process Architecture:", RuntimeInformation.ProcessArchitecture);
            Logger.Info("  {0,-22} {1}", "Processor Count:", Environment.ProcessorCount);
            Logger.Info("  {0,-22} {1}", "64-bit OS:", Environment.Is64BitOperatingSystem);
            Logger.Info("  {0,-22} {1}", "64-bit Process:", Environment.Is64BitProcess);
            Logger.Info("  {0,-22} {1}", "Rendering Backend:", renderingBackendInfo.Name);
            Logger.Info("  {0,-22} {1}", "Adapter:", renderingBackendInfo.GraphicsAdapterName);
            Logger.Info("  {0,-22} {1}", "VideoMemorySize:", renderingBackendInfo.VideoMemorySize);
            Logger.Info("  {0,-22} {1}", "VideoMemorySizeGB:", renderingBackendInfo.VideoMemorySizeGB);
            Logger.Info("  {0,-22} {1}", "Feature Level:", renderingBackendInfo.FeatureLevel);
        }
    }
}