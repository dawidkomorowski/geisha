using System;
using System.Linq;
using Geisha.Engine.Core.Components;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests
{
    [TestFixture]
    public class EntityTests
    {
        private static Entity NewEntity => new Entity();

        [Test]
        public void Constructor_ShouldInstantiateEntityWithNoParent()
        {
            // Arrange
            // Act
            var entity = new Entity();

            // Assert
            Assert.That(entity.Parent, Is.Null);
        }

        [Test]
        public void Constructor_ShouldInstantiateEntityWithNoComponents()
        {
            // Arrange
            // Act
            var entity = new Entity();

            // Assert
            Assert.That(entity.Components, Is.Empty);
        }

        [Test]
        public void Parent_ShouldBeCorrectlySet()
        {
            // Arrange
            var child = NewEntity;
            var parent = NewEntity;

            // Act
            child.Parent = parent;

            // Assert
            Assert.That(child.Parent, Is.EqualTo(parent));
        }

        [Test]
        public void Parent_ShouldAddThisEntityToChildrenOfNewParent_WhenSet()
        {
            // Arrange
            var child = NewEntity;
            var parent = NewEntity;

            // Act
            child.Parent = parent;

            // Assert
            Assert.That(parent.Children.Count, Is.EqualTo(1));
            Assert.That(parent.Children.Single(), Is.EqualTo(child));
        }

        [Test]
        public void Parent_ShouldRemoveThisEntityFromChildrenOfOldParent_WhenSet()
        {
            // Arrange
            var child = NewEntity;
            var oldParent = NewEntity;
            var newParent = NewEntity;

            child.Parent = oldParent;

            // Act
            child.Parent = newParent;

            // Assert
            Assert.That(oldParent.Children, Is.Empty);
        }

        [Test]
        public void Parent_CanBeSetToNull()
        {
            // Arrange
            var child = NewEntity;
            var parent = NewEntity;

            child.Parent = parent;

            // Act
            child.Parent = null;

            // Assert
            Assert.That(child.Parent, Is.Null);
        }

        [Test]
        public void IsRoot_ReturnsFalse_WhenParentIsNotNull()
        {
            // Arrange
            var child = NewEntity;
            var parent = NewEntity;

            // Act
            child.Parent = parent;

            // Assert
            Assert.That(child.IsRoot, Is.False);
        }

        [Test]
        public void IsRoot_ReturnsTrue_WhenParentIsNull()
        {
            // Arrange
            var child = NewEntity;

            // Act
            child.Parent = null;

            // Assert
            Assert.That(child.IsRoot, Is.True);
        }

        [Test]
        public void AddComponent_ShouldAddComponentToEntity()
        {
            // Arrange
            var entity = NewEntity;
            var componentA = new ComponentA();

            // Act
            entity.AddComponent(componentA);

            // Assert
            Assert.That(entity.Components.Count, Is.EqualTo(1));
            Assert.That(entity.Components.Single(), Is.EqualTo(componentA));
        }

        [Test]
        public void RemoveComponent_ShouldRemoveComponentFromEntity()
        {
            // Arrange
            var entity = NewEntity;
            var componentA = new ComponentA();
            entity.AddComponent(componentA);

            // Act
            entity.RemoveComponent(componentA);

            // Assert
            Assert.That(entity.Components, Is.Empty);
        }

        [Test]
        public void GetComponent_ShouldReturnComponentByTypeFromEntity()
        {
            // Arrange
            var entity = NewEntity;
            var componentA = new ComponentA();
            entity.AddComponent(componentA);

            // Act
            var component = entity.GetComponent<ComponentA>();

            // Assert
            Assert.That(component, Is.EqualTo(componentA));
        }

        [Test]
        public void GetComponent_ShouldReturn_ComponentA_ByType_WhenThereIs_ComponentA_and_ComponentB_InEntity()
        {
            // Arrange
            var entity = NewEntity;
            var componentA = new ComponentA();
            var componentB = new ComponentB();
            entity.AddComponent(componentA);
            entity.AddComponent(componentB);

            // Act
            var component = entity.GetComponent<ComponentA>();

            // Assert
            Assert.That(component, Is.EqualTo(componentA));
        }

        [Test]
        public void GetComponent_ShouldThrowException_WhenTryToGetComponentByTypeAndThereIsNoComponentInEntity()
        {
            // Arrange
            var entity = NewEntity;

            // Act
            // Assert
            Assert.Throws<InvalidOperationException>(() => entity.GetComponent<ComponentA>());
        }

        [Test]
        public void HasComponent_ShouldReturnTrue_WhenAskedFor_ComponentA_and_ThereIs_ComponentA_InEntity()
        {
            // Arrange
            var entity = NewEntity;
            var componentA = new ComponentA();
            entity.AddComponent(componentA);

            // Act
            var actual = entity.HasComponent<ComponentA>();

            // Assert
            Assert.That(actual, Is.True);
        }

        [Test]
        public void HasComponent_ShouldReturnFalse_WhenAskedFor_ComponentA_and_ThereIsNo_ComponentA_InEntity()
        {
            // Arrange
            var entity = NewEntity;

            // Act
            var actual = entity.HasComponent<ComponentA>();

            // Assert
            Assert.That(actual, Is.False);
        }

        private class ComponentA : IComponent
        {
        }

        private class ComponentB : IComponent
        {
        }
    }
}