using System.ComponentModel.Composition;
using Geisha.Engine.Core.Configuration;

namespace Geisha.Engine.Core.SceneModel
{
    public interface ISceneManager
    {
        Scene CurrentScene { get; }
    }

    // TODO It is only dummy implementation until some working proof of concept is running. Then actual logic should be implemented here and tests of it added.
    [Export(typeof(ISceneManager))]
    internal class SceneManager : ISceneManager
    {
        [ImportingConstructor]
        public SceneManager(ISceneLoader sceneLoader, IConfigurationManager configurationManager, IStartUpTask startUpTask)
        {
            startUpTask.Run();
            // TODO How to register assets? Assets auto-discovery?
            CurrentScene = sceneLoader.Load(configurationManager.GetConfiguration<CoreConfiguration>().StartUpScene);
        }

        public Scene CurrentScene { get; }
    }

    public interface IStartUpTask
    {
        void Run();
    }
}