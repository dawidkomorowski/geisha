using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.ScenePropertiesEditor;
using Geisha.Engine.Core.SceneModel;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.ScenePropertiesEditor
{
    [TestFixture]
    public class ScenePropertiesEditorViewModelTests
    {
        [Test]
        public void ConstructionScript_ShouldSetSceneModelConstructionScript_WhenSet()
        {
            // Arrange
            var scene = new Scene {ConstructionScript = "Old script"};
            var sceneModel = new SceneModel(scene);
            var scenePropertiesEditorViewModel = new ScenePropertiesEditorViewModel(sceneModel);

            // Act
            scenePropertiesEditorViewModel.ConstructionScript = "New script";

            // Assert
            Assert.That(scenePropertiesEditorViewModel.ConstructionScript, Is.EqualTo("New script"));
            Assert.That(sceneModel.ConstructionScript, Is.EqualTo("New script"));
        }
    }
}