using System.IO;
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
        private SceneSerializer _sceneSerializer = null!;

        protected abstract Scene SerializeAndDeserialize(Scene scene);

        [SetUp]
        public void SetUp()
        {
            _sceneFactory = Substitute.For<ISceneFactory>();
            _sceneFactory.Create().Returns(TestSceneFactory.Create());
            _sceneSerializer = new SceneSerializer(_sceneFactory);
        }

        [Test]
        public void Serialize_and_Deserialize_ShouldSerializeAndDeserializeEmptyScene()
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