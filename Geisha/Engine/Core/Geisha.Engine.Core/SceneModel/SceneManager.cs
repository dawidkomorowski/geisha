namespace Geisha.Engine.Core.SceneModel
{
    // TODO Add documentation.
    public interface ISceneManager
    {
        Scene CurrentScene { get; }

        void LoadScene(string path);
    }

    internal class SceneManager : ISceneManager
    {
        private readonly ISceneConstructionScriptExecutor _sceneConstructionScriptExecutor;
        private readonly ISceneLoader _sceneLoader;

        public SceneManager(ISceneLoader sceneLoader, ISceneConstructionScriptExecutor sceneConstructionScriptExecutor)
        {
            _sceneLoader = sceneLoader;
            _sceneConstructionScriptExecutor = sceneConstructionScriptExecutor;
        }

        public Scene CurrentScene { get; private set; }

        public void LoadScene(string path)
        {
            var scene = _sceneLoader.Load(path);
            _sceneConstructionScriptExecutor.Execute(scene);
            CurrentScene = scene;
        }
    }
}