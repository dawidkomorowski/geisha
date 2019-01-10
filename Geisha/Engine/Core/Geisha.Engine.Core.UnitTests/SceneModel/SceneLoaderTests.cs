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
        private ISceneDefinitionMapper _sceneDefinitionMapper;
        private SceneLoader _sceneLoader;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = Substitute.For<IFileSystem>();
            _sceneDefinitionMapper = Substitute.For<ISceneDefinitionMapper>();
            _sceneLoader = new SceneLoader(_fileSystem, _sceneDefinitionMapper);
        }

        [Test]
        public void Save_ShouldSaveSceneToAFile()
        {
            // Arrange
            const string path = "Some/Path";

            var scene = new Scene();
            var sceneDefinition = new SceneDefinition();
            var serializedSceneDefinition = Serializer.SerializeJson(sceneDefinition);

            _sceneDefinitionMapper.ToDefinition(scene).Returns(sceneDefinition);

            // Act
            _sceneLoader.Save(scene, path);

            // Assert
            _fileSystem.Received(1).WriteAllTextToFile(path, serializedSceneDefinition);
        }

        [Test]
        public void Load_ShouldLoadSceneFromAFile()
        {
            // Arrange
            const string path = "Some/Path";

            var scene = new Scene();
            var sceneDefinition = new SceneDefinition();
            var serializedSceneDefinition = Serializer.SerializeJson(sceneDefinition);

            _fileSystem.ReadAllTextFromFile(path).Returns(serializedSceneDefinition);
            _sceneDefinitionMapper.FromDefinition(Arg.Is<SceneDefinition>(sd => Serializer.SerializeJson(sd) == serializedSceneDefinition)).Returns(scene);

            // Act
            var actual = _sceneLoader.Load(path);

            // Assert
            Assert.That(actual, Is.EqualTo(scene));
        }
    }
}