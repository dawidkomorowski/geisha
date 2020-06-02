using System;
using Autofac;
using Geisha.Common;
using Geisha.Common.Logging;
using Geisha.Engine.Audio;
using Geisha.Engine.Core;
using Geisha.Engine.Core.StartUpTasks;
using Geisha.Engine.Input;
using Geisha.Engine.Rendering;

namespace Geisha.Engine
{
    public interface IEngine : IDisposable
    {
        bool IsScheduledForShutdown { get; }
        void Update();
    }

    internal sealed class Engine : IEngine
    {
        private static readonly ILog Log = LogFactory.Create(typeof(Engine));
        private readonly IContainer _container;
        private readonly ILifetimeScope _lifetimeScope;

        private readonly IGameLoop _gameLoop;
        private readonly IEngineManager _engineManager;

        public Engine(
            IAudioBackend audioBackend,
            IInputBackend inputBackend,
            IRenderingBackend renderingBackend,
            IGame game)
        {
            Log.Info("Starting engine.");
            var containerBuilder = new ContainerBuilder();

            CommonModules.RegisterAll(containerBuilder);
            EngineModules.RegisterAll(containerBuilder);

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
            Log.Info("Engine started successfully.");
        }

        public bool IsScheduledForShutdown => _engineManager.IsEngineScheduledForShutdown;

        public void Update()
        {
            _gameLoop.Update();
        }

        public void Dispose()
        {
            Log.Info("Disposing engine components.");
            _lifetimeScope?.Dispose();
            _container?.Dispose();
            Log.Info("Engine components disposed.");
        }

        private void RunStartUpTasks()
        {
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