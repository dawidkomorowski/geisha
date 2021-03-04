using System.Linq;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.ScenePropertiesEditor;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.ScenePropertiesEditor
{
    [TestFixture]
    public class ScenePropertiesEditorViewModelTests
    {
        [Test]
        public void AvailableSceneBehaviors_ShouldReturnBehaviorsDefinedBySceneModel()
        {
            // Arrange
            var sceneModel = TestSceneModelFactory.Create("Behavior 1", "Behavior 2", "Behavior 3");

            // Act
            var scenePropertiesEditorViewModel = new ScenePropertiesEditorViewModel(sceneModel);

            // Assert
            Assert.That(scenePropertiesEditorViewModel.AvailableSceneBehaviors, Is.EquivalentTo(new[]
            {
                new SceneBehaviorName("Behavior 1"),
                new SceneBehaviorName("Behavior 2"),
                new SceneBehaviorName("Behavior 3")
            }));
        }

        [Test]
        public void SceneBehavior_ShouldSetSceneModelSceneBehavior_WhenSet()
        {
            // Arrange
            const string oldBehaviorName = "Old scene behavior";
            const string newBehaviorName = "New scene behavior";
            var sceneModel = TestSceneModelFactory.Create(oldBehaviorName, newBehaviorName);
            sceneModel.SceneBehavior = sceneModel.AvailableSceneBehaviors.Single(b => b.Value == oldBehaviorName);
            var scenePropertiesEditorViewModel = new ScenePropertiesEditorViewModel(sceneModel);

            // Act
            scenePropertiesEditorViewModel.SceneBehavior = sceneModel.AvailableSceneBehaviors.Single(b => b.Value == newBehaviorName);

            // Assert
            Assert.That(scenePropertiesEditorViewModel.SceneBehavior.Value, Is.EqualTo("New scene behavior"));
            Assert.That(sceneModel.SceneBehavior.Value, Is.EqualTo("New scene behavior"));
        }
    }
}