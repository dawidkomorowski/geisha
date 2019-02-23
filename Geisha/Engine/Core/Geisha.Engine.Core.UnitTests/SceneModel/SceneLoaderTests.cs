using Geisha.Common.Serialization;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Framework.FileSystem;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.SceneModel
{
    [TestFixture]
    public class SceneLoaderTests
    {
        private IFileSystem _fileSystem;
        private ISerializableSceneMapper _serializableSceneMapper;
        private SceneLoader _sceneLoader;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = Substitute.For<IFileSystem>();
            _serializableSceneMapper = Substitute.For<ISerializableSceneMapper>();
            _sceneLoader = new SceneLoader(_fileSystem, _serializableSceneMapper);
        }

        [Test]
        public void Save_ShouldSaveSceneToAFile()
        {
            // Arrange
            const string path = "Some/Path";

            var scene = new Scene();
            var serializableScene = new SerializableScene();
            var serializedSerializableScene = Serializer.SerializeJson(serializableScene);

            _serializableSceneMapper.MapToSerializable(scene).Returns(serializableScene);
            var file = Substitute.For<IFile>();
            _fileSystem.CreateFile(path).Returns(file);

            // Act
            _sceneLoader.Save(scene, path);

            // Assert
            file.Received(1).WriteAllText(serializedSerializableScene);
        }

        [Test]
        public void Load_ShouldLoadSceneFromAFile()
        {
            // Arrange
            const string path = "Some/Path";

            var scene = new Scene();
            var serializableScene = new SerializableScene();
            var serializedSerializableScene = Serializer.SerializeJson(serializableScene);

            var file = Substitute.For<IFile>();
            file.ReadAllText().Returns(serializedSerializableScene);
            _fileSystem.GetFile(path).Returns(file);
            _serializableSceneMapper.MapFromSerializable(Arg.Is<SerializableScene>(ss => Serializer.SerializeJson(ss) == serializedSerializableScene))
                .Returns(scene);

            // Act
            var actual = _sceneLoader.Load(path);

            // Assert
            Assert.That(actual, Is.EqualTo(scene));
        }
    }
}