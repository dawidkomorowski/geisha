using Geisha.Engine.Core.Configuration;

namespace Geisha.Engine.Core.SceneModel
{
    // TODO Add documentation.
    public interface ISceneManager
    {
        Scene CurrentScene { get; }
    }

    internal class SceneManager : ISceneManager
    {
        public SceneManager(ISceneLoader sceneLoader, IConfigurationManager configurationManager, IStartUpTask startUpTask,
            ISceneConstructionScriptExecutor sceneConstructionScriptExecutor)
        {
            startUpTask.Run();
            // TODO How to register assets? Assets auto-discovery?
            var scene = sceneLoader.Load(configurationManager.GetConfiguration<CoreConfiguration>().StartUpScene);
            sceneConstructionScriptExecutor.Execute(scene);
            CurrentScene = scene;
        }

        public Scene CurrentScene { get; }
    }

    public interface IStartUpTask
    {
        void Run();
    }
}