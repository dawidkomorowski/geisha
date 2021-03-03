using System.Linq;
using Geisha.Editor.SceneEditor.UserInterface.ScenePropertiesEditor;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.ScenePropertiesEditor
{
    [TestFixture]
    public class ScenePropertiesEditorViewModelTests
    {
        [Test]
        public void SceneBehaviorName_ShouldSetSceneModelSceneBehaviorName_WhenSet()
        {
            // Arrange
            const string oldBehaviorName = "Old scene behavior";
            const string newBehaviorName = "New scene behavior";
            var sceneModel = TestSceneModelFactory.Create(oldBehaviorName, newBehaviorName);
            sceneModel.SceneBehavior = sceneModel.AvailableSceneBehaviors.Single(b => b.Value == oldBehaviorName);
            var scenePropertiesEditorViewModel = new ScenePropertiesEditorViewModel(sceneModel);

            // Act
            scenePropertiesEditorViewModel.SceneBehaviorName = newBehaviorName;

            // Assert
            Assert.That(scenePropertiesEditorViewModel.SceneBehaviorName, Is.EqualTo("New scene behavior"));
            Assert.That(sceneModel.SceneBehavior.Value, Is.EqualTo("New scene behavior"));
        }
    }
}