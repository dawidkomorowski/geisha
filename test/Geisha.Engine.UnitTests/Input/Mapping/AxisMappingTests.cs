using Geisha.Engine.Input.Mapping;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input.Mapping
{
    [TestFixture]
    public class AxisMappingTests
    {
        [Test]
        public void HardwareAxes_ShouldBeInitializedWithEmptyList()
        {
            // Arrange
            // Act
            var axisMapping = new AxisMapping();

            // Assert
            Assert.That(axisMapping.HardwareAxes, Is.Not.Null);
            Assert.That(axisMapping.HardwareAxes, Is.Empty);
        }
    }
}