using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Coroutines;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Physics.Systems;

namespace Geisha.Engine.E2EApp.EngineApiCanBeInjectedToCustomGameCode
{
    internal sealed class TestSystem : ICustomSystem
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

        public TestSystem
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

        public string Name => "EngineApiCanBeInjectedToCustomGameCode-TestSystem";

        public void ProcessFixedUpdate()
        {
        }

        public void ProcessUpdate(GameTime gameTime)
        {
            E2ETest.Report("E7691D98-AF87-4268-9C39-43822A790377", $"Engine API Injected Into System: {_audioBackend.GetType()}");
            E2ETest.Report("FE445F35-E624-4BCF-800C-FAD91F3C0216", $"Engine API Injected Into System: {_engineManager.GetType()}");
            E2ETest.Report("A9236158-3810-41A0-B14B-8516A39E404B", $"Engine API Injected Into System: {_assetStore.GetType()}");
            E2ETest.Report("9F0D1065-B9AF-4DD6-9ED7-06D3462E5795", $"Engine API Injected Into System: {_debugRenderer.GetType()}");
            E2ETest.Report("FF5B94ED-12B5-43E6-9F3F-0A57BEB162BA", $"Engine API Injected Into System: {_sceneLoader.GetType()}");
            E2ETest.Report("D89648A6-5676-492E-ADB8-6296C1B8BEE6", $"Engine API Injected Into System: {_sceneManager.GetType()}");
            E2ETest.Report("DD6882F2-4A1B-42F3-993C-473593C46DE5", $"Engine API Injected Into System: {_sceneSerializer.GetType()}");
            E2ETest.Report("618427C0-D078-451C-8877-B3B81C99B5FF", $"Engine API Injected Into System: {_coroutineSystem.GetType()}");
            E2ETest.Report("932A0F77-F5F9-4CB5-B3EE-56FDF6139291", $"Engine API Injected Into System: {_physicsSystem.GetType()}");
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