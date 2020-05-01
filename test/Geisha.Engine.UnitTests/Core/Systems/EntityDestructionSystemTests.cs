using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Systems
{
    [TestFixture]
    public class EntityDestructionSystemTests
    {
        private readonly EntityDestructionSystem _entityDestructionSystem = new EntityDestructionSystem();

        #region DestroyEntitiesAfterFixedTimeStep

        [Test]
        public void DestroyEntitiesAfterFixedTimeStep_ShouldRemoveEntityFromScene_WhenDestroyAfterFixedTimeStepIsExecutedForEntity()
        {
            // Arrange
            var scene = new Scene();
            var entity = new Entity();
            scene.AddEntity(entity);

            entity.DestroyAfterFixedTimeStep();

            // Assume
            Assert.That(scene.AllEntities, Contains.Item(entity));

            // Act
            _entityDestructionSystem.DestroyEntitiesAfterFixedTimeStep(scene);

            // Assert
            Assert.That(scene.AllEntities, Does.Not.Contains(entity));
        }

        [Test]
        public void DestroyEntitiesAfterFixedTimeStep_ShouldNotRemoveEntityFromScene_WhenNoDestroyMethodIsExecutedForEntity()
        {
            // Arrange
            var scene = new Scene();
            var entity = new Entity();
            scene.AddEntity(entity);

            // Assume
            Assert.That(scene.AllEntities, Contains.Item(entity));

            // Act
            _entityDestructionSystem.DestroyEntitiesAfterFixedTimeStep(scene);

            // Assert
            Assert.That(scene.AllEntities, Contains.Item(entity));
        }

        [Test]
        public void DestroyEntitiesAfterFixedTimeStep_ShouldNotRemoveEntityFromScene_WhenDestroyAfterFullFrameIsExecutedForEntity()
        {
            // Arrange
            var scene = new Scene();
            var entity = new Entity();
            scene.AddEntity(entity);

            entity.DestroyAfterFullFrame();

            // Assume
            Assert.That(scene.AllEntities, Contains.Item(entity));

            // Act
            _entityDestructionSystem.DestroyEntitiesAfterFixedTimeStep(scene);

            // Assert
            Assert.That(scene.AllEntities, Contains.Item(entity));
        }

        #endregion

        #region DestroyEntitiesAfterFixedTimeStep

        [Test]
        public void DestroyEntitiesAfterFullFrame_ShouldRemoveEntityFromScene_WhenDestroyAfterFullFrameIsExecutedForEntity()
        {
            // Arrange
            var scene = new Scene();
            var entity = new Entity();
            scene.AddEntity(entity);

            entity.DestroyAfterFullFrame();

            // Assume
            Assert.That(scene.AllEntities, Contains.Item(entity));

            // Act
            _entityDestructionSystem.DestroyEntitiesAfterFullFrame(scene);

            // Assert
            Assert.That(scene.AllEntities, Does.Not.Contains(entity));
        }

        [Test]
        public void DestroyEntitiesAfterFullFrame_ShouldNotRemoveEntityFromScene_WhenNoDestroyMethodIsExecutedForEntity()
        {
            // Arrange
            var scene = new Scene();
            var entity = new Entity();
            scene.AddEntity(entity);

            // Assume
            Assert.That(scene.AllEntities, Contains.Item(entity));

            // Act
            _entityDestructionSystem.DestroyEntitiesAfterFullFrame(scene);

            // Assert
            Assert.That(scene.AllEntities, Contains.Item(entity));
        }

        [Test]
        public void DestroyEntitiesAfterFullFrame_ShouldNotRemoveEntityFromScene_WhenDestroyAfterFixedTimeStepIsExecutedForEntity()
        {
            // Arrange
            var scene = new Scene();
            var entity = new Entity();
            scene.AddEntity(entity);

            entity.DestroyAfterFixedTimeStep();

            // Assume
            Assert.That(scene.AllEntities, Contains.Item(entity));

            // Act
            _entityDestructionSystem.DestroyEntitiesAfterFullFrame(scene);

            // Assert
            Assert.That(scene.AllEntities, Contains.Item(entity));
        }

        #endregion
    }
}