using NUnit.Framework;

namespace Geisha.Framework.Rendering.UnitTests
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
        public void Equals_Test(double points1, double points2, bool expected)
        {
            // Arrange
            var fontSize1 = FontSize.FromPoints(points1);
            var fontSize2 = FontSize.FromPoints(points2);

            // Act
            var actual1 = fontSize1.Equals(fontSize2);
            var actual2 = fontSize1.Equals((object) fontSize2);

            // Assert
            Assert.That(actual1, Is.EqualTo(expected));
            Assert.That(actual2, Is.EqualTo(expected));
        }

        [Test]
        public void Equals_ReturnsFalse_GivenNull()
        {
            // Arrange
            var fontSize = FontSize.FromPoints(123.456);

            // Act
            var result = fontSize.Equals(null);

            // Assert
            Assert.That(result, Is.False);
        }

        [TestCase(123.456, 123.456, true)]
        [TestCase(123.456, 123.457, false)]
        public void GetHashCode_Test(double points1, double points2, bool expected)
        {
            // Arrange
            var fontSize1 = FontSize.FromPoints(points1);
            var fontSize2 = FontSize.FromPoints(points2);

            // Act
            var hashCode1 = fontSize1.GetHashCode();
            var hashCode2 = fontSize2.GetHashCode();
            var actual = hashCode1 == hashCode2;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
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

        #region Operators

        [TestCase(123.456, 123.456, true)]
        [TestCase(123.456, 123.457, false)]
        public void EqualityOperator(double points1, double points2, bool expected)
        {
            // Arrange
            var fontSize1 = FontSize.FromPoints(points1);
            var fontSize2 = FontSize.FromPoints(points2);

            // Act
            var actual = fontSize1 == fontSize2;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(123.456, 123.456, false)]
        [TestCase(123.456, 123.457, true)]
        public void InequalityOperator(double points1, double points2, bool expected)
        {
            // Arrange
            var fontSize1 = FontSize.FromPoints(points1);
            var fontSize2 = FontSize.FromPoints(points2);

            // Act
            var actual = fontSize1 != fontSize2;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        #endregion
    }
}