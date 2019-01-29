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
        public SceneManager(ISceneLoader sceneLoader, IConfigurationManager configurationManager, IStartUpTask startUpTask)
        {
            startUpTask.Run();
            // TODO How to register assets? Assets auto-discovery?
            var scene = sceneLoader.Load(configurationManager.GetConfiguration<CoreConfiguration>().StartUpScene);
            var constructionScript = scene.ConstructionScript;
            constructionScript.Execute(scene);
            CurrentScene = scene;
        }

        public Scene CurrentScene { get; }
    }

    public interface IStartUpTask
    {
        void Run();
    }
}