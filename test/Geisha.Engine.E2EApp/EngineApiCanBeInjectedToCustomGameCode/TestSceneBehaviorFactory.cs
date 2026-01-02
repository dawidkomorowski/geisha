using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Coroutines;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Physics.Systems;

namespace Geisha.Engine.E2EApp.EngineApiCanBeInjectedToCustomGameCode
{
    internal sealed class TestSceneBehaviorFactory : ISceneBehaviorFactory
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
        private readonly IPhysicsSystem _physicsSystem;

        public TestSceneBehaviorFactory
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

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene) =>
            new TestSceneBehavior(
                scene,
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

        private sealed class TestSceneBehavior : SceneBehavior
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

            public TestSceneBehavior
            (
                Scene scene,
                IAudioBackend audioBackend,
                IEngineManager engineManager,
                IAssetStore assetStore,
                IDebugRenderer debugRenderer,
                ISceneLoader sceneLoader,
                ISceneManager sceneManager,
                ISceneSerializer sceneSerializer,
                ICoroutineSystem coroutineSystem,
                IPhysicsSystem physicsSystem
            ) : base(scene)
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

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                var exitTestAppComponent = Scene.CreateEntity().CreateComponent<ExitTestAppComponent>();
                exitTestAppComponent.ExitOnFrame = 1;

                E2ETest.Report("3211DA7A-5A4C-409D-B8F6-D82816D7CFA2", $"Engine API Injected Into SceneBehavior: {_audioBackend.GetType()}");
                E2ETest.Report("C7897578-6670-4DEA-A32F-689629FE651E", $"Engine API Injected Into SceneBehavior: {_engineManager.GetType()}");
                E2ETest.Report("B94536CE-5369-4105-B901-EC878E20E71F", $"Engine API Injected Into SceneBehavior: {_assetStore.GetType()}");
                E2ETest.Report("2F59C6C4-5183-4B33-9433-9AD9995F0923", $"Engine API Injected Into SceneBehavior: {_debugRenderer.GetType()}");
                E2ETest.Report("A3DB27E0-2A71-4728-9BE8-5C060F406EC8", $"Engine API Injected Into SceneBehavior: {_sceneLoader.GetType()}");
                E2ETest.Report("5C9C1856-8DF1-4E2D-BEF3-BB524FC62544", $"Engine API Injected Into SceneBehavior: {_sceneManager.GetType()}");
                E2ETest.Report("56048FE9-5C59-44F5-8C4C-D96615B62D8C", $"Engine API Injected Into SceneBehavior: {_sceneSerializer.GetType()}");
                E2ETest.Report("8DC6B886-CC5C-431E-821A-900D3671CB70", $"Engine API Injected Into SceneBehavior: {_coroutineSystem.GetType()}");
                E2ETest.Report("59797ECF-0B77-4CA2-B389-020B884B9E8F", $"Engine API Injected Into SceneBehavior: {_physicsSystem.GetType()}");

                Scene.CreateEntity().CreateComponent<TestComponent>();
            }
        }
    }
}