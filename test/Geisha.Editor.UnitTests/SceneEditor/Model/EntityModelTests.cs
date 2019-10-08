﻿using System.Linq;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Engine.Core.SceneModel;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.Model
{
    [TestFixture]
    public class EntityModelTests
    {
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
            Assert.That(entityModel.Children, Has.Count.EqualTo(1));
            Assert.That(entity.Children, Has.Count.EqualTo(1));

            var childEntityModel = entityModel.Children.Single();
            var childEntity = entity.Children.Single();
            Assert.That(childEntityModel.Name, Is.EqualTo("Child entity 1"));
            Assert.That(childEntity.Name, Is.EqualTo("Child entity 1"));

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
    }
}