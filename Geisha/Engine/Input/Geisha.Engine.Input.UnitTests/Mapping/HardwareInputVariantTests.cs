using Geisha.Engine.Input.Mapping;
using Geisha.Framework.Input;
using NUnit.Framework;

namespace Geisha.Engine.Input.UnitTests.Mapping
{
    [TestFixture]
    public class HardwareInputVariantTests
    {
        #region Constructor

        [Test]
        public void ConstructorFromKey_ShouldSetCurrentVariantToKeyboard()
        {
            // Arrange
            const Key key = Key.Space;

            // Act
            var hardwareInputVariant = new HardwareInputVariant(key);

            // Assert
            Assert.That(hardwareInputVariant.Key, Is.EqualTo(key));
            Assert.That(hardwareInputVariant.CurrentVariant, Is.EqualTo(HardwareInputVariant.Variant.Keyboard));
        }

        #endregion

        #region Formatting members

        [TestCase(Key.Space, "CurrentVariant: Keyboard, Key: Space")]
        public void ToString(Key key, string expected)
        {
            // Arrange
            var hardwareInputVariant = new HardwareInputVariant(key);

            // Act
            var actual = hardwareInputVariant.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        #endregion

        #region Equality members

        [TestCase(Key.Space, Key.Space, true)]
        [TestCase(Key.Space, Key.Enter, false)]
        public void EqualityMembers(Key key1, Key key2, bool expected)
        {
            // Arrange
            var variant1 = new HardwareInputVariant(key1);
            var variant2 = new HardwareInputVariant(key2);

            // Act
            var actual1 = variant1.Equals(variant2);
            var actual2 = variant2.Equals(variant1);

            var actual3 = ((object) variant1).Equals(variant2);
            var actual4 = ((object) variant2).Equals(variant1);

            var actual5 = variant1 == variant2;
            var actual6 = variant2 == variant1;

            var actual7 = !(variant1 != variant2);
            var actual8 = !(variant2 != variant1);

            var actual9 = variant1.GetHashCode() == variant2.GetHashCode();

            // Assert
            Assert.That(actual1, Is.EqualTo(expected));
            Assert.That(actual2, Is.EqualTo(expected));
            Assert.That(actual3, Is.EqualTo(expected));
            Assert.That(actual4, Is.EqualTo(expected));
            Assert.That(actual5, Is.EqualTo(expected));
            Assert.That(actual6, Is.EqualTo(expected));
            Assert.That(actual7, Is.EqualTo(expected));
            Assert.That(actual8, Is.EqualTo(expected));
            Assert.That(actual9, Is.EqualTo(expected));
        }

        #endregion
    }
}