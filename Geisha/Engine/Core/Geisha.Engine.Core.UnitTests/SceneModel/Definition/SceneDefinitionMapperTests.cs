using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Definition;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.SceneModel.Definition
{
    [TestFixture]
    public class SceneDefinitionMapperTests
    {
        private IEntityDefinitionMapper _entityDefinitionMapper;
        private SceneDefinitionMapper _sceneDefinitionMapper;

        [SetUp]
        public void SetUp()
        {
            _entityDefinitionMapper = Substitute.For<IEntityDefinitionMapper>();
            _sceneDefinitionMapper = new SceneDefinitionMapper(_entityDefinitionMapper);
        }

        #region ToDefinition

        [Test]
        public void ToDefinition_ShouldReturnEmptySceneDefinition_GivenEmptyScene()
        {
            // Arrange
            var scene = new Scene();

            // Act
            var actual = _sceneDefinitionMapper.ToDefinition(scene);

            // Assert
            Assert.That(actual.RootEntities, Has.Count.Zero);
        }

        [Test]
        public void ToDefinition_ShouldReturnSceneDefinitionWithRootEntities_GivenSceneWithRootEntities()
        {
            // Arrange
            var entity1 = new Entity();
            var entity2 = new Entity();
            var entity3 = new Entity();

            var scene = new Scene();
            scene.AddEntity(entity1);
            scene.AddEntity(entity2);
            scene.AddEntity(entity3);

            var entityDefinition1 = new EntityDefinition();
            var entityDefinition2 = new EntityDefinition();
            var entityDefinition3 = new EntityDefinition();

            _entityDefinitionMapper.ToDefinition(entity1).Returns(entityDefinition1);
            _entityDefinitionMapper.ToDefinition(entity2).Returns(entityDefinition2);
            _entityDefinitionMapper.ToDefinition(entity3).Returns(entityDefinition3);

            // Act
            var actual = _sceneDefinitionMapper.ToDefinition(scene);

            // Assert
            Assert.That(actual.RootEntities, Has.Count.EqualTo(3));
            Assert.That(actual.RootEntities.ElementAt(0), Is.EqualTo(entityDefinition1));
            Assert.That(actual.RootEntities.ElementAt(1), Is.EqualTo(entityDefinition2));
            Assert.That(actual.RootEntities.ElementAt(2), Is.EqualTo(entityDefinition3));
        }

        #endregion

        #region FromDefinition

        [Test]
        public void FromDefinition_ShouldReturnEmptyScene_GivenEmptySceneDefinition()
        {
            // Arrange
            var sceneDefinition = new SceneDefinition
            {
                RootEntities = new List<EntityDefinition>()
            };

            // Act
            var actual = _sceneDefinitionMapper.FromDefinition(sceneDefinition);

            // Assert
            Assert.That(actual.RootEntities, Has.Count.Zero);
        }

        [Test]
        public void FromDefinition_ShouldReturnSceneWithRootEntities_GivenSceneDefinitionWithRootEntities()
        {
            // Arrange
            var entityDefinition1 = new EntityDefinition();
            var entityDefinition2 = new EntityDefinition();
            var entityDefinition3 = new EntityDefinition();

            var sceneDefinition = new SceneDefinition
            {
                RootEntities = new List<EntityDefinition>
                {
                    entityDefinition1,
                    entityDefinition2,
                    entityDefinition3
                }
            };

            var entity1 = new Entity();
            var entity2 = new Entity();
            var entity3 = new Entity();

            _entityDefinitionMapper.FromDefinition(entityDefinition1).Returns(entity1);
            _entityDefinitionMapper.FromDefinition(entityDefinition2).Returns(entity2);
            _entityDefinitionMapper.FromDefinition(entityDefinition3).Returns(entity3);

            // Act
            var actual = _sceneDefinitionMapper.FromDefinition(sceneDefinition);

            // Assert
            Assert.That(actual.RootEntities, Has.Count.EqualTo(3));
            Assert.That(actual.RootEntities.ElementAt(0), Is.EqualTo(entity1));
            Assert.That(actual.RootEntities.ElementAt(1), Is.EqualTo(entity2));
            Assert.That(actual.RootEntities.ElementAt(2), Is.EqualTo(entity3));
        }

        #endregion
    }
}