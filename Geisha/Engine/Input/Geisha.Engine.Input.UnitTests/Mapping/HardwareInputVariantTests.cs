using Geisha.Engine.Input.Mapping;
using Geisha.Framework.Input;
using NUnit.Framework;

namespace Geisha.Engine.Input.UnitTests.Mapping
{
    [TestFixture]
    public class HardwareInputVariantTests
    {
        [Test]
        public void Key_ShouldSetCurrentVariantToKeyboard_WhenSet()
        {
            // Arrange
            var hardwareInputVariant = new HardwareInputVariant();
            const Key key = Key.Space;

            // Act
            hardwareInputVariant.Key = key;

            // Assert
            Assert.That(hardwareInputVariant.CurrentVariant, Is.EqualTo(HardwareInputVariant.Variant.Keyboard));
            Assert.That(hardwareInputVariant.Key, Is.EqualTo(key));
        }
    }
}