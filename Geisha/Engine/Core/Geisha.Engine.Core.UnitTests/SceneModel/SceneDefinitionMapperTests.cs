using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.SceneModel
{
    [TestFixture]
    public class SceneDefinitionMapperTests
    {
        #region ToDefinition

        [Test]
        public void ToDefinition_ShouldReturnEmptySceneDefinition_GivenEmptyScene()
        {
            // Arrange
            var sceneDefinitionMapper = new SceneDefinitionMapper();
            var scene = new Scene();

            // Act
            var actual = sceneDefinitionMapper.ToDefinition(scene);

            // Assert
            Assert.That(actual.RootEntities, Has.Count.Zero);
        }

        [Test]
        public void ToDefinition_ShouldReturnSceneDefinitionWithRootEntities_GivenSceneWithRootEntities()
        {
            // Arrange
            var sceneDefinitionMapper = new SceneDefinitionMapper();
            var scene = new Scene();
            scene.AddEntity(new Entity());
            scene.AddEntity(new Entity());
            scene.AddEntity(new Entity());

            // Act
            var actual = sceneDefinitionMapper.ToDefinition(scene);

            // Assert
            Assert.That(actual.RootEntities, Has.Count.EqualTo(3));
        }

        #endregion

        #region FromDefinition

        [Test]
        public void FromDefinition_ShouldReturnEmptyScene_GivenEmptySceneDefinition()
        {
            // Arrange
            var sceneDefinitionMapper = new SceneDefinitionMapper();
            var sceneDefinition = new SceneDefinition
            {
                RootEntities = new List<EntityDefinition>()
            };

            // Act
            var actual = sceneDefinitionMapper.FromDefinition(sceneDefinition);

            // Assert
            Assert.That(actual.RootEntities, Has.Count.Zero);
        }

        [Test]
        public void FromDefinition_ShouldReturnSceneWithRootEntities_GivenSceneDefinitionWithRootEntities()
        {
            // Arrange
            var sceneDefinitionMapper = new SceneDefinitionMapper();
            var sceneDefinition = new SceneDefinition
            {
                RootEntities = new List<EntityDefinition>
                {
                    new EntityDefinition(),
                    new EntityDefinition(),
                    new EntityDefinition()
                }
            };

            // Act
            var actual = sceneDefinitionMapper.FromDefinition(sceneDefinition);

            // Assert
            Assert.That(actual.RootEntities, Has.Count.EqualTo(3));
        }

        #endregion
    }
}