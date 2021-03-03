using System;
using System.Linq;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;

namespace Geisha.Editor.UnitTests.SceneEditor
{
    public static class TestSceneModelFactory
    {
        public static SceneModel Create() => Create(TestSceneFactory.Create());
        public static SceneModel Create(Scene scene) => Create(scene, Array.Empty<string>());
        public static SceneModel Create(params string[] availableSceneBehaviors) => Create(TestSceneFactory.Create(), availableSceneBehaviors);

        public static SceneModel Create(Scene scene, params string[] availableSceneBehaviors)
        {
            var availableSceneBehaviorNames = availableSceneBehaviors.Select(s => new SceneBehaviorName(s));

            var factoryProvider = Substitute.For<ISceneBehaviorFactoryProvider>();
            factoryProvider.Get(Arg.Any<string>()).ThrowsForAnyArgs(new InvalidOperationException("Missing substitute configuration."));

            foreach (var sceneBehaviorName in availableSceneBehaviors)
            {
                var factory = Substitute.For<ISceneBehaviorFactory>();
                factory.BehaviorName.Returns(sceneBehaviorName);
                factory.Create(scene).Returns(ci =>
                {
                    var sceneBehavior = Substitute.ForPartsOf<SceneBehavior>(scene);
                    sceneBehavior.Name.Returns(sceneBehaviorName);
                    return sceneBehavior;
                });

                factoryProvider.Configure().Get(sceneBehaviorName).Returns(factory);
            }

            var sceneModel = new SceneModel(scene, availableSceneBehaviorNames, factoryProvider);

            return sceneModel;
        }
    }
}