using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.StartUpTasks
{
    internal interface ILoadStartUpSceneStartUpTask
    {
        void Run();
    }

    internal sealed class LoadStartUpSceneStartUpTask : ILoadStartUpSceneStartUpTask
    {
        private readonly ISceneManager _sceneManager;
        private readonly IConfigurationManager _configurationManager;

        public LoadStartUpSceneStartUpTask(ISceneManager sceneManager, IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
            _sceneManager = sceneManager;
        }

        public void Run()
        {
            var configuration = _configurationManager.GetConfiguration<CoreConfiguration>();
            _sceneManager.LoadScene(configuration.StartUpScene);
        }
    }
}