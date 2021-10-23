using System;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel
{
    [TestFixture]
    public class ComponentIdTests
    {
        [Test]
        public void Constructor_CreatesComponentIdWithEmptyString_GivenNoParameters()
        {
            // Arrange
            // Act
            var componentId = new ComponentId();
            var actual = componentId.Value;

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void Constructor_ThrowsException_GivenNull()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => new ComponentId(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Constructor_CreatesComponentIdWithValue_GivenNonNullString()
        {
            // Arrange
            const string componentIdString = "Some component id";

            // Act
            var componentId = new ComponentId(componentIdString);
            var actual = componentId.Value;

            // Assert
            Assert.That(actual, Is.EqualTo(componentIdString));
        }

        [Test]
        public void ToString_ShouldReturnValue()
        {
            // Arrange
            const string componentIdString = "Some component id";
            var componentId = new ComponentId(componentIdString);

            // Act
            var actual = componentId.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo(componentIdString));
        }

        [TestCase("Component 1", "Component 1", true)]
        [TestCase("Component 1", "Component 2", false)]
        public void EqualityMembers_ShouldEqualComponentId_WhenStringValueIsEqual(string id1, string id2, bool expectedIsEqual)
        {
            // Arrange
            var componentId1 = new ComponentId(id1);
            var componentId2 = new ComponentId(id2);

            // Act
            // Assert
            AssertEqualityMembers
                .ForValues(componentId1, componentId2)
                .UsingEqualityOperator((x, y) => x == y)
                .UsingInequalityOperator((x, y) => x != y)
                .EqualityIsExpectedToBe(expectedIsEqual);
        }

        [Test]
        public void Of_ShouldReturnComponentIdEqualComponentTypeFullName_GivenComponentTypeWithoutComponentIdAttribute()
        {
            // Arrange
            // Act
            var actual = ComponentId.Of(typeof(ComponentWithoutCustomId));

            // Assert
            Assert.That(actual, Is.EqualTo(new ComponentId(typeof(ComponentWithoutCustomId).FullName ?? throw new InvalidOperationException())));
        }

        [Test]
        public void Of_ShouldReturnComponentIdEqualComponentIdAttribute_GivenComponentTypeWithComponentIdAttribute()
        {
            // Arrange
            // Act
            var actual = ComponentId.Of(typeof(ComponentWithCustomId));

            // Assert
            Assert.That(actual, Is.EqualTo(new ComponentId("Custom Component Id")));
        }

        [Test]
        public void Of_ShouldReturnTheSameComponentIdTwice_TryToTestCache()
        {
            // Arrange
            // Act
            var actual1 = ComponentId.Of(typeof(ComponentWithCustomId));
            var actual2 = ComponentId.Of(typeof(ComponentWithCustomId));

            // Assert
            Assert.That(actual1, Is.EqualTo(actual2));
        }

        [Test]
        public void Of_ShouldReturnComponentIdEqualComponentTypeFullName_GivenComponentTypeGenericParameterWithoutComponentIdAttribute()
        {
            // Arrange
            // Act
            var actual = ComponentId.Of<ComponentWithoutCustomId>();

            // Assert
            Assert.That(actual, Is.EqualTo(new ComponentId(typeof(ComponentWithoutCustomId).FullName ?? throw new InvalidOperationException())));
        }

        [Test]
        public void Of_ShouldReturnComponentIdEqualComponentIdAttribute_GivenComponentTypeGenericParameterWithComponentIdAttribute()
        {
            // Arrange
            // Act
            var actual = ComponentId.Of<ComponentWithCustomId>();

            // Assert
            Assert.That(actual, Is.EqualTo(new ComponentId("Custom Component Id")));
        }

        #region Helpers

        private sealed class ComponentWithoutCustomId : Component
        {
        }

        [ComponentId("Custom Component Id")]
        private sealed class ComponentWithCustomId : Component
        {
        }

        #endregion
    }
}