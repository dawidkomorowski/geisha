using System;
using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     Defines API for loading and accessing already loaded scenes.
    /// </summary>
    public interface ISceneManager
    {
        /// <summary>
        ///     Current scene that is loaded and processed by systems. It is the latest scene loaded by
        ///     <see cref="ISceneManager" />.
        /// </summary>
        Scene? CurrentScene { get; }

        /// <summary>
        ///     Loads scene specified by path to a scene file. Loaded scene becomes current scene.
        /// </summary>
        /// <param name="path">Path to the scene file that will be loaded.</param>
        /// <param name="sceneLoadMode">
        ///     Load mode that specifies scene loading behavior. See <see cref="SceneLoadMode" /> for
        ///     details.
        /// </param>
        /// <remarks>
        ///     The scene specified to be loaded will be actually loaded in the beginning of the next frame. So after calling
        ///     <see cref="LoadScene" /> the <see cref="CurrentScene" /> will be processed by systems in currently executing frame
        ///     till its end. Then on the next frame scene is loaded and it replaces <see cref="CurrentScene" />. Previous instance
        ///     of <see cref="CurrentScene" /> becomes subject for garbage collection.
        /// </remarks>
        void LoadScene(string path, SceneLoadMode sceneLoadMode = SceneLoadMode.PreserveAssets);
    }

    /// <summary>
    ///     Enumeration specifying scene loading behavior.
    /// </summary>
    public enum SceneLoadMode
    {
        /// <summary>
        ///     Unload all loaded assets before loading specified scene.
        /// </summary>
        UnloadAssets,

        /// <summary>
        ///     Keep all loaded assets when loading specified scene.
        /// </summary>
        PreserveAssets
    }

    internal interface ISceneManagerForGameLoop : ISceneManager
    {
        void OnNextFrame();
    }

    internal class SceneManager : ISceneManagerForGameLoop
    {
        private readonly IAssetStore _assetStore;
        private readonly ISceneLoader _sceneLoader;
        private LoadSceneRequest _loadSceneRequest;

        public SceneManager(IAssetStore assetStore, ISceneLoader sceneLoader)
        {
            _assetStore = assetStore;
            _sceneLoader = sceneLoader;

            _loadSceneRequest.MarkAsHandled();
        }

        public Scene? CurrentScene { get; private set; }

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
            scene.OnLoaded();
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