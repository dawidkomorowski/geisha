using System;
using System.Linq;
using Geisha.Engine.Core.Math;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math;

[TestFixture]
public class AxisTests
{
    [Test]
    public void Constructor_ShouldCreateAxis()
    {
        // Arrange
        var axisAlignedVector = new Vector2(10, 5);

        // Act
        var axis = new Axis(axisAlignedVector);

        // Assert
        Assert.That(axis.AxisAlignedUnitVector, Is.EqualTo(axisAlignedVector.Unit));
    }

    [TestCaseSource(nameof(TestCases))]
    public void GetProjectionOf_ShouldReturnProjection_GivenSetOfVertices(AxisTestCase testCase)
    {
        // Arrange
        var axis = new Axis(new Vector2(testCase.AxisX, testCase.AxisY));

        // Act
        var actual = axis.GetProjectionOf(testCase.Vertices);

        // Assert
        Assert.That(actual.Min, Is.EqualTo(testCase.ExpectedProjectionMin));
        Assert.That(actual.Max, Is.EqualTo(testCase.ExpectedProjectionMax));
    }

    [TestCase(1, 0, 0, 0, 0)]
    [TestCase(0, 1, 0, 0, 0)]
    [TestCase(1, 0, 5, 8, 5)]
    [TestCase(0, 1, 5, 8, 8)]
    public void GetProjectionOf_ShouldReturnProjection_GivenPoint(double axisX, double axisY, double pointX, double pointY, double expectedProjection)
    {
        // Arrange
        var axis = new Axis(new Vector2(axisX, axisY));
        var point = new Vector2(pointX, pointY);

        // Act
        var actual = axis.GetProjectionOf(point);

        // Assert
        Assert.That(actual.Min, Is.EqualTo(expectedProjection));
        Assert.That(actual.Max, Is.EqualTo(expectedProjection));
    }

    public class AxisTestCase
    {
        public double AxisX { get; init; }
        public double AxisY { get; init; }
        public Vector2[] Vertices { get; init; } = Array.Empty<Vector2>();

        public double ExpectedProjectionMin { get; init; }
        public double ExpectedProjectionMax { get; init; }

        public override string ToString()
        {
            var verticesToString = Vertices.Aggregate("[", (s, v) => s + "(" + v + "), ", s => s[..^2] + "]");
            return
                $"{nameof(AxisX)}: {AxisX}, {nameof(AxisY)}: {AxisY}, {nameof(Vertices)}: {verticesToString}, {nameof(ExpectedProjectionMin)}: {ExpectedProjectionMin}, {nameof(ExpectedProjectionMax)}: {ExpectedProjectionMax}";
        }
    }

    private static readonly AxisTestCase[] TestCases =
    {
        // Axis vector is normalized
        new()
        {
            AxisX = 123,
            AxisY = 0,
            Vertices = new[] { new Vector2(-13, 37), new Vector2(25, -18) },
            ExpectedProjectionMin = -13,
            ExpectedProjectionMax = 25
        },
        new()
        {
            AxisX = 0,
            AxisY = 123,
            Vertices = new[] { new Vector2(-13, 37), new Vector2(25, -18) },
            ExpectedProjectionMin = -18,
            ExpectedProjectionMax = 37
        },

        // Axis X (horizontal)
        new()
        {
            AxisX = 1,
            AxisY = 0,
            Vertices = new[] { new Vector2(0, 0) },
            ExpectedProjectionMin = 0,
            ExpectedProjectionMax = 0
        },
        new()
        {
            AxisX = 1,
            AxisY = 0,
            Vertices = new[] { new Vector2(2, 3) },
            ExpectedProjectionMin = 2,
            ExpectedProjectionMax = 2
        },
        new()
        {
            AxisX = 1,
            AxisY = 0,
            Vertices = new[] { new Vector2(-13, 37), new Vector2(25, -18) },
            ExpectedProjectionMin = -13,
            ExpectedProjectionMax = 25
        },
        new()
        {
            AxisX = 1,
            AxisY = 0,
            Vertices = new[] { new Vector2(-13, 37), new Vector2(25, -18), new Vector2(-86, -113), new Vector2(97, 67), new Vector2(7, -3) },
            ExpectedProjectionMin = -86,
            ExpectedProjectionMax = 97
        },

        // Axis Y (vertical)
        new()
        {
            AxisX = 0,
            AxisY = 1,
            Vertices = new[] { new Vector2(0, 0) },
            ExpectedProjectionMin = 0,
            ExpectedProjectionMax = 0
        },
        new()
        {
            AxisX = 0,
            AxisY = 1,
            Vertices = new[] { new Vector2(2, 3) },
            ExpectedProjectionMin = 3,
            ExpectedProjectionMax = 3
        },
        new()
        {
            AxisX = 0,
            AxisY = 1,
            Vertices = new[] { new Vector2(-13, 37), new Vector2(25, -18) },
            ExpectedProjectionMin = -18,
            ExpectedProjectionMax = 37
        },
        new()
        {
            AxisX = 0,
            AxisY = 1,
            Vertices = new[] { new Vector2(-13, 37), new Vector2(25, -18), new Vector2(-86, -113), new Vector2(97, 67), new Vector2(7, -3) },
            ExpectedProjectionMin = -113,
            ExpectedProjectionMax = 67
        }
    };
}