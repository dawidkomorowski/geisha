using System;
using Geisha.Engine.Core.SceneModel;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel
{
    [TestFixture]
    public class ComponentTests
    {
        [Test]
        public void ComponentId_ShouldReturnComponentIdEqualFullNameOfComponentType_WhenComponentIdAttributeIsNotApplied()
        {
            // Arrange
            var component = new ComponentWithoutCustomId();

            // Act
            // Assert
            Assert.That(component.ComponentId, Is.EqualTo(new ComponentId(typeof(ComponentWithoutCustomId).FullName ?? throw new InvalidOperationException())));
        }

        [Test]
        public void ComponentId_ShouldReturnComponentIdEqualComponentIdAttribute_WhenComponentIdAttributeIsApplied()
        {
            // Arrange
            var component = new ComponentWithCustomId();

            // Act
            // Assert
            Assert.That(component.ComponentId, Is.EqualTo(new ComponentId("Custom Component Id")));
        }

        private sealed class ComponentWithoutCustomId : Component
        {
        }

        [ComponentId("Custom Component Id")]
        private sealed class ComponentWithCustomId : Component
        {
        }
    }
}