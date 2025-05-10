using System.Collections.Generic;
using Geisha.Engine.Core.Math;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math;

[TestFixture]
public class LineSegmentTests
{
    private static readonly IEqualityComparer<Vector2> Vector2Comparer = CommonEqualityComparer.Vector2(0.000001);

    [Test]
    public void Constructor_ShouldCreateLineSegment_GivenSpecifiedStartAndEndPoints()
    {
        // Arrange
        var startPoint = new Vector2(1, 2);
        var endPoint = new Vector2(3, 4);

        // Act
        var lineSegment = new LineSegment(startPoint, endPoint);

        // Assert
        Assert.That(lineSegment.StartPoint, Is.EqualTo(startPoint));
        Assert.That(lineSegment.EndPoint, Is.EqualTo(endPoint));
    }

    [TestCase(0, 0, 5, 0, 0, 1)]
    [TestCase(0, 0, 0, 5, -1, 0)]
    [TestCase(1, 2, 3, 4, -0.707106, 0.707106)]
    [TestCase(3, 4, 1, 2, 0.707106, -0.707106)]
    public void Normal_ShouldReturnNormalVectorOfLineSegment(double sx, double sy, double ex, double ey, double nx, double ny)
    {
        // Arrange
        var lineSegment = new LineSegment(new Vector2(sx, sy), new Vector2(ex, ey));
        var expectedNormal = new Vector2(nx, ny);

        // Act
        var actual = lineSegment.Normal;

        // Assert
        Assert.That(actual, Is.EqualTo(expectedNormal).Using(Vector2Comparer));
    }

    [TestCase(new double[] { 1, 1, 5, 5 }, new double[] { 3, 1, 5, 3 })]
    [TestCase(new double[] { 1, -1, 1, 1 }, new double[] { -1, -1, -1, 1 })]
    [TestCase(new double[] { -1, -1, 1, -1 }, new double[] { -1, 1, 1, 1 })]
    public void Intersects_ShouldReturnNoIntersection_GivenTwoLineSegmentsAreParallelAndDisjoint(double[] line1, double[] line2)
    {
        // Arrange
        Assert.That(line1, Has.Length.EqualTo(4));
        Assert.That(line2, Has.Length.EqualTo(4));

        var lineSegment1 = new LineSegment(new Vector2(line1[0], line1[1]), new Vector2(line1[2], line1[3]));
        var lineSegment2 = new LineSegment(new Vector2(line2[0], line2[1]), new Vector2(line2[2], line2[3]));

        // Act
        var intersectionResult = lineSegment1.Intersects(lineSegment2, out var intersectionPoint);

        // Assert
        Assert.That(intersectionResult, Is.EqualTo(LineSegment.IntersectionResult.NoIntersection));
        Assert.That(intersectionPoint, Is.EqualTo(Vector2.Zero));
    }

    [Test]
    public void Intersects_ShouldReturnNoIntersection_GivenTwoLineSegmentsAreParallelAndOverlapping()
    {
        // Arrange
        var lineSegment1 = new LineSegment(new Vector2(1, 1), new Vector2(5, 5));
        var lineSegment2 = new LineSegment(new Vector2(2, 2), new Vector2(4, 4));

        // Act
        var intersectionResult = lineSegment1.Intersects(lineSegment2, out var intersectionPoint);

        // Assert
        Assert.That(intersectionResult, Is.EqualTo(LineSegment.IntersectionResult.NoIntersection));
        Assert.That(intersectionPoint, Is.EqualTo(Vector2.Zero));
    }

    [TestCase(new double[] { 1, 1, 5, 5 }, new double[] { 3, 1, 1, 3 }, new double[] { 2, 2 })]
    [TestCase(new[] { -2, -0.5, 1, 1 }, new[] { -2, 1.5, 1, -1 }, new[] { -0.5, 0.25 })]
    [TestCase(new[] { -1, 1.4, -0.8, 0.2 }, new[] { -1.6, 0.8, -0.6, 0.4 }, new[] { -0.85, 0.5 })]
    public void Intersects_ShouldReturnLineSegmentIntersection_GivenTwoLineSegmentsThatCrossEachOther(double[] line1, double[] line2, double[] intersection)
    {
        // Arrange
        Assert.That(line1, Has.Length.EqualTo(4));
        Assert.That(line2, Has.Length.EqualTo(4));
        Assert.That(intersection, Has.Length.EqualTo(2));

        var lineSegment1 = new LineSegment(new Vector2(line1[0], line1[1]), new Vector2(line1[2], line1[3]));
        var lineSegment2 = new LineSegment(new Vector2(line2[0], line2[1]), new Vector2(line2[2], line2[3]));
        var expected = new Vector2(intersection[0], intersection[1]);

        // Act
        var intersectionResult = lineSegment1.Intersects(lineSegment2, out var intersectionPoint);

        // Assert
        Assert.That(intersectionResult, Is.EqualTo(LineSegment.IntersectionResult.LineSegmentIntersection));
        Assert.That(intersectionPoint, Is.EqualTo(expected).Using(Vector2Comparer));
    }

