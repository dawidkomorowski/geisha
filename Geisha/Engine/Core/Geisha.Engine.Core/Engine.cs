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

        public Engine(IGameLoop gameLoop, IEngineManager engineManager, IRegisterDiagnosticInfoProvidersStartUpTask registerDiagnosticInfoProvidersStartUpTask,
            IRegisterAssetsAutomaticallyStarUpTask registerAssetsAutomaticallyStarUpTask)
        {
            _gameLoop = gameLoop;
            _engineManager = engineManager;
            _registerDiagnosticInfoProvidersStartUpTask = registerDiagnosticInfoProvidersStartUpTask;
            _registerAssetsAutomaticallyStarUpTask = registerAssetsAutomaticallyStarUpTask;

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
        }
    }
}