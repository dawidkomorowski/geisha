namespace Geisha.Engine.Core.SceneModel
{
    // TODO It is only dummy implementation until some working proof of concept is running. Then actual logic should be implemented here and tests of it added.
    public class SceneManager : ISceneManager
    {
        public Scene CurrentScene { get; }

        public SceneManager(Scene scene)
        {
            CurrentScene = scene;
        }
    }
}