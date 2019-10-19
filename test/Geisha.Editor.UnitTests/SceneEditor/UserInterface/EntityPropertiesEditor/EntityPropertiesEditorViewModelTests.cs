using System.Linq;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor;
using Geisha.Engine.Core.SceneModel;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.EntityPropertiesEditor
{
    [TestFixture]
    public class EntityPropertiesEditorViewModelTests
    {
        [Test]
        public void Name_ShouldSetEntityModelName_WhenSet()
        {
            // Arrange
            var entity = new Entity {Name = "Old name"};
            var entityModel = new EntityModel(entity);
            var entityPropertiesEditorViewModel = new EntityPropertiesEditorViewModel(entityModel);

            // Act
            entityPropertiesEditorViewModel.Name = "New name";

            // Assert
            Assert.That(entityPropertiesEditorViewModel.Name, Is.EqualTo("New name"));
            Assert.That(entityModel.Name, Is.EqualTo("New name"));
        }

        [Test]
        public void AddTransformComponentCommand_ShouldAddTransformComponentModelToEntityModel()
        {
            // Arrange
            var entity = new Entity();
            var entityModel = new EntityModel(entity);
            var entityPropertiesEditorViewModel = new EntityPropertiesEditorViewModel(entityModel);

            // Act
            entityPropertiesEditorViewModel.AddTransformComponentCommand.Execute(null);

            // Assert
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components.Single().Name, Is.EqualTo("Transform Component"));
        }
    }
}