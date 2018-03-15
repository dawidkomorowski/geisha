using Geisha.Engine.Core.SceneModel;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.SceneModel
{
    [TestFixture]
    public class SceneDefinitionMapperTests
    {
        [Test]
        public void ToDefinition_ShouldReturnEmptySceneDefinition_GivenEmptyScene()
        {
            // Arrange
            var sceneDefinitionMapper = new SceneDefinitionMapper();
            var scene = new Scene();

            // Act
            var actual = sceneDefinitionMapper.ToDefinition(scene);

            // Assert
            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public void FromDefinition_ShouldReturnEmptyScene_GivenEmptySceneDefinition()
        {
            // Arrange
            var sceneDefinitionMapper = new SceneDefinitionMapper();
            var sceneDefinition = new SceneDefinition();

            // Act
            var actual = sceneDefinitionMapper.FromDefinition(sceneDefinition);

            // Assert
            Assert.That(actual, Is.Not.Null);
        }
    }
}