using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.ScenePropertiesEditor;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.ScenePropertiesEditor
{
    [TestFixture]
    public class ScenePropertiesEditorViewModelTests
    {
        [Test]
        public void SceneBehaviorName_ShouldSetSceneModelSceneBehaviorName_WhenSet()
        {
            Assert.Fail("TODO");

            // Arrange
            var scene = TestSceneFactory.CreateWithBehaviorFactoriesFor("Old scene behavior", "New scene behavior");
            // TODO scene.SceneBehaviorName = "Old scene behavior";
            // TODO var sceneModel = new SceneModel(scene);
            // TODO var scenePropertiesEditorViewModel = new ScenePropertiesEditorViewModel(sceneModel);

            // Act
            // TODO scenePropertiesEditorViewModel.SceneBehaviorName = "New scene behavior";

            // Assert
            // TODO Assert.That(scenePropertiesEditorViewModel.SceneBehaviorName, Is.EqualTo("New scene behavior"));
            // TODO Assert.That(sceneModel.SceneBehaviorName, Is.EqualTo("New scene behavior"));
        }
    }
}