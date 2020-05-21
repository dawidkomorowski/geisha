using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel.Serialization
{
    [TestFixture]
    public class SerializableSceneMapperTests
    {
        private ISerializableEntityMapper _serializableEntityMapper = null!;
        private SerializableSceneMapper _serializableSceneMapper = null!;

        [SetUp]
        public void SetUp()
        {
            _serializableEntityMapper = Substitute.For<ISerializableEntityMapper>();
            _serializableSceneMapper = new SerializableSceneMapper(_serializableEntityMapper);
        }

        #region MapToSerializable

        [Test]
        public void MapToSerializable_ShouldReturnEmptySerializableScene_GivenEmptyScene()
        {
            // Arrange
            var scene = new Scene();

            // Act
            var actual = _serializableSceneMapper.MapToSerializable(scene);

            // Assert
            Assert.That(actual.RootEntities, Has.Count.Zero);
            Assert.That(actual.ConstructionScript, Is.Empty);
        }

        [Test]
        public void MapToSerializable_ShouldReturnSerializableSceneWithRootEntities_GivenSceneWithRootEntities()
        {
            // Arrange
            var entity1 = new Entity();
            var entity2 = new Entity();
            var entity3 = new Entity();

            var scene = new Scene();
            scene.AddEntity(entity1);
            scene.AddEntity(entity2);
            scene.AddEntity(entity3);

            var serializableEntity1 = new SerializableEntity();
            var serializableEntity2 = new SerializableEntity();
            var serializableEntity3 = new SerializableEntity();

            _serializableEntityMapper.MapToSerializable(entity1).Returns(serializableEntity1);
            _serializableEntityMapper.MapToSerializable(entity2).Returns(serializableEntity2);
            _serializableEntityMapper.MapToSerializable(entity3).Returns(serializableEntity3);

            // Act
            var actual = _serializableSceneMapper.MapToSerializable(scene);

            // Assert
            Assert.That(actual.RootEntities, Has.Count.EqualTo(3));
            Assert.That(actual.RootEntities.ElementAt(0), Is.EqualTo(serializableEntity1));
            Assert.That(actual.RootEntities.ElementAt(1), Is.EqualTo(serializableEntity2));
            Assert.That(actual.RootEntities.ElementAt(2), Is.EqualTo(serializableEntity3));
        }

        [Test]
        public void MapToSerializable_ShouldReturnSerializableSceneWithConstructionScript_GivenSceneWithConstructionScript()
        {
            // Arrange
            var constructionScriptName = Guid.NewGuid().ToString();

            var scene = new Scene {ConstructionScript = constructionScriptName};

            // Act
            var actual = _serializableSceneMapper.MapToSerializable(scene);

            // Assert
            Assert.That(actual.ConstructionScript, Is.EqualTo(constructionScriptName));
        }

        #endregion

        #region MapFromSerializable

        [Test]
        public void MapFromSerializable_ShouldReturnEmptyScene_GivenEmptySerializableScene()
        {
            // Arrange
            var serializableScene = new SerializableScene
            {
                RootEntities = new List<SerializableEntity>(),
                ConstructionScript = string.Empty
            };

            // Act
            var actual = _serializableSceneMapper.MapFromSerializable(serializableScene);

            // Assert
            Assert.That(actual.RootEntities, Has.Count.Zero);
            Assert.That(actual.ConstructionScript, Is.Empty);
        }

        [Test]
        public void MapFromSerializable_ShouldReturnSceneWithRootEntities_GivenSerializableSceneWithRootEntities()
        {
            // Arrange
            var serializableEntity1 = new SerializableEntity();
            var serializableEntity2 = new SerializableEntity();
            var serializableEntity3 = new SerializableEntity();

            var serializableScene = new SerializableScene
            {
                RootEntities = new List<SerializableEntity>
                {
                    serializableEntity1,
                    serializableEntity2,
                    serializableEntity3
                },
                ConstructionScript = string.Empty
            };

            var entity1 = new Entity();
            var entity2 = new Entity();
            var entity3 = new Entity();

            _serializableEntityMapper.MapFromSerializable(serializableEntity1).Returns(entity1);
            _serializableEntityMapper.MapFromSerializable(serializableEntity2).Returns(entity2);
            _serializableEntityMapper.MapFromSerializable(serializableEntity3).Returns(entity3);

            // Act
            var actual = _serializableSceneMapper.MapFromSerializable(serializableScene);

            // Assert
            Assert.That(actual.RootEntities, Has.Count.EqualTo(3));
            Assert.That(actual.RootEntities.ElementAt(0), Is.EqualTo(entity1));
            Assert.That(actual.RootEntities.ElementAt(1), Is.EqualTo(entity2));
            Assert.That(actual.RootEntities.ElementAt(2), Is.EqualTo(entity3));
        }

        [Test]
        public void MapFromSerializable_ShouldReturnSceneWithConstructionScript_GivenSerializableSceneWithConstructionScript()
        {
            // Arrange
            var constructionScriptName = Guid.NewGuid().ToString();

            var serializableScene = new SerializableScene
            {
                RootEntities = new List<SerializableEntity>(),
                ConstructionScript = constructionScriptName
            };

            // Act
            var actual = _serializableSceneMapper.MapFromSerializable(serializableScene);

            // Assert
            Assert.That(actual.ConstructionScript, Is.EqualTo(constructionScriptName));
        }

        #endregion
    }
}