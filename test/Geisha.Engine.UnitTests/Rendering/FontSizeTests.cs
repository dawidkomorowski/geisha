using Geisha.Engine.Rendering;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering
{
    [TestFixture]
    public class FontSizeTests
    {
        #region Static methods

        [TestCase(72.0d, 96.0d)]
        [TestCase(123.456d, 164.608d)]
        public void FromPoints_ShouldReturnInstanceWithCorrectPointsAndDips(double expectedPoints, double expectedDips)
        {
            // Arrange
            // Act
            var actual = FontSize.FromPoints(expectedPoints);

            // Assert
            Assert.That(actual.Points, Is.EqualTo(expectedPoints));
            Assert.That(actual.Dips, Is.EqualTo(expectedDips));
        }

        [TestCase(96.0d, 72.0d)]
        [TestCase(164.608d, 123.456d)]
        public void FromDips_ShouldReturnInstanceWithCorrectPointsAndDips(double expectedDips, double expectedPoints)
        {
            // Arrange
            // Act
            var actual = FontSize.FromDips(expectedDips);

            // Assert
            Assert.That(actual.Points, Is.EqualTo(expectedPoints));
            Assert.That(actual.Dips, Is.EqualTo(expectedDips));
        }

        #endregion

        #region Methods

        [TestCase(123.456, 123.456, true)]
        [TestCase(123.456, 123.457, false)]
        public void EqualityMembers_ShouldEqualFontSize_WhenPointsAreEqual(double points1, double points2, bool expectedIsEqual)
        {
            // Arrange
            var fontSize1 = FontSize.FromPoints(points1);
            var fontSize2 = FontSize.FromPoints(points2);

            // Act
            // Assert
            AssertEqualityMembers
                .ForValues(fontSize1, fontSize2)
                .UsingEqualityOperator((x, y) => x == y)
                .UsingInequalityOperator((x, y) => x != y)
                .EqualityIsExpectedToBe(expectedIsEqual);
        }

        [TestCase(72.0d, "Points: 72, Dips: 96")]
        [TestCase(123.456d, "Points: 123.456, Dips: 164.608")]
        [SetCulture("")]
        public void ToString_Test(double points, string expected)
        {
            // Arrange
            var fontSize = FontSize.FromPoints(points);

            // Act
            var actual = fontSize.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        #endregion
    }
}