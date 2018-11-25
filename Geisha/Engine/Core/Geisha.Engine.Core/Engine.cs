namespace Geisha.Engine.Core
{
    public interface IEngine
    {
        bool IsScheduledForShutdown { get; }
        void Update();
    }

    internal class Engine : IEngine
    {
        private readonly IEngineManager _engineManager;
        private readonly IGameLoop _gameLoop;

        public Engine(IGameLoop gameLoop, IEngineManager engineManager)
        {
            _gameLoop = gameLoop;
            _engineManager = engineManager;
        }

        public bool IsScheduledForShutdown => _engineManager.IsEngineScheduledForShutdown;

        public void Update()
        {
            _gameLoop.Update();
        }
    }
}