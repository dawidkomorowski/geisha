using System;
using Autofac;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
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
    ///     This class should not be directly used unless you are creating custom entry point to your application and you
    ///     need full control over window initialization and backend implementations being used.
    /// </remarks>
    public sealed class Engine : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IContainer _container;
        private readonly ILifetimeScope _lifetimeScope;

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
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (audioBackend == null) throw new ArgumentNullException(nameof(audioBackend));
            if (inputBackend == null) throw new ArgumentNullException(nameof(inputBackend));
            if (renderingBackend == null) throw new ArgumentNullException(nameof(renderingBackend));
            if (game == null) throw new ArgumentNullException(nameof(game));

            Logger.Info("Initializing engine components.");
            var containerBuilder = new ContainerBuilder();

            EngineModules.RegisterAll(containerBuilder);

            containerBuilder.RegisterInstance(configuration.Audio).As<AudioConfiguration>().SingleInstance();
            containerBuilder.RegisterInstance(configuration.Core).As<CoreConfiguration>().SingleInstance();
            containerBuilder.RegisterInstance(configuration.Physics).As<PhysicsConfiguration>().SingleInstance();
            containerBuilder.RegisterInstance(configuration.Rendering).As<RenderingConfiguration>().SingleInstance();

            containerBuilder.RegisterInstance(audioBackend).As<IAudioBackend>().SingleInstance();
            containerBuilder.RegisterInstance(inputBackend).As<IInputBackend>().SingleInstance();
            containerBuilder.RegisterInstance(renderingBackend).As<IRenderingBackend>().SingleInstance();

            var componentsRegistry = new ComponentsRegistry(containerBuilder);
            game.RegisterComponents(componentsRegistry);

            _container = containerBuilder.Build();
            _lifetimeScope = _container.BeginLifetimeScope();

            ConfigureAudioBackend();
            RegisterAssets();
            LoadStartUpScene();

            _gameLoop = _lifetimeScope.Resolve<IGameLoop>();
            _engineManager = _lifetimeScope.Resolve<IEngineManager>();
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
            _lifetimeScope.Dispose();
            _container.Dispose();
            Logger.Info("Engine components disposed.");
        }

        private void ConfigureAudioBackend()
        {
            var audioBackend = _lifetimeScope.Resolve<IAudioBackend>();
            var audioConfiguration = _lifetimeScope.Resolve<AudioConfiguration>();

            audioBackend.AudioPlayer.EnableSound = audioConfiguration.EnableSound;
        }

        private void RegisterAssets()
        {
            var assetStore = _lifetimeScope.Resolve<IAssetStore>();
            var coreConfiguration = _lifetimeScope.Resolve<CoreConfiguration>();

            assetStore.RegisterAssets(coreConfiguration.AssetsRootDirectoryPath);
        }

        private void LoadStartUpScene()
        {
            var sceneManager = _lifetimeScope.Resolve<ISceneManager>();
            var coreConfiguration = _lifetimeScope.Resolve<CoreConfiguration>();

            if (coreConfiguration.StartUpScene != string.Empty)
            {
                sceneManager.LoadScene(coreConfiguration.StartUpScene);
            }
            else
            {
                sceneManager.LoadEmptyScene(coreConfiguration.StartUpSceneBehavior);
            }
        }
    }
}