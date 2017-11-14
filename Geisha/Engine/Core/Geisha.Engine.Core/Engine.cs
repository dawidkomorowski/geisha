using System.ComponentModel.Composition;

namespace Geisha.Engine.Core
{
    public interface IEngine
    {
        bool Update();
    }

    [Export(typeof(IEngine))]
    public class Engine : IEngine
    {
        private readonly IEngineManager _engineManager;
        private readonly IGameLoop _gameLoop;

        [ImportingConstructor]
        public Engine(IGameLoop gameLoop, IEngineManager engineManager)
        {
            _gameLoop = gameLoop;
            _engineManager = engineManager;
        }

        public bool Update()
        {
            _gameLoop.Update();
            return !_engineManager.IsEngineScheduledForShutdown;
        }
    }
}