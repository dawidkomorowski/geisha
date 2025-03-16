using Geisha.Engine.Core.Math;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math;

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
    public void Overlaps_ShouldReturnTrue_WhenTwoProjectionsOverlap(double min1, double max1, double min2, double max2, bool expected)
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

    [TestCase(0, 0, 0, 0, 0)]
    [TestCase(5, 10, 10, 15, 0)]
    [TestCase(5, 10, 11, 15, 1)]
    [TestCase(-10, -5, -15, -10, 0)]
    [TestCase(-10, -5, -15, -11, 1)]
    [TestCase(5, 10, 9, 15, -1)]
    [TestCase(-10, -5, -15, -9, -1)]
    [TestCase(5, 10, 7, 8, -3)]
    [TestCase(-10, -5, -8, -7, -3)]
    [TestCase(-5, 5, 5, 10, 0)]
    [TestCase(-5, 5, 6, 10, 1)]
    [TestCase(-5, 5, -10, -5, 0)]
    [TestCase(-5, 5, -10, -6, 1)]
    [TestCase(-5, 5, 7, 10, 2)]
    [TestCase(-5, 5, -10, -7, 2)]
    [TestCase(-5, 5, 3, 10, -2)]
    [TestCase(-5, 5, -10, -3, -2)]
    public void Distance_ShouldReturnDistanceBetweenTwoProjections(double min1, double max1, double min2, double max2, double expected)
    {
        // Arrange
        var p1 = new Projection(min1, max1);
        var p2 = new Projection(min2, max2);

        // Act
        var actual1 = p1.Distance(p2);
        var actual2 = p2.Distance(p1);

        // Assert
        Assert.That(actual1, Is.EqualTo(expected));
        Assert.That(actual2, Is.EqualTo(expected));
    }
}