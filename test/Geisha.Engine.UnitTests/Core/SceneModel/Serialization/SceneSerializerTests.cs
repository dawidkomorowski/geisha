using System;
using System.IO;
using System.Linq;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel.Serialization
{
    [TestFixture]
    public abstract class SceneSerializerTests
    {
        private ISceneFactory _sceneFactory = null!;
        private ISceneBehaviorFactoryProvider _sceneBehaviorFactoryProvider = null!;
        private SceneSerializer _sceneSerializer = null!;

        protected abstract Scene SerializeAndDeserialize(Scene scene);

        [SetUp]
        public void SetUp()
        {
            _sceneFactory = Substitute.For<ISceneFactory>();
            _sceneFactory.Create().Returns(ci => TestSceneFactory.Create());

            _sceneBehaviorFactoryProvider = Substitute.For<ISceneBehaviorFactoryProvider>();
            var emptySceneBehaviorFactory = Substitute.For<ISceneBehaviorFactory>();
            emptySceneBehaviorFactory.BehaviorName.Returns(string.Empty);
            emptySceneBehaviorFactory.Create(Arg.Any<Scene>())
                .Returns(ci => SceneBehavior.CreateEmpty(ci.Arg<Scene>()));
            _sceneBehaviorFactoryProvider.Get(string.Empty).Returns(emptySceneBehaviorFactory);

            _sceneSerializer = new SceneSerializer(_sceneFactory, _sceneBehaviorFactoryProvider);
        }

        [Test]
        public void Serialize_and_Deserialize_EmptyScene()
        {
            // Arrange
            var scene = TestSceneFactory.Create();

            // Act
            var actual = SerializeAndDeserialize(scene);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.RootEntities, Has.Count.Zero);
            Assert.That(actual.SceneBehavior.Name, Is.EqualTo(SceneBehavior.CreateEmpty(actual).Name));
        }

        [Test]
        public void Serialize_and_Deserialize_SceneWithSceneBehavior()
        {
            // Arrange
            var sceneBehaviorName = Guid.NewGuid().ToString();

            var emptyScene = TestSceneFactory.Create();
            _sceneFactory.Create().Returns(emptyScene);

            var sceneBehavior = Substitute.ForPartsOf<SceneBehavior>(emptyScene);
            sceneBehavior.Name.Returns(sceneBehaviorName);

            var sceneBehaviorFactory = Substitute.For<ISceneBehaviorFactory>();
            sceneBehaviorFactory.BehaviorName.Returns(sceneBehaviorName);
            sceneBehaviorFactory.Create(emptyScene).Returns(sceneBehavior);

            _sceneBehaviorFactoryProvider.Get(sceneBehaviorName).Returns(sceneBehaviorFactory);

            // Prepare scene to serialize
            var sceneToSerialize = TestSceneFactory.Create();

            var sceneBehaviorToSerialize = Substitute.ForPartsOf<SceneBehavior>(sceneToSerialize);
            sceneBehaviorToSerialize.Name.Returns(sceneBehaviorName);

            sceneToSerialize.SceneBehavior = sceneBehaviorToSerialize;

            // Act
            var actual = SerializeAndDeserialize(sceneToSerialize);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.RootEntities, Has.Count.Zero);
            Assert.That(actual.SceneBehavior.Name, Is.EqualTo(sceneBehaviorName));
        }

        [Test]
        public void Serialize_and_Deserialize_SceneWithRootEntities()
        {
            // Arrange
            var scene = TestSceneFactory.Create();
            scene.AddEntity(new Entity());
            scene.AddEntity(new Entity());
            scene.AddEntity(new Entity());

            // Act
            var actual = SerializeAndDeserialize(scene);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.RootEntities, Has.Count.EqualTo(3));
        }

        [Test]
        public void Serialize_and_Deserialize_SceneWithEntityWithName()
        {
            // Arrange
            var entity = new Entity
            {
                Name = "Entity Name"
            };

            var scene = TestSceneFactory.Create();
            scene.AddEntity(entity);

            // Act
            var actual = SerializeAndDeserialize(scene);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.RootEntities, Has.Count.EqualTo(1));
            Assert.That(actual.RootEntities.Single().Name, Is.EqualTo("Entity Name"));
        }

        [Test]
        public void Serialize_and_Deserialize_SceneWithEntityWithChildren()
        {
            // Arrange
            var entity = new Entity();
            entity.AddChild(new Entity());
            entity.AddChild(new Entity());
            entity.AddChild(new Entity());

            var scene = TestSceneFactory.Create();
            scene.AddEntity(entity);

            // Act
            var actual = SerializeAndDeserialize(scene);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.RootEntities, Has.Count.EqualTo(1));
            Assert.That(actual.RootEntities.Single().Children, Has.Count.EqualTo(3));
        }

        [TestFixture]
        public sealed class SceneSerializerTestsUsingStream : SceneSerializerTests
        {
            protected override Scene SerializeAndDeserialize(Scene scene)
            {
                using var memoryStream = new MemoryStream();
                _sceneSerializer.Serialize(scene, memoryStream);
                memoryStream.Position = 0;
                return _sceneSerializer.Deserialize(memoryStream);
            }
        }

        [TestFixture]
        public sealed class SceneSerializerTestsUsingString : SceneSerializerTests
        {
            protected override Scene SerializeAndDeserialize(Scene scene)
            {
                var json = _sceneSerializer.Serialize(scene);
                return _sceneSerializer.Deserialize(json);
            }
        }
    }
}