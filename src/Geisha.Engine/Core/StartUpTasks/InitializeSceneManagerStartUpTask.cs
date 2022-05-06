using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.StartUpTasks
{
    internal sealed class InitializeSceneManagerStartUpTask : IStartUpTask
    {
        private readonly ISceneManagerInit _sceneManager;
        private readonly IEnumerable<ISceneObserver> _sceneObservers;

        public InitializeSceneManagerStartUpTask(ISceneManagerInit sceneManager, IEnumerable<ISceneObserver> sceneObservers)
        {
            _sceneManager = sceneManager;
            _sceneObservers = sceneObservers;
        }

        public void Run()
        {
            _sceneManager.Initialize(_sceneObservers);
        }
    }
}