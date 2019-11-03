﻿using System;
using System.Linq;
using Geisha.Common.Math;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.Model
{
    [TestFixture]
    public class EntityModelTests
    {
        #region Constructor tests

        [Test]
        public void Constructor_ShouldThrowException_GivenEntityWithNotSupportedComponentType()
        {
            // Arrange
            var entity = new Entity();
            entity.AddComponent(Substitute.For<IComponent>());

            // Act
            // Assert
            Assert.That(() => new EntityModel(entity), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Constructor_ShouldCreateEntityModelWithTransformComponent()
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
        public void Constructor_ShouldCreateEntityModelWithCircleColliderComponent()
        {
            // Arrange
            var entity = new Entity();
            entity.AddComponent(new CircleColliderComponent());

            // Act
            var entityModel = new EntityModel(entity);

            // Assert
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components.Single().Name, Is.EqualTo("Circle Collider Component"));
        }

        [Test]
        public void Constructor_ShouldCreateEntityModelWithRectangleColliderComponent()
        {
            // Arrange
            var entity = new Entity();
            entity.AddComponent(new RectangleColliderComponent());

            // Act
            var entityModel = new EntityModel(entity);

            // Assert
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components.Single().Name, Is.EqualTo("Rectangle Collider Component"));
        }

        #endregion

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
            ((TransformComponentModel) transformComponentModel).Translation = new Vector3(123, 456, 789);
            Assert.That(((TransformComponent) transformComponent).Translation, Is.EqualTo(new Vector3(123, 456, 789)));

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
            Assert.That(transformComponentModel.Translation, Is.EqualTo(Vector3.Zero));
            Assert.That(transformComponentModel.Rotation, Is.EqualTo(Vector3.Zero));
            Assert.That(transformComponentModel.Scale, Is.EqualTo(Vector3.One));
        }

        [Test]
        public void AddCircleColliderComponent_ShouldAddCircleColliderComponentAndNotifyWithEvent()
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
            entityModel.AddCircleColliderComponent();

            // Assert
            Assert.That(entity.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));

            var circleColliderComponent = entity.Components.Single();
            var circleColliderComponentModel = entityModel.Components.Single();
            Assert.That(circleColliderComponent, Is.TypeOf<CircleColliderComponent>());
            Assert.That(circleColliderComponentModel, Is.TypeOf<CircleColliderComponentModel>());

            // Assert that created component model is bound to component
            ((CircleColliderComponentModel) circleColliderComponentModel).Radius = 123;
            Assert.That(((CircleColliderComponent) circleColliderComponent).Radius, Is.EqualTo(123));

            Assert.That(eventSender, Is.EqualTo(entityModel));
            Assert.That(eventArgs.ComponentModel, Is.EqualTo(circleColliderComponentModel));
        }

        [Test]
        public void AddRectangleColliderComponent_ShouldAddRectangleColliderComponentAndNotifyWithEvent()
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
            entityModel.AddRectangleColliderComponent();

            // Assert
            Assert.That(entity.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));

            var rectangleColliderComponent = entity.Components.Single();
            var rectangleColliderComponentModel = entityModel.Components.Single();
            Assert.That(rectangleColliderComponent, Is.TypeOf<RectangleColliderComponent>());
            Assert.That(rectangleColliderComponentModel, Is.TypeOf<RectangleColliderComponentModel>());

            // Assert that created component model is bound to component
            ((RectangleColliderComponentModel) rectangleColliderComponentModel).Dimension = new Vector2(123, 456);
            Assert.That(((RectangleColliderComponent) rectangleColliderComponent).Dimension, Is.EqualTo(new Vector2(123, 456)));

            Assert.That(eventSender, Is.EqualTo(entityModel));
            Assert.That(eventArgs.ComponentModel, Is.EqualTo(rectangleColliderComponentModel));
        }
    }
}