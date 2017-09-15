using System.ComponentModel.Composition;

namespace Geisha.Engine.Core.SceneModel
{
    public interface ISceneManager
    {
        Scene CurrentScene { get; }
    }

    // TODO It is only dummy implementation until some working proof of concept is running. Then actual logic should be implemented here and tests of it added.
    [Export(typeof(ISceneManager))]
    public class SceneManager : ISceneManager
    {
        [ImportingConstructor]
        public SceneManager(ITestSceneProvider testSceneProvider)
        {
            CurrentScene = testSceneProvider.GetTestScene();
        }

        public Scene CurrentScene { get; }
    }
}