using Geisha.Engine.Core.StartUpTasks;

namespace Geisha.Engine.Core
{
    public interface IEngine
    {
        bool IsScheduledForShutdown { get; }
        void Update();
    }

    internal sealed class Engine : IEngine
    {
        private readonly IEngineManager _engineManager;
        private readonly IGameLoop _gameLoop;
        private readonly IRegisterDiagnosticInfoProvidersStartUpTask _registerDiagnosticInfoProvidersStartUpTask;
        private readonly IRegisterAssetsAutomaticallyStarUpTask _registerAssetsAutomaticallyStarUpTask;
        private readonly ILoadStartUpSceneStartUpTask _loadStartUpSceneStartUpTask;

        public Engine(IGameLoop gameLoop, IEngineManager engineManager, IRegisterDiagnosticInfoProvidersStartUpTask registerDiagnosticInfoProvidersStartUpTask,
            IRegisterAssetsAutomaticallyStarUpTask registerAssetsAutomaticallyStarUpTask, ILoadStartUpSceneStartUpTask loadStartUpSceneStartUpTask)
        {
            _gameLoop = gameLoop;
            _engineManager = engineManager;
            _registerDiagnosticInfoProvidersStartUpTask = registerDiagnosticInfoProvidersStartUpTask;
            _registerAssetsAutomaticallyStarUpTask = registerAssetsAutomaticallyStarUpTask;
            _loadStartUpSceneStartUpTask = loadStartUpSceneStartUpTask;

            RunStartUpTasks();
        }

        public bool IsScheduledForShutdown => _engineManager.IsEngineScheduledForShutdown;

        public void Update()
        {
            _gameLoop.Update();
        }

        private void RunStartUpTasks()
        {
            _registerDiagnosticInfoProvidersStartUpTask.Run();
            _registerAssetsAutomaticallyStarUpTask.Run();
            _loadStartUpSceneStartUpTask.Run();
        }
    }
}