using Geisha.Engine.Input.Mapping;
using NUnit.Framework;

namespace Geisha.Engine.Input.UnitTests.Mapping
{
    [TestFixture]
    public class InputMappingTests
    {
        [Test]
        public void ActionMappingGroups_ShouldBeInitializedWithEmptyList()
        {
            // Arrange
            // Act
            var inputMapping = new InputMapping();

            // Assert
            Assert.That(inputMapping.ActionMappingGroups, Is.Not.Null);
            Assert.That(inputMapping.ActionMappingGroups, Is.Empty);
        }

        [Test]
        public void AxisMappingGroups_ShouldBeInitializedWithEmptyList()
        {
            // Arrange
            // Act
            var inputMapping = new InputMapping();

            // Assert
            Assert.That(inputMapping.AxisMappingGroups, Is.Not.Null);
            Assert.That(inputMapping.AxisMappingGroups, Is.Empty);
        }
    }
}