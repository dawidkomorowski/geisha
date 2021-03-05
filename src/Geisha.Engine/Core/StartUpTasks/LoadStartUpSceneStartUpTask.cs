using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.StartUpTasks
{
    internal sealed class LoadStartUpSceneStartUpTask : IStartUpTask
    {
        private readonly ISceneManager _sceneManager;
        private readonly CoreConfiguration _configuration;

        public LoadStartUpSceneStartUpTask(ISceneManager sceneManager, CoreConfiguration configuration)
        {
            _sceneManager = sceneManager;
            _configuration = configuration;
        }

        public void Run()
        {
            if (_configuration.StartUpScene != string.Empty)
            {
                _sceneManager.LoadScene(_configuration.StartUpScene);
            }
            else
            {
                _sceneManager.LoadEmptyScene(_configuration.StartUpSceneBehavior);
            }
        }
    }
}