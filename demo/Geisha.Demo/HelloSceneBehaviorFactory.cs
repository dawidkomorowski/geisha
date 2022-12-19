using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Demo
{
    internal sealed class HelloSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "HelloSceneBehavior";
        private readonly CommonScreenFactory _commonScreenFactory;
        private readonly IAssetStore _assetStore;

        public HelloSceneBehaviorFactory(CommonScreenFactory commonScreenFactory, IAssetStore assetStore)
        {
            _commonScreenFactory = commonScreenFactory;
            _assetStore = assetStore;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new HelloSceneBehavior(scene, _commonScreenFactory, _assetStore);

        private sealed class HelloSceneBehavior : SceneBehavior
        {
            private readonly CommonScreenFactory _commonScreenFactory;
            private readonly IAssetStore _assetStore;

            public HelloSceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory, IAssetStore assetStore) : base(scene)
            {
                _commonScreenFactory = commonScreenFactory;
                _assetStore = assetStore;
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                _commonScreenFactory.CreateCommonScreen(Scene);
            }
        }
    }
}