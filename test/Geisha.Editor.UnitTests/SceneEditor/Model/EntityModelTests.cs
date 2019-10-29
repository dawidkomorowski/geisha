using System.Linq;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.Model
{
    [TestFixture]
    public class EntityModelTests
    {
        [Test]
        public void Constructor_ShouldCreateEntityModelWithComponents()
        {
            // Arrange
            var entity = new Entity();
            entity.AddComponent(new TransformComponent());

            // Act
            var entityModel = new EntityModel(entity);

            // Assert
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components.Single().Name, Is.EqualTo("Transform Component"));
        }

        [Test]
        public void Name_ShouldUpdateEntityNameAndNotifyWithEvent_WhenChanged()
        {
            // Arrange
            var entity = new Entity {Name = "Old name"};
            var entityModel = new EntityModel(entity);

            object eventSender = null;
            PropertyChangedEventArgs<string> eventArgs = null;
            entityModel.NameChanged += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            entityModel.Name = "New name";

            // Assert
            Assert.That(entity.Name, Is.EqualTo("New name"));
            Assert.That(entityModel.Name, Is.EqualTo("New name"));
            Assert.That(eventSender, Is.EqualTo(entityModel));
            Assert.That(eventArgs.OldValue, Is.EqualTo("Old name"));
            Assert.That(eventArgs.NewValue, Is.EqualTo("New name"));
        }

        [Test]
        public void AddChildEntity_ShouldAddNewChildEntityAndNotifyWithEvent()
        {
            // Arrange
            var entity = new Entity();
            var entityModel = new EntityModel(entity);

            object eventSender = null;
            EntityAddedEventArgs eventArgs = null;
            entityModel.EntityAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            entityModel.AddChildEntity();

            // Assert
            Assert.That(entity.Children, Has.Count.EqualTo(1));
            Assert.That(entityModel.Children, Has.Count.EqualTo(1));

            var childEntity = entity.Children.Single();
            var childEntityModel = entityModel.Children.Single();
            Assert.That(childEntity.Name, Is.EqualTo("Child entity 1"));
            Assert.That(childEntityModel.Name, Is.EqualTo("Child entity 1"));

            Assert.That(eventSender, Is.EqualTo(entityModel));
            Assert.That(eventArgs.EntityModel, Is.EqualTo(childEntityModel));
        }

        [Test]
        public void AddChildEntity_ShouldAddChildEntitiesWithIncrementingDefaultNames_WhenEntityInitiallyHasNoChildren()
        {
            // Arrange
            var entity = new Entity();
            var entityModel = new EntityModel(entity);

            // Act
            entityModel.AddChildEntity();
            entityModel.AddChildEntity();
            entityModel.AddChildEntity();

            // Assert
            Assert.That(entityModel.Children.Select(e => e.Name), Is.EquivalentTo(new[] {"Child entity 1", "Child entity 2", "Child entity 3"}));
        }

        [Test]
        public void AddTransformComponent_ShouldAddTransformComponentAndNotifyWithEvent()
        {
            // Arrange
            var entity = new Entity();
            var entityModel = new EntityModel(entity);

            object eventSender = null;
            ComponentAddedEventArgs eventArgs = null;
            entityModel.ComponentAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            entityModel.AddTransformComponent();

            // Assert
            Assert.That(entity.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));

            var transformComponent = entity.Components.Single();
            var transformComponentModel = entityModel.Components.Single();
            Assert.That(transformComponent, Is.TypeOf<TransformComponent>());
            Assert.That(transformComponentModel, Is.TypeOf<TransformComponentModel>());

            // Assert that created component model is bound to component
            ((TransformComponentModel) transformComponentModel).TranslationX = 123;
            Assert.That(((TransformComponent) transformComponent).Translation.X, Is.EqualTo(123));

            Assert.That(eventSender, Is.EqualTo(entityModel));
            Assert.That(eventArgs.ComponentModel, Is.EqualTo(transformComponentModel));
        }

        [Test]
        public void AddTransformComponent_ShouldAddTransformComponentWithDefaultValues()
        {
            // Arrange
            var entity = new Entity();
            var entityModel = new EntityModel(entity);

            // Act
            entityModel.AddTransformComponent();

            // Assert
            var transformComponentModel = (TransformComponentModel) entityModel.Components.Single();
            Assert.That(transformComponentModel.TranslationX, Is.Zero);
            Assert.That(transformComponentModel.TranslationY, Is.Zero);
            Assert.That(transformComponentModel.TranslationZ, Is.Zero);
            Assert.That(transformComponentModel.RotationX, Is.Zero);
            Assert.That(transformComponentModel.RotationY, Is.Zero);
            Assert.That(transformComponentModel.RotationZ, Is.Zero);
            Assert.That(transformComponentModel.ScaleX, Is.EqualTo(1));
            Assert.That(transformComponentModel.ScaleY, Is.EqualTo(1));
            Assert.That(transformComponentModel.ScaleZ, Is.EqualTo(1));
        }
    }
}