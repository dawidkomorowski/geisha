using System.ComponentModel.Composition;
using Geisha.Engine.Core.Configuration;
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
        private readonly IConfigurationManager _configurationManager;

        private double _accumulator = 0;

        [ImportingConstructor]
        public GameLoop(ISystemsProvider systemsProvider, IDeltaTimeProvider deltaTimeProvider,
            ISceneManager sceneManager, IConfigurationManager configurationManager)
        {
            _systemsProvider = systemsProvider;
            _deltaTimeProvider = deltaTimeProvider;
            _sceneManager = sceneManager;
            _configurationManager = configurationManager;
        }

        public void Update()
        {
            var scene = _sceneManager.CurrentScene;
            var deltaTime = _deltaTimeProvider.GetDeltaTime();
            var variableUpdateSystems = _systemsProvider.GetVariableUpdateSystems();
            var fixedUpdateSystems = _systemsProvider.GetFixedUpdateSystems();
            var fixedTimeStep = _configurationManager.FixedDeltaTime;

            _accumulator += deltaTime;

            while (_accumulator >= fixedTimeStep)
            {
                foreach (var system in fixedUpdateSystems)
                {
                    system.FixedUpdate(scene);
                }

                _accumulator -= fixedTimeStep;
            }

            foreach (var system in variableUpdateSystems)
            {
                system.Update(scene, deltaTime);
            }
        }
    }
}