using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.SceneModel;
using NSubstitute;

namespace Geisha.TestUtils
{
    public static class TestSceneFactory
    {
        public static Scene Create()
        {
            return new Scene(Enumerable.Empty<ISceneBehaviorFactory>());
        }

        public static Scene Create(IEnumerable<ISceneBehaviorFactory> sceneBehaviorFactories)
        {
            return new Scene(sceneBehaviorFactories);
        }

        public static Scene CreateWithBehaviorFactoriesFor(params string[] sceneBehaviorNames)
        {
            var sceneBehaviorFactories = sceneBehaviorNames.Select(name =>
            {
                var sceneBehaviorFactory = Substitute.For<ISceneBehaviorFactory>();
                sceneBehaviorFactory.BehaviorName.Returns(name);
                return sceneBehaviorFactory;
            }).ToArray();

            var scene = new Scene(sceneBehaviorFactories);

            foreach (var sceneBehaviorFactory in sceneBehaviorFactories)
            {
                sceneBehaviorFactory.Create(scene).Returns(ci => Substitute.ForPartsOf<SceneBehavior>(scene));
            }

            return scene;
        }
    }
}