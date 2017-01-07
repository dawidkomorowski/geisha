using System.ComponentModel.Composition;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core
{
    [Export(typeof(IGameLoop))]
    public class GameLoop : IGameLoop
    {
        private readonly ISystemsProvider _systemsProvider;
        private readonly IDeltaTimeProvider _deltaTimeProvider;
        private readonly ISceneManager _sceneManager;

        [ImportingConstructor]
        public GameLoop(ISystemsProvider systemsProvider, IDeltaTimeProvider deltaTimeProvider,
            ISceneManager sceneManager)
        {
            _systemsProvider = systemsProvider;
            _deltaTimeProvider = deltaTimeProvider;
            _sceneManager = sceneManager;
        }

        public void Update()
        {
            var deltaTime = _deltaTimeProvider.GetDeltaTime();
            var systems = _systemsProvider.GetSystemsUpdatableForScene(_sceneManager.CurrentScene);
            systems.Update(deltaTime);
        }
    }
}