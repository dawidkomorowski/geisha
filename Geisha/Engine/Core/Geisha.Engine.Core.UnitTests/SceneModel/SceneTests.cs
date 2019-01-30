using System.Linq;
using Geisha.Engine.Core.SceneModel;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.SceneModel
{
    [TestFixture]
    public class SceneTests
    {
        [Test]
        public void AddEntity_ShouldAddNewRootEntity()
        {
            // Arrange
            var scene = new Scene();
            var entity = new Entity();

            // Act
            scene.AddEntity(entity);

            // Assert
            Assert.That(scene.RootEntities, Is.EquivalentTo(new[] {entity}));
        }

        [Test]
        public void AddEntity_ShouldSetSceneOnEntity()
        {
            // Arrange
            var scene = new Scene();
            var entity = new Entity();

            // Act
            scene.AddEntity(entity);

            // Assert
            Assert.That(entity.Scene, Is.EqualTo(scene));
        }

        [Test]
        public void RemoveEntity_ShouldRemoveEntityFromRootEntities()
        {
            // Arrange
            var scene = new Scene();
            var entity = new Entity();

            scene.AddEntity(entity);

            // Act
            scene.RemoveEntity(entity);

            // Assert
            Assert.That(scene.RootEntities, Is.Empty);
        }

        [Test]
        public void RemoveEntity_ShouldUnsetSceneOnEntity()
        {
            // Arrange
            var scene = new Scene();
            var entity = new Entity();

            scene.AddEntity(entity);

            // Act
            scene.RemoveEntity(entity);

            // Assert
            Assert.That(entity.Scene, Is.Null);
        }

        [Test]
        public void RemoveEntity_ShouldRemoveChildOfOtherEntity_WhenEntityIsNotRoot()
        {
            // Arrange
            var scene = new Scene();

            var rootEntity = new Entity();
            var childOfRoot = new Entity {Parent = rootEntity};

            scene.AddEntity(rootEntity);

            // Act
            scene.RemoveEntity(childOfRoot);

            // Assert
            Assert.That(scene.AllEntities.Count(), Is.EqualTo(1));
            Assert.That(scene.AllEntities, Does.Not.Contains(childOfRoot));
            Assert.That(rootEntity.Children, Has.Count.EqualTo(0));
            Assert.That(childOfRoot.Parent, Is.Null);
            Assert.That(childOfRoot.Scene, Is.Null);
        }

        [Test]
        public void AllEntities_ShouldReturnAllEntitiesInWholeSceneGraph()
        {
            // Arrange
            var scene = new Scene();

            var rootEntity1 = new Entity();
            var rootEntity2 = new Entity();
            var child1OfRoot1 = new Entity {Parent = rootEntity1};
            var child2OfRoot1 = new Entity {Parent = rootEntity1};
            var child1OfRoot2 = new Entity {Parent = rootEntity2};
            var child1OfChild1OfRoot1 = new Entity {Parent = child1OfRoot1};

            scene.AddEntity(rootEntity1);
            scene.AddEntity(rootEntity2);

            // Act
            var allEntities = scene.AllEntities;

            // Assert
            Assert.That(allEntities, Is.EquivalentTo(new[] {rootEntity1, rootEntity2, child1OfRoot1, child2OfRoot1, child1OfRoot2, child1OfChild1OfRoot1}));
        }
    }
}