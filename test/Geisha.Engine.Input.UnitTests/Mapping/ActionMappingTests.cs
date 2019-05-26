using Geisha.Engine.Input.Mapping;
using NUnit.Framework;

namespace Geisha.Engine.Input.UnitTests.Mapping
{
    [TestFixture]
    public class ActionMappingTests
    {
        [Test]
        public void HardwareActions_ShouldBeInitializedWithEmptyList()
        {
            // Arrange
            // Act
            var actionMapping = new ActionMapping();

            // Assert
            Assert.That(actionMapping.HardwareActions, Is.Not.Null);
            Assert.That(actionMapping.HardwareActions, Is.Empty);
        }
    }
}