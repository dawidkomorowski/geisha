using Geisha.Engine.Core.Math;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math;

// TODO Add more test cases.
[TestFixture]
public class LineSegmentTests
{
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

    [Test]
    public void Intersects_ShouldReturnNoIntersection_GivenTwoLineSegmentsAreParallelAndDisjoint()
    {
        // Arrange
        var lineSegment1 = new LineSegment(new Vector2(1, 1), new Vector2(5, 5));
        var lineSegment2 = new LineSegment(new Vector2(3, 1), new Vector2(5, 3));

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

    [Test]
    public void Intersects_ShouldReturnLineSegmentIntersection_GivenTwoLineSegmentsThatCrossEachOther()
    {
        // Arrange
        var lineSegment1 = new LineSegment(new Vector2(1, 1), new Vector2(5, 5));
        var lineSegment2 = new LineSegment(new Vector2(3, 1), new Vector2(1, 3));

        // Act
        var intersectionResult = lineSegment1.Intersects(lineSegment2, out var intersectionPoint);

        // Assert
        Assert.That(intersectionResult, Is.EqualTo(LineSegment.IntersectionResult.LineSegmentIntersection));
        Assert.That(intersectionPoint, Is.EqualTo(new Vector2(2, 2)));
    }

    [Test]
    public void Intersects_ShouldReturnLineIntersection_GivenTwoLineSegmentsThatWhenExtendedWouldCrossEachOther()
    {
        // Arrange
        var lineSegment1 = new LineSegment(new Vector2(1, 1), new Vector2(5, 5));
        var lineSegment2 = new LineSegment(new Vector2(3, 1), new Vector2(4, 0));

        // Act
        var intersectionResult = lineSegment1.Intersects(lineSegment2, out var intersectionPoint);

        // Assert
        Assert.That(intersectionResult, Is.EqualTo(LineSegment.IntersectionResult.LineIntersection));
        Assert.That(intersectionPoint, Is.EqualTo(new Vector2(2, 2)));
    }
}