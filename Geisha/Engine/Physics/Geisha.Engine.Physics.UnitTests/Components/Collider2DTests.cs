﻿using System.Linq;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.Physics.UnitTests.Components
{
    [TestFixture]
    public class Collider2DTests
    {
        private class TestCollider2D : Collider2D
        {
        }

        [Test]
        public void Constructor_ShouldCreateColliderThatIsNotColliding()
        {
            // Arrange
            // Act
            var collider2D = new TestCollider2D();

            // Assert
            Assert.That(collider2D.IsColliding, Is.False);
            Assert.That(collider2D.CollidingEntities, Is.Empty);
        }

        [Test]
        public void AddCollidingEntity_ShouldMakeEntityColliding()
        {
            // Arrange
            var collider2D = new TestCollider2D();
            var entity = new Entity();

            // Assume
            Assume.That(collider2D.IsColliding, Is.False);
            Assert.That(collider2D.CollidingEntities, Is.Empty);

            // Act
            collider2D.AddCollidingEntity(entity);

            // Assert
            Assert.That(collider2D.IsColliding, Is.True);
            Assert.That(collider2D.CollidingEntities, Has.Count.EqualTo(1));
            Assert.That(collider2D.CollidingEntities.Single(), Is.EqualTo(entity));
        }

        [Test]
        public void AddCollidingEntity_ShouldNotAddDuplicateEntities()
        {
            // Arrange
            var collider2D = new TestCollider2D();
            var entity = new Entity();

            // Assume
            Assert.That(collider2D.CollidingEntities, Is.Empty);

            // Act
            collider2D.AddCollidingEntity(entity);
            collider2D.AddCollidingEntity(entity);

            // Assert
            Assert.That(collider2D.CollidingEntities, Has.Count.EqualTo(1));
            Assert.That(collider2D.CollidingEntities.Single(), Is.EqualTo(entity));
        }

        [Test]
        public void ClearCollidingEntities_ShouldMakeEntityNotColliding()
        {
            // Arrange
            var collider2D = new TestCollider2D();
            var entity = new Entity();

            collider2D.AddCollidingEntity(entity);

            // Assume
            Assert.That(collider2D.IsColliding, Is.True);
            Assert.That(collider2D.CollidingEntities, Has.Count.EqualTo(1));
            Assert.That(collider2D.CollidingEntities.Single(), Is.EqualTo(entity));

            // Act
            collider2D.ClearCollidingEntities();

            // Assert
            Assume.That(collider2D.IsColliding, Is.False);
            Assert.That(collider2D.CollidingEntities, Is.Empty);
        }
    }
}