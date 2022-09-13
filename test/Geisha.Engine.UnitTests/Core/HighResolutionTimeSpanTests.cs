using Geisha.Engine.Core;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core
{
    [TestFixture]
    public class HighResolutionTimeSpanTests
    {
        [TestCase(0)]
        [TestCase(123)]
        [TestCase(0.123)]
        [TestCase(0.123456)]
        public void FromSeconds_ShouldReturnTimeSpanThatRepresentsSpecifiedNumberOfSeconds_WithSingleTickPrecision(double seconds)
        {
            // Arrange
            // Act
            var actual = HighResolutionTimeSpan.FromSeconds(seconds);

            // Assert
            Assert.That(actual.TotalSeconds, Is.EqualTo(seconds).Within(0.0000001));
        }
    }
}