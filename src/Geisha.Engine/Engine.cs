using System;
using Autofac;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.Logging;
using Geisha.Engine.Core.StartUpTasks;
using Geisha.Engine.Input.Backend;
using Geisha.Engine.Physics;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;

namespace Geisha.Engine
{
    public sealed class Engine : IDisposable
    {
        private static readonly ILog Log = LogFactory.Create(typeof(Engine));
        private readonly IContainer _container;
        private readonly ILifetimeScope _lifetimeScope;

        private readonly IGameLoop _gameLoop;
        private readonly IEngineManager _engineManager;

        public Engine(
            Configuration configuration,
            IAudioBackend audioBackend,
            IInputBackend inputBackend,
            IRenderingBackend renderingBackend,
            IGame game)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (audioBackend == null) throw new ArgumentNullException(nameof(audioBackend));
            if (inputBackend == null) throw new ArgumentNullException(nameof(inputBackend));
            if (renderingBackend == null) throw new ArgumentNullException(nameof(renderingBackend));
            if (game == null) throw new ArgumentNullException(nameof(game));

            Log.Info("Initializing engine components.");
            var containerBuilder = new ContainerBuilder();

            EngineModules.RegisterAll(containerBuilder);

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

            RunStartUpTasks();

            _gameLoop = _lifetimeScope.Resolve<IGameLoop>();
            _engineManager = _lifetimeScope.Resolve<IEngineManager>();
            Log.Info("Engine components initialized.");
        }

        public bool IsScheduledForShutdown => _engineManager.IsEngineScheduledForShutdown;

        public void Update()
        {
            _gameLoop.Update();
        }

        public void Dispose()
        {
            Log.Info("Disposing engine components.");
            _lifetimeScope.Dispose();
            _container.Dispose();
            Log.Info("Engine components disposed.");
        }

        private void RunStartUpTasks()
        {
            Run<InitializeSceneBehaviorFactoryProviderStartUpTask>();
            Run<InitializeSceneManagerStartUpTask>();
            Run<RegisterDiagnosticInfoProvidersStartUpTask>();
            Run<RegisterAssetsAutomaticallyStartUpTask>();
            Run<LoadStartUpSceneStartUpTask>();
        }

        private void Run<TStartUpTask>() where TStartUpTask : IStartUpTask
        {
            var startUpTask = _lifetimeScope.Resolve<TStartUpTask>();
            startUpTask.Run();
        }
    }
}