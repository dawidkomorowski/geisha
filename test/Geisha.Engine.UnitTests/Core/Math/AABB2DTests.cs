using Geisha.Engine.Core.Math;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math;

[TestFixture]
// ReSharper disable once InconsistentNaming
public class AABB2DTests
{
    #region Constructors

    [Test]
    public void Constructor_ShouldSetMinAndMax()
    {
        // Arrange
        // Act
        var aabb = new AABB2D(new Vector2(1, 2), new Vector2(5, 8));

        // Assert
        Assert.That(aabb.Min, Is.EqualTo(new Vector2(1, 2)));
        Assert.That(aabb.Max, Is.EqualTo(new Vector2(5, 8)));
    }

    #endregion

    #region Properties

    [TestCase( /*Min*/ 0, 0, /*Max*/ 10, 6, /*Center*/ 5, 3)]
    [TestCase( /*Min*/ -5, -3, /*Max*/ 5, 3, /*Center*/ 0, 0)]
    [TestCase( /*Min*/ 1, 2, /*Max*/ 5, 8, /*Center*/ 3, 5)]
    public void Center_Test(double minX, double minY, double maxX, double maxY, double centerX, double centerY)
    {
        // Arrange
        var aabb = new AABB2D(new Vector2(minX, minY), new Vector2(maxX, maxY));

        // Act
        // Assert
        Assert.That(aabb.Center, Is.EqualTo(new Vector2(centerX, centerY)));
    }

    [TestCase( /*Min*/ 0, 0, /*Max*/ 10, 6, /*Dimensions*/ 10, 6)]
    [TestCase( /*Min*/ -5, -3, /*Max*/ 4, 5, /*Dimensions*/ 9, 8)]
    [TestCase( /*Min*/ 1, 2, /*Max*/ 4, 6, /*Dimensions*/ 3, 4)]
    public void Dimensions_Test(double minX, double minY, double maxX, double maxY, double dimX, double dimY)
    {
        // Arrange
        var aabb = new AABB2D(new Vector2(minX, minY), new Vector2(maxX, maxY));

        // Act
        // Assert
        Assert.That(aabb.Dimensions, Is.EqualTo(new Vector2(dimX, dimY)));
    }

    #endregion

    #region Methods

    [TestCase( /*AABB*/ 0, 0, 10, 6, /*P*/ 5, 3, /*E*/ true)]
    [TestCase( /*AABB*/ 0, 0, 10, 6, /*P*/ 0, 0, /*E*/ true)]
    [TestCase( /*AABB*/ 0, 0, 10, 6, /*P*/ 10, 6, /*E*/ true)]
    [TestCase( /*AABB*/ 0, 0, 10, 6, /*P*/ 0, 3, /*E*/ true)]
    [TestCase( /*AABB*/ 0, 0, 10, 6, /*P*/ 10, 3, /*E*/ true)]
    [TestCase( /*AABB*/ 0, 0, 10, 6, /*P*/ 5, 0, /*E*/ true)]
    [TestCase( /*AABB*/ 0, 0, 10, 6, /*P*/ 5, 6, /*E*/ true)]
    [TestCase( /*AABB*/ 0, 0, 10, 6, /*P*/ -1, 3, /*E*/ false)]
    [TestCase( /*AABB*/ 0, 0, 10, 6, /*P*/ 11, 3, /*E*/ false)]
    [TestCase( /*AABB*/ 0, 0, 10, 6, /*P*/ 5, -1, /*E*/ false)]
    [TestCase( /*AABB*/ 0, 0, 10, 6, /*P*/ 5, 7, /*E*/ false)]
    public void Contains(double minX, double minY, double maxX, double maxY, double pointX, double pointY, bool expected)
    {
        // Arrange
        var aabb = new AABB2D(new Vector2(minX, minY), new Vector2(maxX, maxY));
        var point = new Vector2(pointX, pointY);

        // Act
        var actual = aabb.Contains(point);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 0, 0, 10, 6, /*E*/ true)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 2, 1, 8, 5, /*E*/ true)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 5, 3, 15, 9, /*E*/ true)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ -5, -3, 5, 3, /*E*/ true)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ -5, 3, 5, 9, /*E*/ true)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 5, -3, 15, 3, /*E*/ true)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 10, 0, 16, 6, /*E*/ true)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ -6, 0, 0, 6, /*E*/ true)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 0, 6, 10, 12, /*E*/ true)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 0, -6, 10, 0, /*E*/ true)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 11, 0, 17, 6, /*E*/ false)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ -6, 0, -1, 6, /*E*/ false)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 0, 7, 10, 13, /*E*/ false)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 0, -6, 10, -1, /*E*/ false)]
    public void Overlaps(double minX1, double minY1, double maxX1, double maxY1,
        double minX2, double minY2, double maxX2, double maxY2, bool expected)
    {
        // Arrange
        var aabb1 = new AABB2D(new Vector2(minX1, minY1), new Vector2(maxX1, maxY1));
        var aabb2 = new AABB2D(new Vector2(minX2, minY2), new Vector2(maxX2, maxY2));

        // Act
        var actual = aabb1.Overlaps(aabb2);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ToAxisAlignedRectangle_ShouldReturnAxisAlignedRectangleWithSameCenterAndDimensions()
    {
        // Arrange
        var aabb = new AABB2D(new Vector2(0, 0), new Vector2(10, 6));

        // Act
        var rectangle = aabb.ToAxisAlignedRectangle();

        // Assert
        Assert.That(rectangle.Center, Is.EqualTo(aabb.Center));
        Assert.That(rectangle.Dimensions, Is.EqualTo(aabb.Dimensions));
    }

    #endregion
}