using Geisha.Editor.SceneEditor.Model;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.Model
{
    [TestFixture]
    public class SceneModelFactoryTests
    {
        [Test]
        public void Create_ShouldReturnSceneModelWithAvailableSceneBehaviorsForEachAvailableSceneBehaviorFactory()
        {
            // Arrange
            var sceneBehaviorFactoryProvider = Substitute.For<ISceneBehaviorFactoryProvider>();

            var sceneBehaviorFactory1 = Substitute.For<ISceneBehaviorFactory>();
            sceneBehaviorFactory1.BehaviorName.Returns("Behavior 1");
            var sceneBehaviorFactory2 = Substitute.For<ISceneBehaviorFactory>();
            sceneBehaviorFactory2.BehaviorName.Returns("Behavior 2");
            var sceneBehaviorFactory3 = Substitute.For<ISceneBehaviorFactory>();
            sceneBehaviorFactory3.BehaviorName.Returns("Behavior 3");

            var sceneModelFactory = new SceneModelFactory(sceneBehaviorFactoryProvider,
                new[] {sceneBehaviorFactory1, sceneBehaviorFactory2, sceneBehaviorFactory3});
            var scene = TestSceneFactory.Create();

            var sceneBehaviorName1 = new SceneBehaviorName("Behavior 1");
            var sceneBehaviorName2 = new SceneBehaviorName("Behavior 2");
            var sceneBehaviorName3 = new SceneBehaviorName("Behavior 3");

            // Act
            var sceneModel = sceneModelFactory.Create(scene);

            // Assert
            Assert.That(sceneModel.AvailableSceneBehaviors, Is.EquivalentTo(new[] {sceneBehaviorName1, sceneBehaviorName2, sceneBehaviorName3}));
        }
    }
}