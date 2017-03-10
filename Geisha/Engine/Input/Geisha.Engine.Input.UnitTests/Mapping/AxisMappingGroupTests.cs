using Geisha.Engine.Input.Mapping;
using NUnit.Framework;

namespace Geisha.Engine.Input.UnitTests.Mapping
{
    [TestFixture]
    public class AxisMappingGroupTests
    {
        [Test]
        public void AxisMappings_ShouldBeInitializedWithEmptyList()
        {
            // Arrange
            // Act
            var axisMappingGroup = new AxisMappingGroup();

            // Assert
            Assert.That(axisMappingGroup.AxisMappings, Is.Not.Null);
            Assert.That(axisMappingGroup.AxisMappings, Is.Empty);
        }
    }
}