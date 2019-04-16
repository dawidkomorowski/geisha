using System;
using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Core.SceneModel
{
    // TODO Add documentation.
    public interface ISceneManager
    {
        Scene CurrentScene { get; }

        void LoadScene(string path, SceneLoadMode sceneLoadMode = SceneLoadMode.PreserveAssets);
    }

    // TODO Add documentation.
    public enum SceneLoadMode
    {
        UnloadAssets,
        PreserveAssets
    }

    internal interface ISceneManagerForGameLoop : ISceneManager
    {
        void OnNextFrame();
    }

    internal class SceneManager : ISceneManagerForGameLoop
    {
        private readonly IAssetStore _assetStore;
        private readonly ISceneConstructionScriptExecutor _sceneConstructionScriptExecutor;
        private readonly ISceneLoader _sceneLoader;
        private LoadSceneRequest _loadSceneRequest;

        public SceneManager(IAssetStore assetStore, ISceneConstructionScriptExecutor sceneConstructionScriptExecutor, ISceneLoader sceneLoader)
        {
            _assetStore = assetStore;
            _sceneConstructionScriptExecutor = sceneConstructionScriptExecutor;
            _sceneLoader = sceneLoader;

            _loadSceneRequest.MarkAsHandled();
        }

        public Scene CurrentScene { get; private set; }

        public void LoadScene(string path, SceneLoadMode sceneLoadMode = SceneLoadMode.PreserveAssets)
        {
            _loadSceneRequest = new LoadSceneRequest(path, sceneLoadMode);
        }

        public void OnNextFrame()
        {
            if (_loadSceneRequest.IsHandled) return;

            LoadSceneInternal(_loadSceneRequest.SceneFilePath, _loadSceneRequest.SceneLoadMode);
            _loadSceneRequest.MarkAsHandled();
        }

        private void LoadSceneInternal(string path, SceneLoadMode sceneLoadMode)
        {
            if (sceneLoadMode == SceneLoadMode.UnloadAssets)
            {
                _assetStore.UnloadAssets();
            }

            var scene = _sceneLoader.Load(path);
            _sceneConstructionScriptExecutor.Execute(scene);
            CurrentScene = scene;

            GC.Collect();
        }

        private struct LoadSceneRequest
        {
            public LoadSceneRequest(string sceneFilePath, SceneLoadMode sceneLoadMode)
            {
                SceneFilePath = sceneFilePath;
                SceneLoadMode = sceneLoadMode;
                IsHandled = false;
            }

            public string SceneFilePath { get; }
            public SceneLoadMode SceneLoadMode { get; }
            public bool IsHandled { get; private set; }

            public void MarkAsHandled()
            {
                IsHandled = true;
            }
        }
    }
}