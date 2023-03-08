using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Coroutines;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.E2EApp.EngineApiCanBeInjectedToCustomGameCode
{
    internal sealed class EngineApiDependencyInjectionTestSystem : ICustomSystem
    {
        private readonly IAudioBackend _audioBackend;
        private readonly IEngineManager _engineManager;
        private readonly IAssetStore _assetStore;
        private readonly IDebugRenderer _debugRenderer;
        private readonly ISceneLoader _sceneLoader;
        private readonly ISceneManager _sceneManager;
        private readonly ISceneSerializer _sceneSerializer;
        private readonly ICoroutineSystem _coroutineSystem;

        public EngineApiDependencyInjectionTestSystem(
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

        public string Name => "EngineApiDependencyInjectionTestSystem";

        public void ProcessFixedUpdate()
        {
        }

        public void ProcessUpdate(GameTime gameTime)
        {
            E2ETest.Report("E7691D98-AF87-4268-9C39-43822A790377", "Engine API Injected Into System");
        }

        public void OnEntityCreated(Entity entity)
        {
        }

        public void OnEntityRemoved(Entity entity)
        {
        }

        public void OnEntityParentChanged(Entity entity, Entity? oldParent, Entity? newParent)
        {
        }

        public void OnComponentCreated(Component component)
        {
        }

        public void OnComponentRemoved(Component component)
        {
        }
    }
}