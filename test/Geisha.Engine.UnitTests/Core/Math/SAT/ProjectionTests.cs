using Geisha.Engine.Core.Math.SAT;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math.SAT
{
    [TestFixture]
    public class ProjectionTests
    {
        [TestCase(0, 0, 0, 0, true)]
        [TestCase(5, 10, 10, 15, true)]
        [TestCase(5, 10, 11, 15, false)]
        [TestCase(-10, -5, -15, -10, true)]
        [TestCase(-10, -5, -15, -11, false)]
        [TestCase(5, 10, 9, 15, true)]
        [TestCase(-10, -5, -15, -9, true)]
        [TestCase(5, 10, 7, 8, true)]
        [TestCase(-10, -5, -8, -7, true)]
        [TestCase(-5, 5, 5, 10, true)]
        [TestCase(-5, 5, 6, 10, false)]
        [TestCase(-5, 5, -10, -5, true)]
        [TestCase(-5, 5, -10, -6, false)]
        [TestCase(-5, 5, 3, 10, true)]
        [TestCase(-5, 5, -10, -3, true)]
        public void Overlaps(double min1, double max1, double min2, double max2, bool expected)
        {
            // Arrange
            var p1 = new Projection(min1, max1);
            var p2 = new Projection(min2, max2);

            // Act
            var actual1 = p1.Overlaps(p2);
            var actual2 = p2.Overlaps(p1);

            // Assert
            Assert.That(actual1, Is.EqualTo(expected));
            Assert.That(actual2, Is.EqualTo(expected));
        }
    }
}