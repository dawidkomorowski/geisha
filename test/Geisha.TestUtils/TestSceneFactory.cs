using System;
using System.Linq;
using Geisha.Engine.Core.SceneModel;
using NSubstitute;

namespace Geisha.TestUtils
{
    public static class TestSceneFactory
    {
        public static Scene Create()
        {
            return new Scene();
        }

        // TODO Is it needed?
        public static Scene CreateWithBehaviorFactoriesFor(params string[] sceneBehaviorNames)
        {
            throw new NotImplementedException();
            var sceneBehaviorFactories = sceneBehaviorNames.Select(name =>
            {
                var sceneBehaviorFactory = Substitute.For<ISceneBehaviorFactory>();
                sceneBehaviorFactory.BehaviorName.Returns(name);
                return sceneBehaviorFactory;
            }).ToArray();

            var scene = new Scene();

            foreach (var sceneBehaviorFactory in sceneBehaviorFactories)
            {
                sceneBehaviorFactory.Create(scene).Returns(ci => Substitute.ForPartsOf<SceneBehavior>(scene));
            }

            return scene;
        }
    }
}