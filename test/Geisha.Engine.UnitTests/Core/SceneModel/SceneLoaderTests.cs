using Geisha.Common.FileSystem;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel
{
    [TestFixture]
    public class SceneLoaderTests
    {
        private IFileSystem _fileSystem;
        private IJsonSerializer _jsonSerializer;
        private ISerializableSceneMapper _serializableSceneMapper;
        private SceneLoader _sceneLoader;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = Substitute.For<IFileSystem>();
            _jsonSerializer = Substitute.For<IJsonSerializer>();
            _serializableSceneMapper = Substitute.For<ISerializableSceneMapper>();
            _sceneLoader = new SceneLoader(_fileSystem, _jsonSerializer, _serializableSceneMapper);
        }

        [Test]
        public void Save_ShouldSaveSceneToAFile()
        {
            // Arrange
            const string path = "Some/Path";

            var scene = new Scene();
            var serializableScene = new SerializableScene();
            const string json = "serialized data";

            _serializableSceneMapper.MapToSerializable(scene).Returns(serializableScene);
            var file = Substitute.For<IFile>();
            _fileSystem.CreateFile(path).Returns(file);
            _jsonSerializer.Serialize(serializableScene).Returns(json);

            // Act
            _sceneLoader.Save(scene, path);

            // Assert
            file.Received(1).WriteAllText(json);
        }

        [Test]
        public void Load_ShouldLoadSceneFromAFile()
        {
            // Arrange
            const string path = "Some/Path";

            var scene = new Scene();
            var serializableScene = new SerializableScene();
            const string json = "serialized data";

            var file = Substitute.For<IFile>();
            file.ReadAllText().Returns(json);
            _fileSystem.GetFile(path).Returns(file);
            _jsonSerializer.Deserialize<SerializableScene>(json).Returns(serializableScene);
            _serializableSceneMapper.MapFromSerializable(serializableScene).Returns(scene);

            // Act
            var actual = _sceneLoader.Load(path);

            // Assert
            Assert.That(actual, Is.EqualTo(scene));
        }
    }
}