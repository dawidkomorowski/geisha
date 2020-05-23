using System.Linq;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.EntityPropertiesEditor
{
    [TestFixture]
    public class EntityPropertiesEditorViewModelTests
    {
        private IComponentPropertiesEditorViewModelFactory _componentPropertiesEditorViewModelFactory = null!;

        [SetUp]
        public void SetUp()
        {
            _componentPropertiesEditorViewModelFactory = Substitute.For<IComponentPropertiesEditorViewModelFactory>();
        }

        [Test]
        public void Constructor_ShouldCreateEntityPropertiesViewModelWithComponents()
        {
            // Arrange
            var entity = new Entity();
            entity.AddComponent(new TransformComponent());
            var entityModel = new EntityModel(entity);

            var componentPropertiesEditorViewModel = new TestComponentPropertiesEditorViewModel();
            _componentPropertiesEditorViewModelFactory.Create(Arg.Any<TransformComponentModel>()).Returns(componentPropertiesEditorViewModel);

            // Act
            var entityPropertiesEditorViewModel = new EntityPropertiesEditorViewModel(entityModel, _componentPropertiesEditorViewModelFactory);

            // Assert
            Assert.That(entityPropertiesEditorViewModel.Components, Has.Count.EqualTo(1));
            Assert.That(entityPropertiesEditorViewModel.Components.Single(), Is.EqualTo(componentPropertiesEditorViewModel));
        }

        [Test]
        public void Name_ShouldSetEntityModelName_WhenSet()
        {
            // Arrange
            var entity = new Entity {Name = "Old name"};
            var entityModel = new EntityModel(entity);
            var entityPropertiesEditorViewModel = new EntityPropertiesEditorViewModel(entityModel, _componentPropertiesEditorViewModelFactory);

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
            var entityPropertiesEditorViewModel = new EntityPropertiesEditorViewModel(entityModel, _componentPropertiesEditorViewModelFactory);

            // Act
            entityPropertiesEditorViewModel.AddTransformComponentCommand.Execute(null);

            // Assert
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components.Single().Name, Is.EqualTo("Transform Component"));
        }

        [Test]
        public void AddEllipseRendererComponent_ShouldAddEllipseRendererComponentModelToEntityModel()
        {
            // Arrange
            var entity = new Entity();
            var entityModel = new EntityModel(entity);
            var entityPropertiesEditorViewModel = new EntityPropertiesEditorViewModel(entityModel, _componentPropertiesEditorViewModelFactory);

            // Act
            entityPropertiesEditorViewModel.AddEllipseRendererComponentCommand.Execute(null);

            // Assert
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components.Single().Name, Is.EqualTo("Ellipse Renderer Component"));
        }

        [Test]
        public void AddRectangleRendererComponent_ShouldAddRectangleRendererComponentModelToEntityModel()
        {
            // Arrange
            var entity = new Entity();
            var entityModel = new EntityModel(entity);
            var entityPropertiesEditorViewModel = new EntityPropertiesEditorViewModel(entityModel, _componentPropertiesEditorViewModelFactory);

            // Act
            entityPropertiesEditorViewModel.AddRectangleRendererComponentCommand.Execute(null);

            // Assert
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components.Single().Name, Is.EqualTo("Rectangle Renderer Component"));
        }

        [Test]
        public void AddTextRendererComponent_ShouldAddTextRendererComponentModelToEntityModel()
        {
            // Arrange
            var entity = new Entity();
            var entityModel = new EntityModel(entity);
            var entityPropertiesEditorViewModel = new EntityPropertiesEditorViewModel(entityModel, _componentPropertiesEditorViewModelFactory);

            // Act
            entityPropertiesEditorViewModel.AddTextRendererComponentCommand.Execute(null);

            // Assert
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components.Single().Name, Is.EqualTo("Text Renderer Component"));
        }

        [Test]
        public void AddCircleColliderComponent_ShouldAddCircleColliderComponentModelToEntityModel()
        {
            // Arrange
            var entity = new Entity();
            var entityModel = new EntityModel(entity);
            var entityPropertiesEditorViewModel = new EntityPropertiesEditorViewModel(entityModel, _componentPropertiesEditorViewModelFactory);

            // Act
            entityPropertiesEditorViewModel.AddCircleColliderComponentCommand.Execute(null);

            // Assert
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components.Single().Name, Is.EqualTo("Circle Collider Component"));
        }

        [Test]
        public void AddRectangleColliderComponent_ShouldAddRectangleColliderComponentModelToEntityModel()
        {
            // Arrange
            var entity = new Entity();
            var entityModel = new EntityModel(entity);
            var entityPropertiesEditorViewModel = new EntityPropertiesEditorViewModel(entityModel, _componentPropertiesEditorViewModelFactory);

            // Act
            entityPropertiesEditorViewModel.AddRectangleColliderComponentCommand.Execute(null);

            // Assert
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components.Single().Name, Is.EqualTo("Rectangle Collider Component"));
        }

        [Test]
        public void OnComponentAdded_ShouldAddComponentPropertiesEditorViewModelToComponents()
        {
            // Arrange
            var entity = new Entity();
            var entityModel = new EntityModel(entity);
            var entityPropertiesEditorViewModel = new EntityPropertiesEditorViewModel(entityModel, _componentPropertiesEditorViewModelFactory);

            var componentPropertiesEditorViewModel = new TestComponentPropertiesEditorViewModel();
            _componentPropertiesEditorViewModelFactory.Create(Arg.Any<TransformComponentModel>()).Returns(componentPropertiesEditorViewModel);

            // Act
            entityPropertiesEditorViewModel.AddTransformComponentCommand.Execute(null);

            // Assert
            Assert.That(entityPropertiesEditorViewModel.Components, Has.Count.EqualTo(1));
            Assert.That(entityPropertiesEditorViewModel.Components.Single(), Is.EqualTo(componentPropertiesEditorViewModel));
        }

        private sealed class TestComponentPropertiesEditorViewModel : ComponentPropertiesEditorViewModel
        {
            public TestComponentPropertiesEditorViewModel() : base(Substitute.For<IComponentModel>())
            {
            }
        }
    }
}