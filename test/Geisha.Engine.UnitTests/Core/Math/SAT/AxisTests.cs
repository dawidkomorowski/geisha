using System;
using System.Linq;
using Geisha.Common.Math;
using Geisha.Common.Math.SAT;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math.SAT
{
    [TestFixture]
    public class AxisTests
    {
        [TestCaseSource(nameof(TestCases))]
        public void GetProjectionOf(AxisTestCase testCase)
        {
            // Arrange
            var axis = new Axis(new Vector2(testCase.AxisX, testCase.AxisY));
            var shape = Substitute.For<IShape>();
            shape.GetVertices().Returns(testCase.Vertices);

            // Act
            var actual1 = axis.GetProjectionOf(shape);
            var actual2 = axis.GetProjectionOf(shape.GetVertices());

            // Assert
            Assert.That(actual1.Min, Is.EqualTo(testCase.ExpectedProjectionMin));
            Assert.That(actual1.Max, Is.EqualTo(testCase.ExpectedProjectionMax));

            Assert.That(actual2.Min, Is.EqualTo(testCase.ExpectedProjectionMin));
            Assert.That(actual2.Max, Is.EqualTo(testCase.ExpectedProjectionMax));
        }

        [TestCase(1, 0, 0, 0, 10, -10, 10)]
        [TestCase(0, 1, 0, 0, 10, -10, 10)]
        [TestCase(1, 0, 5, 8, 10, -5, 15)]
        [TestCase(0, 1, 5, 8, 10, -2, 18)]
        public void GetProjectionOf_Circle(double axisX, double axisY, double centerX, double centerY, double radius, double expectedProjectionMin,
            double expectedProjectionMax)
        {
            // Arrange
            var axis = new Axis(new Vector2(axisX, axisY));
            var shape = Substitute.For<IShape>();
            shape.IsCircle.Returns(true);
            shape.Center.Returns(new Vector2(centerX, centerY));
            shape.Radius.Returns(radius);

            // Act
            var actual = axis.GetProjectionOf(shape);

            // Assert
            Assert.That(actual.Min, Is.EqualTo(expectedProjectionMin));
            Assert.That(actual.Max, Is.EqualTo(expectedProjectionMax));
        }

        [TestCase(1, 0, 0, 0, 0)]
        [TestCase(0, 1, 0, 0, 0)]
        [TestCase(1, 0, 5, 8, 5)]
        [TestCase(0, 1, 5, 8, 8)]
        public void GetProjectionOf_Point(double axisX, double axisY, double pointX, double pointY, double expectedProjection)
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
            public double AxisX { get; set; }
            public double AxisY { get; set; }
            public Vector2[] Vertices { get; set; } = Array.Empty<Vector2>();

            public double ExpectedProjectionMin { get; set; }
            public double ExpectedProjectionMax { get; set; }

            public override string ToString()
            {
                var verticesToString = Vertices.Aggregate("[", (s, v) => s + "(" + v + "), ", s => s.Substring(0, s.Length - 2) + "]");
                return
                    $"{nameof(AxisX)}: {AxisX}, {nameof(AxisY)}: {AxisY}, {nameof(Vertices)}: {verticesToString}, {nameof(ExpectedProjectionMin)}: {ExpectedProjectionMin}, {nameof(ExpectedProjectionMax)}: {ExpectedProjectionMax}";
            }
        }

        private static readonly AxisTestCase[] TestCases =
        {
            // Axis vector is normalized
            new AxisTestCase
            {
                AxisX = 123,
                AxisY = 0,
                Vertices = new[] {new Vector2(-13, 37), new Vector2(25, -18)},
                ExpectedProjectionMin = -13,
                ExpectedProjectionMax = 25
            },
            new AxisTestCase
            {
                AxisX = 0,
                AxisY = 123,
                Vertices = new[] {new Vector2(-13, 37), new Vector2(25, -18)},
                ExpectedProjectionMin = -18,
                ExpectedProjectionMax = 37
            },

            // Axis X (horizontal)
            new AxisTestCase
            {
                AxisX = 1,
                AxisY = 0,
                Vertices = new[] {new Vector2(0, 0)},
                ExpectedProjectionMin = 0,
                ExpectedProjectionMax = 0
            },
            new AxisTestCase
            {
                AxisX = 1,
                AxisY = 0,
                Vertices = new[] {new Vector2(2, 3)},
                ExpectedProjectionMin = 2,
                ExpectedProjectionMax = 2
            },
            new AxisTestCase
            {
                AxisX = 1,
                AxisY = 0,
                Vertices = new[] {new Vector2(-13, 37), new Vector2(25, -18)},
                ExpectedProjectionMin = -13,
                ExpectedProjectionMax = 25
            },
            new AxisTestCase
            {
                AxisX = 1,
                AxisY = 0,
                Vertices = new[] {new Vector2(-13, 37), new Vector2(25, -18), new Vector2(-86, -113), new Vector2(97, 67), new Vector2(7, -3)},
                ExpectedProjectionMin = -86,
                ExpectedProjectionMax = 97
            },

            // Axis Y (vertical)
            new AxisTestCase
            {
                AxisX = 0,
                AxisY = 1,
                Vertices = new[] {new Vector2(0, 0)},
                ExpectedProjectionMin = 0,
                ExpectedProjectionMax = 0
            },
            new AxisTestCase
            {
                AxisX = 0,
                AxisY = 1,
                Vertices = new[] {new Vector2(2, 3)},
                ExpectedProjectionMin = 3,
                ExpectedProjectionMax = 3
            },
            new AxisTestCase
            {
                AxisX = 0,
                AxisY = 1,
                Vertices = new[] {new Vector2(-13, 37), new Vector2(25, -18)},
                ExpectedProjectionMin = -18,
                ExpectedProjectionMax = 37
            },
            new AxisTestCase
            {
                AxisX = 0,
                AxisY = 1,
                Vertices = new[] {new Vector2(-13, 37), new Vector2(25, -18), new Vector2(-86, -113), new Vector2(97, 67), new Vector2(7, -3)},
                ExpectedProjectionMin = -113,
                ExpectedProjectionMax = 67
            }
        };
    }
}