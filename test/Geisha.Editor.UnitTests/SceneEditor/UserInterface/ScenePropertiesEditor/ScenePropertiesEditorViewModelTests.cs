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
            // Arrange
            var scene = TestSceneFactory.CreateWithBehaviorFactoriesFor("Old scene behavior", "New scene behavior");
            scene.SceneBehaviorName = "Old scene behavior";
            var sceneModel = new SceneModel(scene);
            var scenePropertiesEditorViewModel = new ScenePropertiesEditorViewModel(sceneModel);

            // Act
            scenePropertiesEditorViewModel.SceneBehaviorName = "New scene behavior";

            // Assert
            Assert.That(scenePropertiesEditorViewModel.SceneBehaviorName, Is.EqualTo("New scene behavior"));
            Assert.That(sceneModel.SceneBehaviorName, Is.EqualTo("New scene behavior"));
        }
    }
}