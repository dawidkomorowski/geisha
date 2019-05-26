using Geisha.Engine.Input.Mapping;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input.Mapping
{
    [TestFixture]
    public class InputMappingTests
    {
        [Test]
        public void ActionMappings_ShouldBeInitializedWithEmptyList()
        {
            // Arrange
            // Act
            var inputMapping = new InputMapping();

            // Assert
            Assert.That(inputMapping.ActionMappings, Is.Not.Null);
            Assert.That(inputMapping.ActionMappings, Is.Empty);
        }

        [Test]
        public void AxisMappings_ShouldBeInitializedWithEmptyList()
        {
            // Arrange
            // Act
            var inputMapping = new InputMapping();

            // Assert
            Assert.That(inputMapping.AxisMappings, Is.Not.Null);
            Assert.That(inputMapping.AxisMappings, Is.Empty);
        }
    }
}