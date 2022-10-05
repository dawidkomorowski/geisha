using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        Scene CurrentScene { get; }

        /// <summary>
        ///     Loads empty scene with <see cref="Scene.SceneBehavior" /> set to the value specified by
        ///     <paramref name="sceneBehaviorName" />. Loaded scene becomes current scene.
        /// </summary>
        /// <param name="sceneBehaviorName">Name of <see cref="SceneBehavior" /> to set for empty scene.</param>
        /// <param name="sceneLoadMode">
        ///     Load mode that specifies scene loading behavior. See <see cref="SceneLoadMode" /> for
        ///     details.
        /// </param>
        /// <remarks>
        ///     The empty scene will be actually loaded in the beginning of the next frame. So after calling
        ///     <see cref="LoadEmptyScene" /> the <see cref="CurrentScene" /> will be process by systems in currently executing
        ///     frame till its end. Then on the next frame scene is loaded and it replaces <see cref="CurrentScene" />. Previous
        ///     instance of <see cref="CurrentScene" /> becomes subject for garbage collection.
        /// </remarks>
        void LoadEmptyScene(string sceneBehaviorName, SceneLoadMode sceneLoadMode = SceneLoadMode.PreserveAssets);

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

    internal interface ISceneManagerInternal : ISceneManager
    {
        void OnNextFrame();
    }

    internal class SceneManager : ISceneManagerInternal
    {
        private readonly IAssetStore _assetStore;
        private readonly ISceneBehaviorFactoryProvider _sceneBehaviorFactoryProvider;
        private readonly ISceneFactory _sceneFactory;
        private readonly ISceneLoader _sceneLoader;
        private readonly List<ISceneObserver> _sceneObservers = new();
        private bool _isInitialized;
        private SceneLoadRequest _sceneLoadRequest;

        public SceneManager(IAssetStore assetStore, ISceneLoader sceneLoader, ISceneFactory sceneFactory,
            ISceneBehaviorFactoryProvider sceneBehaviorFactoryProvider)
        {
            _assetStore = assetStore;
            _sceneLoader = sceneLoader;
            _sceneFactory = sceneFactory;
            _sceneBehaviorFactoryProvider = sceneBehaviorFactoryProvider;

            _sceneLoadRequest.MarkAsHandled();

            CurrentScene = _sceneFactory.Create();
        }

        #region Implementation of ISceneManager

        public Scene CurrentScene { get; private set; }

        public void LoadEmptyScene(string sceneBehaviorName, SceneLoadMode sceneLoadMode = SceneLoadMode.PreserveAssets)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(SceneManager)} is not initialized.");
            }

            _sceneLoadRequest = SceneLoadRequest.LoadEmptyScene(sceneBehaviorName, sceneLoadMode);
        }

        public void LoadScene(string path, SceneLoadMode sceneLoadMode = SceneLoadMode.PreserveAssets)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(SceneManager)} is not initialized.");
            }

            _sceneLoadRequest = SceneLoadRequest.LoadSceneFromFile(path, sceneLoadMode);
        }

        #endregion

        #region Implementation of ISceneManagerInternal

        public void Initialize(IEnumerable<ISceneObserver> sceneObservers)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(SceneManager)} is already initialized.");
            }

            _sceneObservers.AddRange(sceneObservers);

            foreach (var sceneObserver in _sceneObservers)
            {
                CurrentScene.AddObserver(sceneObserver);
            }

            _isInitialized = true;
        }

        public void OnNextFrame()
        {
            Debug.Assert(_isInitialized, "_isInitialized");

            if (_sceneLoadRequest.IsHandled) return;

            LoadSceneInternal();
            _sceneLoadRequest.MarkAsHandled();
        }

        #endregion

        private void LoadSceneInternal()
        {
            switch (_sceneLoadRequest.SceneLoadMode)
            {
                case SceneLoadMode.UnloadAssets:
                    _assetStore.UnloadAssets();
                    break;
                case SceneLoadMode.PreserveAssets:
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(_sceneLoadRequest.SceneLoadMode)}", _sceneLoadRequest.SceneLoadMode,
                        $"Unhandled {nameof(SceneLoadMode)}.");
            }

            Scene scene;

            switch (_sceneLoadRequest.Source)
            {
                case SceneLoadRequest.SceneSource.Empty:
                    scene = _sceneFactory.Create();
                    scene.SceneBehavior = _sceneBehaviorFactoryProvider.Get(_sceneLoadRequest.SceneBehaviorName).Create(scene);
                    break;
                case SceneLoadRequest.SceneSource.File:
                    scene = _sceneLoader.Load(_sceneLoadRequest.SceneFilePath);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_sceneLoadRequest.Source), _sceneLoadRequest.Source,
                        $"Unhandled {nameof(SceneLoadRequest.SceneSource)}.");
            }

            foreach (var sceneObserver in _sceneObservers)
            {
                CurrentScene.RemoveObserver(sceneObserver);
            }

            CurrentScene = scene;

            foreach (var sceneObserver in _sceneObservers)
            {
                CurrentScene.AddObserver(sceneObserver);
            }

            scene.OnLoaded();

            GC.Collect();
        }

        private struct SceneLoadRequest
        {
            public static SceneLoadRequest LoadEmptyScene(string sceneBehaviorName, SceneLoadMode sceneLoadMode) =>
                new(SceneSource.Empty, sceneBehaviorName, string.Empty, sceneLoadMode);

            public static SceneLoadRequest LoadSceneFromFile(string sceneFilePath, SceneLoadMode sceneLoadMode) =>
                new(SceneSource.File, string.Empty, sceneFilePath, sceneLoadMode);

            private SceneLoadRequest(SceneSource source, string sceneBehaviorName, string sceneFilePath, SceneLoadMode sceneLoadMode)
            {
                Source = source;
                SceneBehaviorName = sceneBehaviorName;
                SceneFilePath = sceneFilePath;
                SceneLoadMode = sceneLoadMode;
                IsHandled = false;
            }

            public SceneSource Source { get; }
            public string SceneBehaviorName { get; }
            public string SceneFilePath { get; }
            public SceneLoadMode SceneLoadMode { get; }
            public bool IsHandled { get; private set; }

            public void MarkAsHandled()
            {
                IsHandled = true;
            }

            public enum SceneSource
            {
                Empty,
                File
            }
        }
    }
}