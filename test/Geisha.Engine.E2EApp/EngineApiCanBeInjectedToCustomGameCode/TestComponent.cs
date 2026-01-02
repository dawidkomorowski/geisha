using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Coroutines;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Physics.Systems;

namespace Geisha.Engine.E2EApp.EngineApiCanBeInjectedToCustomGameCode
{
    internal sealed class TestComponent : BehaviorComponent
    {
        private readonly IAudioBackend _audioBackend;
        private readonly IEngineManager _engineManager;
        private readonly IAssetStore _assetStore;
        private readonly IDebugRenderer _debugRenderer;
        private readonly ISceneLoader _sceneLoader;
        private readonly ISceneManager _sceneManager;
        private readonly ISceneSerializer _sceneSerializer;
        private readonly ICoroutineSystem _coroutineSystem;
        private readonly IPhysicsSystem _physicsSystem;

        public TestComponent
        (
            Entity entity,
            IAudioBackend audioBackend,
            IEngineManager engineManager,
            IAssetStore assetStore,
            IDebugRenderer debugRenderer,
            ISceneLoader sceneLoader,
            ISceneManager sceneManager,
            ISceneSerializer sceneSerializer,
            ICoroutineSystem coroutineSystem,
            IPhysicsSystem physicsSystem
        ) : base(entity)
        {
            _audioBackend = audioBackend;
            _engineManager = engineManager;
            _assetStore = assetStore;
            _debugRenderer = debugRenderer;
            _sceneLoader = sceneLoader;
            _sceneManager = sceneManager;
            _sceneSerializer = sceneSerializer;
            _coroutineSystem = coroutineSystem;
            _physicsSystem = physicsSystem;
        }

        public override void OnStart()
        {
            E2ETest.Report("484E1AFA-EEFE-4E3A-9D8E-A304847C8C16", $"Engine API Injected Into Component: {_audioBackend.GetType()}");
            E2ETest.Report("568407AA-0471-42BD-8CBD-6CB2A7526B76", $"Engine API Injected Into Component: {_engineManager.GetType()}");
            E2ETest.Report("7B72B6EB-69BC-49F2-BEA5-CC073581F1D0", $"Engine API Injected Into Component: {_assetStore.GetType()}");
            E2ETest.Report("4449A465-20AB-4E99-9C62-EB475387910D", $"Engine API Injected Into Component: {_debugRenderer.GetType()}");
            E2ETest.Report("462F0430-A3D3-4E2D-91C6-A4C8EBBE24C8", $"Engine API Injected Into Component: {_sceneLoader.GetType()}");
            E2ETest.Report("035F113D-43D8-4B92-B4DF-D1F6FDCBEEC9", $"Engine API Injected Into Component: {_sceneManager.GetType()}");
            E2ETest.Report("899851F9-822D-4826-860B-7AB4C611DAC1", $"Engine API Injected Into Component: {_sceneSerializer.GetType()}");
            E2ETest.Report("4AF66A42-E328-471B-BDB0-C0D987F5EAE1", $"Engine API Injected Into Component: {_coroutineSystem.GetType()}");
            E2ETest.Report("11519AAE-5E1A-4462-973A-81B09672721D", $"Engine API Injected Into Component: {_physicsSystem.GetType()}");
        }
    }

    internal sealed class TestComponentFactory : ComponentFactory<TestComponent>
    {
        private readonly IAudioBackend _audioBackend;
        private readonly IEngineManager _engineManager;
        private readonly IAssetStore _assetStore;
        private readonly IDebugRenderer _debugRenderer;
        private readonly ISceneLoader _sceneLoader;
        private readonly ISceneManager _sceneManager;
        private readonly ISceneSerializer _sceneSerializer;
        private readonly ICoroutineSystem _coroutineSystem;
        private readonly IPhysicsSystem _physicsSystem;

        public TestComponentFactory
        (
            IAudioBackend audioBackend,
            IEngineManager engineManager,
            IAssetStore assetStore,
            IDebugRenderer debugRenderer,
            ISceneLoader sceneLoader,
            ISceneManager sceneManager,
            ISceneSerializer sceneSerializer,
            ICoroutineSystem coroutineSystem,
            IPhysicsSystem physicsSystem
        )
        {
            _audioBackend = audioBackend;
            _engineManager = engineManager;
            _assetStore = assetStore;
            _debugRenderer = debugRenderer;
            _sceneLoader = sceneLoader;
            _sceneManager = sceneManager;
            _sceneSerializer = sceneSerializer;
            _coroutineSystem = coroutineSystem;
            _physicsSystem = physicsSystem;
        }

        protected override TestComponent CreateComponent(Entity entity) =>
            new(
                entity,
                _audioBackend,
                _engineManager,
                _assetStore,
                _debugRenderer,
                _sceneLoader,
                _sceneManager,
                _sceneSerializer,
                _coroutineSystem,
                _physicsSystem
            );
    }
}