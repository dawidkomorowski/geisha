using Geisha.Demo.Common;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Demo.Screens.Tmp
{
    internal sealed class TmpSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "Tmp";
        private readonly CommonScreenFactory _commonScreenFactory;

        public TmpSceneBehaviorFactory(CommonScreenFactory commonScreenFactory)
        {
            _commonScreenFactory = commonScreenFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new TmpSceneBehavior(scene, _commonScreenFactory);

        private sealed class TmpSceneBehavior : SceneBehavior
        {
            private readonly CommonScreenFactory _commonScreenFactory;

            public TmpSceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory) : base(scene)
            {
                _commonScreenFactory = commonScreenFactory;
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                _commonScreenFactory.CreateCommonScreen(Scene, "https://github.com/dawidkomorowski/geisha");
            }
        }
    }
}