using Geisha.Engine.Input.Mapping;
using NUnit.Framework;

namespace Geisha.Engine.Input.UnitTests.Mapping
{
    [TestFixture]
    public class ActionMappingGroupTests
    {
        [Test]
        public void ActionMapping_ShouldBeInitializedWithEmptyList()
        {
            // Arrange
            // Act
            var actionMappingGroup = new ActionMappingGroup();

            // Assert
            Assert.That(actionMappingGroup.ActionMappings, Is.Not.Null);
            Assert.That(actionMappingGroup.ActionMappings, Is.Empty);
        }
    }
}