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
        private readonly ISceneLoader _sceneLoader;
        private readonly ISceneConstructionScriptExecutor _sceneConstructionScriptExecutor;
        private readonly IStartUpTask _startUpTask;

        public SceneManager(ISceneLoader sceneLoader, IStartUpTask startUpTask,
            ISceneConstructionScriptExecutor sceneConstructionScriptExecutor)
        {
            _sceneLoader = sceneLoader;
            _sceneConstructionScriptExecutor = sceneConstructionScriptExecutor;
            _startUpTask = startUpTask;
        }

        public Scene CurrentScene { get; private set; }

        public void LoadScene(string path)
        {
            _startUpTask.Run();

            var scene = _sceneLoader.Load(path);
            _sceneConstructionScriptExecutor.Execute(scene);
            CurrentScene = scene;
        }
    }

    public interface IStartUpTask
    {
        void Run();
    }
}