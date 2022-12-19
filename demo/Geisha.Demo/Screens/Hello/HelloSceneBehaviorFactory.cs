using Geisha.Demo.Common;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Demo.Screens.Hello
{
    internal sealed class HelloSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "Hello";
        private readonly CommonScreenFactory _commonScreenFactory;

        public HelloSceneBehaviorFactory(CommonScreenFactory commonScreenFactory)
        {
            _commonScreenFactory = commonScreenFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new HelloSceneBehavior(scene, _commonScreenFactory);

        private sealed class HelloSceneBehavior : SceneBehavior
        {
            private readonly CommonScreenFactory _commonScreenFactory;

            public HelloSceneBehavior(Scene scene, CommonScreenFactory commonScreenFactory) : base(scene)
            {
                _commonScreenFactory = commonScreenFactory;
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                _commonScreenFactory.CreateCommonScreen(Scene, "https://github.com/dawidkomorowski/geisha/blob/master/src/Geisha.Engine/Engine.cs");
            }
        }
    }
}