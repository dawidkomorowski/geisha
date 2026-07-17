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

    [Test]
    public void Constructor_FromMinXMinYMaxXMaxY_ShouldSetMinAndMax()
    {
        // Arrange
        // Act
        var aabb = new AABB2D(1, 2, 5, 8);

        // Assert
        Assert.That(aabb.Min, Is.EqualTo(new Vector2(1, 2)));
        Assert.That(aabb.Max, Is.EqualTo(new Vector2(5, 8)));
    }

    #endregion

    #region Factory methods

    [Test]
    public void FromSize_FromVector2_ShouldCreateAABBCentredAtOriginWithGivenSize()
    {
        // Arrange
        // Act
        var aabb = AABB2D.FromSize(new Vector2(10, 6));

        // Assert
        Assert.That(aabb.Min, Is.EqualTo(new Vector2(-5, -3)));
        Assert.That(aabb.Max, Is.EqualTo(new Vector2(5, 3)));
    }

    [Test]
    public void FromSize_FromSizeD_ShouldCreateAABBCentredAtOriginWithGivenSize()
    {
        // Arrange
        // Act
        var aabb = AABB2D.FromSize(new SizeD(10, 6));

        // Assert
        Assert.That(aabb.Min, Is.EqualTo(new Vector2(-5, -3)));
        Assert.That(aabb.Max, Is.EqualTo(new Vector2(5, 3)));
    }

    [Test]
    public void FromSize_FromSize_ShouldCreateAABBCentredAtOriginWithGivenSize()
    {
        // Arrange
        // Act
        var aabb = AABB2D.FromSize(new Size(10, 6));

        // Assert
        Assert.That(aabb.Min, Is.EqualTo(new Vector2(-5, -3)));
        Assert.That(aabb.Max, Is.EqualTo(new Vector2(5, 3)));
    }

    [Test]
    public void FromSize_FromWidthAndHeight_ShouldCreateAABBCentredAtOriginWithGivenSize()
    {
        // Arrange
        // Act
        var aabb = AABB2D.FromSize(10, 6);

        // Assert
        Assert.That(aabb.Min, Is.EqualTo(new Vector2(-5, -3)));
        Assert.That(aabb.Max, Is.EqualTo(new Vector2(5, 3)));
    }

    [Test]
    public void FromCenterAndSize_FromVector2AndVector2_ShouldCreateAABBWithGivenCenterAndSize()
    {
        // Arrange
        // Act
        var aabb = AABB2D.FromCenterAndSize(new Vector2(2, 3), new Vector2(10, 6));

        // Assert
        Assert.That(aabb.Min, Is.EqualTo(new Vector2(-3, 0)));
        Assert.That(aabb.Max, Is.EqualTo(new Vector2(7, 6)));
    }

    [Test]
    public void FromCenterAndSize_FromVector2AndSizeD_ShouldCreateAABBWithGivenCenterAndSize()
    {
        // Arrange
        // Act
        var aabb = AABB2D.FromCenterAndSize(new Vector2(2, 3), new SizeD(10, 6));

        // Assert
        Assert.That(aabb.Min, Is.EqualTo(new Vector2(-3, 0)));
        Assert.That(aabb.Max, Is.EqualTo(new Vector2(7, 6)));
    }

    [Test]
    public void FromCenterAndSize_FromVector2AndSize_ShouldCreateAABBWithGivenCenterAndSize()
    {
        // Arrange
        // Act
        var aabb = AABB2D.FromCenterAndSize(new Vector2(2, 3), new Size(10, 6));

        // Assert
        Assert.That(aabb.Min, Is.EqualTo(new Vector2(-3, 0)));
        Assert.That(aabb.Max, Is.EqualTo(new Vector2(7, 6)));
    }

    [Test]
    public void FromCenterAndSize_FromCenterXCenterYWidthHeight_ShouldCreateAABBWithGivenCenterAndSize()
    {
        // Arrange
        // Act
        var aabb = AABB2D.FromCenterAndSize(2, 3, 10, 6);

        // Assert
        Assert.That(aabb.Min, Is.EqualTo(new Vector2(-3, 0)));
        Assert.That(aabb.Max, Is.EqualTo(new Vector2(7, 6)));
    }

    [TestCase(new double[] { }, /*Min*/ 0, 0, /*Max*/ 0, 0)]
    [TestCase(new[] { 3.0, 4.0 }, /*Min*/ 3, 4, /*Max*/ 3, 4)]
    [TestCase(new[] { 2.0, -3.0, /**/ 8.0, 5.0, /**/ 1.0, 4.0 }, /*Min*/ 1, -3, /*Max*/ 8, 5)]
    public void FromPoints_Test(double[] points, double minX, double minY, double maxX, double maxY)
    {
        // Arrange
        var pointsAsVectors = new Vector2[points.Length / 2];
        for (var i = 0; i < points.Length; i += 2)
        {
            pointsAsVectors[i / 2] = new Vector2(points[i], points[i + 1]);
        }

        // Act
        var aabb = AABB2D.FromPoints(pointsAsVectors);

        // Assert
        Assert.That(aabb.Min, Is.EqualTo(new Vector2(minX, minY)));
        Assert.That(aabb.Max, Is.EqualTo(new Vector2(maxX, maxY)));
    }

    [TestCase(new double[] { }, /*Min*/ 0, 0, /*Max*/ 0, 0)]
    [TestCase(new[]
    {
        /*A1*/ 0.0, 0.0, 10.0, 6.0
    }, /*Min*/ 0, 0, /*Max*/ 10, 6)]
    [TestCase(new[]
    {
        /*A1*/ 0.0, 0.0, 10.0, 6.0, /*A2*/ -2.0, -1.0, 5.0, 4.0
    }, /*Min*/ -2, -1, /*Max*/ 10, 6)]
    [TestCase(new[]
    {
        /*A1*/ 0.0, 0.0, 10.0, 6.0, /*A2*/ 3.0, 2.0, 15.0, 9.0
    }, /*Min*/ 0, 0, /*Max*/ 15, 9)]
    public void FromAABBs_Test(double[] aabbs, double minX, double minY, double maxX, double maxY)
    {
        // Arrange
        var aabbsAsStructures = new AABB2D[aabbs.Length / 4];
        for (var i = 0; i < aabbs.Length; i += 4)
        {
            aabbsAsStructures[i / 4] = new AABB2D(aabbs[i], aabbs[i + 1], aabbs[i + 2], aabbs[i + 3]);
        }

        // Act
        var aabb = AABB2D.FromAABBs(aabbsAsStructures);

        // Assert
        Assert.That(aabb.Min, Is.EqualTo(new Vector2(minX, minY)));
        Assert.That(aabb.Max, Is.EqualTo(new Vector2(maxX, maxY)));
    }

    #endregion

    #region Properties

    [TestCase( /*Min*/ 0, 0, /*Max*/ 10, 6, /*Center*/ 5, 3)]
    [TestCase( /*Min*/ -5, -3, /*Max*/ 5, 3, /*Center*/ 0, 0)]
    [TestCase( /*Min*/ 1, 2, /*Max*/ 5, 8, /*Center*/ 3, 5)]
    public void Center_Test(double minX, double minY, double maxX, double maxY, double centerX, double centerY)
    {
        // Arrange
        var aabb = new AABB2D(minX, minY, maxX, maxY);

        // Act
        // Assert
        Assert.That(aabb.Center, Is.EqualTo(new Vector2(centerX, centerY)));
    }

    [TestCase( /*Min*/ 0, 0, /*Max*/ 10, 6, /*Size*/ 10, 6)]
    [TestCase( /*Min*/ -5, -3, /*Max*/ 4, 5, /*Size*/ 9, 8)]
    [TestCase( /*Min*/ 1, 2, /*Max*/ 4, 6, /*Size*/ 3, 4)]
    public void Size_Test(double minX, double minY, double maxX, double maxY, double sizeX, double sizeY)
    {
        // Arrange
        var aabb = new AABB2D(minX, minY, maxX, maxY);

        // Act
        // Assert
        Assert.That(aabb.Size, Is.EqualTo(new Vector2(sizeX, sizeY)));
    }

    [TestCase( /*Min*/ 0, 0, /*Max*/ 10, 6, /*Width*/ 10)]
    [TestCase( /*Min*/ -5, -3, /*Max*/ 4, 5, /*Width*/ 9)]
    [TestCase( /*Min*/ 1, 2, /*Max*/ 4, 6, /*Width*/ 3)]
    public void Width_Test(double minX, double minY, double maxX, double maxY, double width)
    {
        // Arrange
        var aabb = new AABB2D(minX, minY, maxX, maxY);

        // Act
        // Assert
        Assert.That(aabb.Width, Is.EqualTo(width));
    }

    [TestCase( /*Min*/ 0, 0, /*Max*/ 10, 6, /*Height*/ 6)]
    [TestCase( /*Min*/ -5, -3, /*Max*/ 4, 5, /*Height*/ 8)]
    [TestCase( /*Min*/ 1, 2, /*Max*/ 4, 6, /*Height*/ 4)]
    public void Height_Test(double minX, double minY, double maxX, double maxY, double height)
    {
        // Arrange
        var aabb = new AABB2D(minX, minY, maxX, maxY);

        // Act
        // Assert
        Assert.That(aabb.Height, Is.EqualTo(height));
    }

    [TestCase( /*Min*/ 0, 0, /*Max*/ 10, 6, /*E*/ true)]
    [TestCase( /*Min*/ -5, -3, /*Max*/ 5, 3, /*E*/ true)]
    [TestCase( /*Min*/ 5, 5, /*Max*/ 5, 5, /*E*/ true)]  // degenerate (point)
    [TestCase( /*Min*/ 0, 0, /*Max*/ 10, 0, /*E*/ true)]  // degenerate (horizontal line)
    [TestCase( /*Min*/ 0, 0, /*Max*/ 0, 6, /*E*/ true)]  // degenerate (vertical line)
    [TestCase( /*Min*/ 0, 0, /*Max*/ -1, 6, /*E*/ false)] // inverted X
    [TestCase( /*Min*/ 0, 0, /*Max*/ 10, -1, /*E*/ false)] // inverted Y
    [TestCase( /*Min*/ 0, 0, /*Max*/ -1, -1, /*E*/ false)] // both inverted
    public void IsValid_Test(double minX, double minY, double maxX, double maxY, bool expected)
    {
        // Arrange
        var aabb = new AABB2D(minX, minY, maxX, maxY);

        // Act
        // Assert
        Assert.That(aabb.IsValid, Is.EqualTo(expected));
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
    public void Contains_Point_Test(double minX, double minY, double maxX, double maxY, double pointX, double pointY, bool expected)
    {
        // Arrange
        var aabb = new AABB2D(minX, minY, maxX, maxY);
        var point = new Vector2(pointX, pointY);

        // Act
        var actual = aabb.Contains(point);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 0, 0, 10, 6, /*E*/ true)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 2, 1, 8, 5, /*E*/ true)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 0, 1, 8, 5, /*E*/ true)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 2, 1, 10, 5, /*E*/ true)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 2, 0, 8, 5, /*E*/ true)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 2, 1, 8, 6, /*E*/ true)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ -1, 1, 8, 5, /*E*/ false)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 2, 1, 11, 5, /*E*/ false)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 2, -1, 8, 5, /*E*/ false)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 2, 1, 8, 7, /*E*/ false)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 11, 0, 17, 6, /*E*/ false)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ -7, 0, -1, 6, /*E*/ false)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 0, 7, 10, 13, /*E*/ false)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 0, -7, 10, -1, /*E*/ false)]
    public void Contains_AABB2D_Test(double minX1, double minY1, double maxX1, double maxY1,
        double minX2, double minY2, double maxX2, double maxY2, bool expected)
    {
        // Arrange
        var aabb1 = new AABB2D(minX1, minY1, maxX1, maxY1);
        var aabb2 = new AABB2D(minX2, minY2, maxX2, maxY2);

        // Act
        var actual = aabb1.Contains(aabb2);

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
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ -7, 0, -1, 6, /*E*/ false)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 0, 7, 10, 13, /*E*/ false)]
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 0, -7, 10, -1, /*E*/ false)]
    public void Overlaps_Test(double minX1, double minY1, double maxX1, double maxY1,
        double minX2, double minY2, double maxX2, double maxY2, bool expected)
    {
        // Arrange
        var aabb1 = new AABB2D(minX1, minY1, maxX1, maxY1);
        var aabb2 = new AABB2D(minX2, minY2, maxX2, maxY2);

        // Act
        var actual = aabb1.Overlaps(aabb2);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 0, 0, 10, 6, /*E*/ 0, 0, 10, 6)]   // identical boxes
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 5, 3, 15, 9, /*E*/ 5, 3, 10, 6)]  // partial overlap top-right
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ -5, -3, 5, 3, /*E*/ 0, 0, 5, 3)]  // partial overlap bottom-left
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 2, 1, 8, 5, /*E*/ 2, 1, 8, 5)]    // A1 fully contains A2
    [TestCase( /*A1*/ 2, 1, 8, 5, /*A2*/ 0, 0, 10, 6, /*E*/ 2, 1, 8, 5)]    // A2 fully contains A1
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 10, 0, 16, 6, /*E*/ 10, 0, 10, 6)] // touching right edge (degenerate vertical line)
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 0, 6, 10, 12, /*E*/ 0, 6, 10, 6)] // touching top edge (degenerate horizontal line)
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 10, 6, 16, 12, /*E*/ 10, 6, 10, 6)] // touching corner (degenerate point)
    public void Intersect_ShouldReturnIntersectionAABB(double minX1, double minY1, double maxX1, double maxY1,
        double minX2, double minY2, double maxX2, double maxY2,
        double eMinX, double eMinY, double eMaxX, double eMaxY)
    {
        // Arrange
        var aabb1 = new AABB2D(minX1, minY1, maxX1, maxY1);
        var aabb2 = new AABB2D(minX2, minY2, maxX2, maxY2);
        var expected = new AABB2D(eMinX, eMinY, eMaxX, eMaxY);

        // Act
        var actual = aabb1.Intersect(aabb2);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 11, 0, 17, 6)]  // separated right
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ -7, 0, -1, 6)]  // separated left
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 0, 7, 10, 13)]  // separated above
    [TestCase( /*A1*/ 0, 0, 10, 6, /*A2*/ 0, -7, 10, -1)] // separated below
    public void Intersect_ShouldReturnInvalidAABB_WhenBoxesDoNotOverlap(double minX1, double minY1, double maxX1, double maxY1,
        double minX2, double minY2, double maxX2, double maxY2)
    {
        // Arrange
        var aabb1 = new AABB2D(minX1, minY1, maxX1, maxY1);
        var aabb2 = new AABB2D(minX2, minY2, maxX2, maxY2);

        // Act
        var actual = aabb1.Intersect(aabb2);

        // Assert
        Assert.That(actual.IsValid, Is.False);
    }

    [Test]
    public void ToAxisAlignedRectangle_ShouldReturnAxisAlignedRectangleWithSameCenterAndSize()
    {
        // Arrange
        var aabb = new AABB2D(0, 0, 10, 6);

        // Act
        var rectangle = aabb.ToAxisAlignedRectangle();

        // Assert
        Assert.That(rectangle.Center, Is.EqualTo(aabb.Center));
        Assert.That(rectangle.Dimensions, Is.EqualTo(aabb.Size));
    }

    [Test]
    public void ToRectangle_ShouldReturnRectangleWithSameCenterAndSize()
    {
        // Arrange
        var aabb = new AABB2D(0, 0, 10, 6);

        // Act
        var rectangle = aabb.ToRectangle();

        // Assert
        Assert.That(rectangle.Center, Is.EqualTo(aabb.Center));
        Assert.That(rectangle.Width, Is.EqualTo(aabb.Width));
        Assert.That(rectangle.Height, Is.EqualTo(aabb.Height));
    }

    #endregion
}