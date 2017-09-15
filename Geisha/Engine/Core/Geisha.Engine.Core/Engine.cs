using System.ComponentModel.Composition;

namespace Geisha.Engine.Core
{
    public interface IEngine
    {
        void Update();
    }

    [Export(typeof(IEngine))]
    public class Engine : IEngine
    {
        private readonly IGameLoop _gameLoop;

        [ImportingConstructor]
        public Engine(IGameLoop gameLoop)
        {
            _gameLoop = gameLoop;
        }

        public void Update()
        {
            _gameLoop.Update();
        }
    }
}