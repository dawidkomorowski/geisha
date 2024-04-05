using System;
using Geisha.Engine.Core.Math;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math;

[TestFixture]
public class Polygon2DTests
{
    [Test]
    public void GetOrientation_ShouldReturnClockwise_GivenClockwiseOrientedPolygon()
    {
        // Arrange
        Span<Vector2> polygon = stackalloc Vector2[3];
        polygon[0] = new Vector2(-2, -2);
        polygon[1] = new Vector2(0, 3);
        polygon[2] = new Vector2(1, -1);

        // Act
        var actual = Polygon2D.GetOrientation(polygon);

        // Assert
        Assert.That(actual, Is.EqualTo(Polygon2DOrientation.Clockwise));
    }

    [Test]
    public void GetOrientation_ShouldReturnCounterClockwise_GivenCounterClockwiseOrientedPolygon()
    {
        // Arrange
        Span<Vector2> polygon = stackalloc Vector2[3];
        polygon[0] = new Vector2(-2, -2);
        polygon[1] = new Vector2(1, -1);
        polygon[2] = new Vector2(0, 3);

        // Act
        var actual = Polygon2D.GetOrientation(polygon);

        // Assert
        Assert.That(actual, Is.EqualTo(Polygon2DOrientation.CounterClockwise));
    }

    [Test]
    public void GetOrientation_ShouldReturnCollinear_GivenPolygonIsStraightLine()
    {
        // Arrange
        Span<Vector2> polygon = stackalloc Vector2[3];
        polygon[0] = new Vector2(-2, -2);
        polygon[1] = new Vector2(1, 1);
        polygon[2] = new Vector2(3, 3);

        // Act
        var actual = Polygon2D.GetOrientation(polygon);

        // Assert
        Assert.That(actual, Is.EqualTo(Polygon2DOrientation.Collinear));
    }
}