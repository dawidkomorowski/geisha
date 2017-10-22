using System.Linq;
using Geisha.Common.Math;
using Geisha.Common.Math.SAT;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Math.SAT
{
    [TestFixture]
    public class AxisTests
    {
        [TestCaseSource(nameof(TestCases))]
        public void Project(AxisTestCase testCase)
        {
            // Arrange
            var axis = new Axis(new Vector2(testCase.AxisX, testCase.AxisY));
            var shape = Substitute.For<IShape>();
            shape.GetVertices().Returns(testCase.Vertices);

            // Act
            var actual = axis.Project(shape);

            // Assert
            Assert.That(actual.Min, Is.EqualTo(testCase.ExpectedProjectionMin));
            Assert.That(actual.Max, Is.EqualTo(testCase.ExpectedProjectionMax));
        }

        public class AxisTestCase
        {
            public double AxisX { get; set; }
            public double AxisY { get; set; }
            public Vector2[] Vertices { get; set; }

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