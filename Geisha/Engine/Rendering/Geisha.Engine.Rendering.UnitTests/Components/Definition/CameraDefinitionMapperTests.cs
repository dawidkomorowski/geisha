using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Components.Definition;
using NUnit.Framework;

namespace Geisha.Engine.Rendering.UnitTests.Components.Definition
{
    [TestFixture]
    public class CameraDefinitionMapperTests
    {
        [Test]
        public void ToDefinition()
        {
            // Arrange
            var mapper = new CameraDefinitionMapper();
            var camera = new CameraComponent();

            // Act
            var actual = mapper.MapToSerializable(camera);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.TypeOf<CameraDefinition>());
        }

        [Test]
        public void FromDefinition()
        {
            // Arrange
            var mapper = new CameraDefinitionMapper();
            var cameraDefinition = new CameraDefinition();

            // Act
            var actual = mapper.MapFromSerializable(cameraDefinition);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.TypeOf<CameraComponent>());
        }
    }
}