﻿using System;
using System.Linq;
using Geisha.Engine.Core.Components;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests
{
    [TestFixture]
    public class EntityTests
    {
        private IEntity NewEntity => new Entity();

        [Test]
        public void Constructor_ShouldInstantiateEntityWithNoComponents()
        {
            // Arrange
            // Act
            IEntity entity = new Entity();

            // Assert
            Assert.That(entity.Components, Is.Empty);
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