    [TestCase(new double[] { 1, 1, 5, 5 }, new double[] { 3, 1, 4, 0 }, new double[] { 2, 2 })]
    [TestCase(new[] { 0.6, -0.4, 0.2, -0.2 }, new[] { -0.2, -0.4, 0.2, 0.2 }, new[] { 0, -0.1 })]
    [TestCase(new[] { 0.6, -0.6, -0.4, -0.4 }, new[] { 0.2, 0.4, 1.4, 0.8 }, new[] { -1.525, -0.175 })]
    public void Intersects_ShouldReturnLineIntersection_GivenTwoLineSegmentsThatWhenExtendedWouldCrossEachOther(
        double[] line1, double[] line2, double[] intersection)
    {
        // Arrange
        Assert.That(line1, Has.Length.EqualTo(4));
        Assert.That(line2, Has.Length.EqualTo(4));
        Assert.That(intersection, Has.Length.EqualTo(2));

        var lineSegment1 = new LineSegment(new Vector2(line1[0], line1[1]), new Vector2(line1[2], line1[3]));
        var lineSegment2 = new LineSegment(new Vector2(line2[0], line2[1]), new Vector2(line2[2], line2[3]));
        var expected = new Vector2(intersection[0], intersection[1]);

        // Act
        var intersectionResult = lineSegment1.Intersects(lineSegment2, out var intersectionPoint);

        // Assert
        Assert.That(intersectionResult, Is.EqualTo(LineSegment.IntersectionResult.LineIntersection));
        Assert.That(intersectionPoint, Is.EqualTo(expected).Using(Vector2Comparer));
    }

    [Test]
    [SetCulture("")]
    public void ToString_Test()
    {
        // Arrange
        var lineSegment = new LineSegment(new Vector2(1.2, 3.4), new Vector2(5.6, 7.8));

        // Act
        var actual = lineSegment.ToString();

        // Assert
        Assert.That(actual, Is.EqualTo("StartPoint: X: 1.2, Y: 3.4, EndPoint: X: 5.6, Y: 7.8"));
    }

    [TestCase(new double[] { 1, 2, 3, 4 }, new double[] { 1, 2, 3, 4 }, true)]
    [TestCase(new double[] { 1, 2, 3, 4 }, new double[] { 0, 2, 3, 4 }, false)]
    [TestCase(new double[] { 1, 2, 3, 4 }, new double[] { 1, 0, 3, 4 }, false)]
    [TestCase(new double[] { 1, 2, 3, 4 }, new double[] { 1, 2, 0, 4 }, false)]
    [TestCase(new double[] { 1, 2, 3, 4 }, new double[] { 1, 2, 3, 0 }, false)]
    public void EqualityMembers_ShouldEqualLineSegment_WhenStartPointAndEndPointAreEqual(double[] line1, double[] line2, bool expectedIsEqual)
    {
        // Arrange
        Assert.That(line1, Has.Length.EqualTo(4));
        Assert.That(line2, Has.Length.EqualTo(4));

        var lineSegment1 = new LineSegment(new Vector2(line1[0], line1[1]), new Vector2(line1[2], line1[3]));
        var lineSegment2 = new LineSegment(new Vector2(line2[0], line2[1]), new Vector2(line2[2], line2[3]));

        // Act
        // Assert
        AssertEqualityMembers
            .ForValues(lineSegment1, lineSegment2)
            .UsingEqualityOperator((x, y) => x == y)
            .UsingInequalityOperator((x, y) => x != y)
            .EqualityIsExpectedToBe(expectedIsEqual);
    }
}