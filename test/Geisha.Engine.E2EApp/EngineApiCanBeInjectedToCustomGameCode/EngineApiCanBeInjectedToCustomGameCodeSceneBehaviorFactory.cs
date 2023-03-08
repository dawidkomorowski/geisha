using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Coroutines;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.E2EApp.EngineApiCanBeInjectedToCustomGameCode
{
    internal sealed class EngineApiCanBeInjectedToCustomGameCodeSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "EngineApiCanBeInjectedToCustomGameCode";

        private readonly IAudioBackend _audioBackend;
        private readonly IEngineManager _engineManager;
        private readonly IAssetStore _assetStore;
        private readonly IDebugRenderer _debugRenderer;
        private readonly ISceneLoader _sceneLoader;
        private readonly ISceneManager _sceneManager;
        private readonly ISceneSerializer _sceneSerializer;
        private readonly ICoroutineSystem _coroutineSystem;

        public EngineApiCanBeInjectedToCustomGameCodeSceneBehaviorFactory(
            IAudioBackend audioBackend,
            IEngineManager engineManager,
            IAssetStore assetStore,
            IDebugRenderer debugRenderer,
            ISceneLoader sceneLoader,
            ISceneManager sceneManager,
            ISceneSerializer sceneSerializer,
            ICoroutineSystem coroutineSystem)
        {
            _audioBackend = audioBackend;
            _engineManager = engineManager;
            _assetStore = assetStore;
            _debugRenderer = debugRenderer;
            _sceneLoader = sceneLoader;
            _sceneManager = sceneManager;
            _sceneSerializer = sceneSerializer;
            _coroutineSystem = coroutineSystem;
        }

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene) =>
            new EngineApiCanBeInjectedToCustomGameCodeSceneBehavior(
                scene,
                _audioBackend,
                _engineManager,
                _assetStore,
                _debugRenderer,
                _sceneLoader,
                _sceneManager,
                _sceneSerializer,
                _coroutineSystem
            );

        private sealed class EngineApiCanBeInjectedToCustomGameCodeSceneBehavior : SceneBehavior
        {
            private readonly IAudioBackend _audioBackend;
            private readonly IEngineManager _engineManager;
            private readonly IAssetStore _assetStore;
            private readonly IDebugRenderer _debugRenderer;
            private readonly ISceneLoader _sceneLoader;
            private readonly ISceneManager _sceneManager;
            private readonly ISceneSerializer _sceneSerializer;
            private readonly ICoroutineSystem _coroutineSystem;

            public EngineApiCanBeInjectedToCustomGameCodeSceneBehavior(
                Scene scene,
                IAudioBackend audioBackend,
                IEngineManager engineManager,
                IAssetStore assetStore,
                IDebugRenderer debugRenderer,
                ISceneLoader sceneLoader,
                ISceneManager sceneManager,
                ISceneSerializer sceneSerializer,
                ICoroutineSystem coroutineSystem) : base(scene)
            {
                _audioBackend = audioBackend;
                _engineManager = engineManager;
                _assetStore = assetStore;
                _debugRenderer = debugRenderer;
                _sceneLoader = sceneLoader;
                _sceneManager = sceneManager;
                _sceneSerializer = sceneSerializer;
                _coroutineSystem = coroutineSystem;
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                var exitTestAppComponent = Scene.CreateEntity().CreateComponent<ExitTestAppComponent>();
                exitTestAppComponent.ExitOnFrame = 1;

                E2ETest.Report("9CA85BC0-A6B3-44ED-9FA7-C64F0909F1A3", "Engine API Injected Into SceneBehavior");

                Scene.CreateEntity().CreateComponent<EngineApiDependencyInjectionTestComponent>();
            }
        }
    }
}