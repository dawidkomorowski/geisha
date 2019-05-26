using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Systems
{
    [TestFixture]
    public class EntityDestructionSystemTests
    {
        private readonly EntityDestructionSystem _entityDestructionSystem = new EntityDestructionSystem();

        [Test]
        public void FixedUpdate_ShouldRemoveEntityFromScene_WhenItIsScheduledForDestruction()
        {
            // Arrange
            var scene = new Scene();
            var entity = new Entity();
            scene.AddEntity(entity);

            entity.Destroy();

            // Assume
            Assume.That(entity.IsScheduledForDestruction, Is.True);
            Assert.That(scene.AllEntities, Contains.Item(entity));

            // Act
            _entityDestructionSystem.FixedUpdate(scene);

            // Assert
            Assert.That(scene.AllEntities, Does.Not.Contains(entity));
        }

        [Test]
        public void FixedUpdate_ShouldNotRemoveEntityFromScene_WhenItIsNotScheduledForDestruction()
        {
            // Arrange
            var scene = new Scene();
            var entity = new Entity();
            scene.AddEntity(entity);

            // Assume
            Assume.That(entity.IsScheduledForDestruction, Is.False);
            Assert.That(scene.AllEntities, Contains.Item(entity));

            // Act
            _entityDestructionSystem.FixedUpdate(scene);

            // Assert
            Assert.That(scene.AllEntities, Contains.Item(entity));
        }
    }
}