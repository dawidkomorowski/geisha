using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using System;
using Geisha.Engine.Core.Components;

namespace Geisha.Demo
{
    internal sealed class MainSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "Main";
        private readonly IAssetStore _assetStore;
        private readonly IAudioBackend _audioBackend;

        public MainSceneBehaviorFactory(IAssetStore assetStore, IAudioBackend audioBackend)
        {
            _assetStore = assetStore;
            _audioBackend = audioBackend;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new MainSceneBehavior(scene, _assetStore, _audioBackend);

        private sealed class MainSceneBehavior : SceneBehavior
        {
            private readonly IAssetStore _assetStore;
            private readonly IAudioPlayer _audioPlayer;

            public MainSceneBehavior(Scene scene, IAssetStore assetStore, IAudioBackend audioBackend) : base(scene)
            {
                _assetStore = assetStore;
                _audioPlayer = audioBackend.AudioPlayer;
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                var music = _assetStore.GetAsset<ISound>(new AssetId(new Guid("a11ff6d2-00f3-4a2e-ae34-4e60ad948efc")));
                _audioPlayer.Play(music, true);

                Scene.CreateEntity().CreateComponent<GoToHelloScreenComponent>();
            }
        }
    }

    internal sealed class GoToHelloScreenComponent : BehaviorComponent
    {
        private readonly ISceneManager _sceneManager;

        public GoToHelloScreenComponent(Entity entity, ISceneManager sceneManager) : base(entity)
        {
            _sceneManager = sceneManager;
        }

        public override void OnStart()
        {
            _sceneManager.LoadEmptyScene("Hello");
        }
    }

    internal sealed class GoToHelloScreenComponentFactory : ComponentFactory<GoToHelloScreenComponent>
    {
        private readonly ISceneManager _sceneManager;

        public GoToHelloScreenComponentFactory(ISceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        protected override GoToHelloScreenComponent CreateComponent(Entity entity) => new(entity, _sceneManager);
    }
}