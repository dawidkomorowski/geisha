using Geisha.Engine.Core.SceneModel;
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
            Assert.That(componentId1.Equals(componentId2), Is.EqualTo(expectedIsEqual));
            Assert.That(componentId2.Equals(componentId1), Is.EqualTo(expectedIsEqual));
            Assert.That(componentId1.Equals((object) componentId2), Is.EqualTo(expectedIsEqual));
            Assert.That(componentId2.Equals((object) componentId1), Is.EqualTo(expectedIsEqual));
            Assert.That(componentId1 == componentId2, Is.EqualTo(expectedIsEqual));
            Assert.That(componentId2 == componentId1, Is.EqualTo(expectedIsEqual));
            Assert.That(componentId1.GetHashCode().Equals(componentId2.GetHashCode()), Is.EqualTo(expectedIsEqual));

            Assert.That(componentId1.Equals(null), Is.False);
            Assert.That(componentId2.Equals(null), Is.False);
        }
    }
}