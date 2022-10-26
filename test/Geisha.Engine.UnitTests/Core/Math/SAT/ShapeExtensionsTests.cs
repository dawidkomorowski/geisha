using System;
using System.Linq;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Math.SAT;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math.SAT
{
    [TestFixture]
    public class ShapeExtensionsTests
    {
        [TestCaseSource(nameof(OverlapsTestCases))]
        public void Overlaps(OverlapsTestCase testCase)
        {
            // Arrange
            var shape1 = testCase.Shape1 ?? throw new ArgumentException($"{nameof(OverlapsTestCase.Shape1)} cannot be null.");
            var shape2 = testCase.Shape2 ?? throw new ArgumentException($"{nameof(OverlapsTestCase.Shape2)} cannot be null.");

            // Act
            var actual1 = shape1.Overlaps(shape2);
            var actual2 = shape2.Overlaps(shape1);

            // Assert
            Assert.That(actual1, Is.EqualTo(testCase.Expected));
            Assert.That(actual2, Is.EqualTo(testCase.Expected));
        }

        public class OverlapsTestCase
        {
            public IShape? Shape1 { get; set; }
            public IShape? Shape2 { get; set; }

            public bool Expected { get; set; }

            public string? Description { get; set; }

            public override string ToString()
            {
                return Description ?? throw new InvalidOperationException($"{nameof(Description)} cannot be null.");
            }
        }

        private static readonly OverlapsTestCase[] OverlapsTestCases =
        {
            

            // Circles and polygons
            CreateCircleAndPolygonTestCase(new Vector2(268, 123), 50, CreatePolygon(new Vector2(371, 115), new Vector2(393, 264), new Vector2(274, 210)),
                false),
            CreateCircleAndPolygonTestCase(new Vector2(288, 137), 50, CreatePolygon(new Vector2(371, 115), new Vector2(393, 264), new Vector2(274, 210)), true),
            CreateCircleAndPolygonTestCase(new Vector2(400, 270), 50, CreatePolygon(new Vector2(371, 115), new Vector2(393, 264), new Vector2(274, 210)), true),
            CreateCircleAndPolygonTestCase(new Vector2(219, 216), 50, CreatePolygon(new Vector2(371, 115), new Vector2(393, 264), new Vector2(274, 210)),
                false),
            CreateCircleAndPolygonTestCase(new Vector2(279, 207), 50, CreatePolygon(new Vector2(371, 115), new Vector2(393, 264), new Vector2(274, 210)), true),
            CreateCircleAndPolygonTestCase(new Vector2(231, 241), 50, CreatePolygon(new Vector2(371, 115), new Vector2(393, 264), new Vector2(274, 210)),
                false),
            CreateCircleAndPolygonTestCase(new Vector2(326, 240), 10, CreatePolygon(new Vector2(371, 115), new Vector2(393, 264), new Vector2(274, 210)), true),
            CreateCircleAndPolygonTestCase(new Vector2(332, 230), 10, CreatePolygon(new Vector2(371, 115), new Vector2(393, 264), new Vector2(274, 210)), true),
            // Circle inside a polygon touching none of polygon vertices or edges
            CreateCircleAndPolygonTestCase(new Vector2(342, 196), 10, CreatePolygon(new Vector2(371, 115), new Vector2(393, 264), new Vector2(274, 210)), true)
        };

        private static IShape CreatePolygon(params Vector2[] vertices)
        {
            var shape = Substitute.For<IShape>();
            shape.IsCircle.Returns(false);
            shape.Center.Returns(Vector2.Zero);
            shape.Radius.Returns(0);
            shape.GetVertices().Returns(vertices);
            shape.GetAxes().Returns(Array.Empty<Axis>());

            return shape;
        }

        private static IShape CreateCircle(Vector2 center, double radius)
        {
            var shape = Substitute.For<IShape>();
            shape.IsCircle.Returns(true);
            shape.Center.Returns(center);
            shape.Radius.Returns(radius);
            shape.GetVertices().Throws(ci => new NotSupportedException());
            shape.GetAxes().Throws(ci => new NotSupportedException());
            return shape;
        }

        private static string VerticesFormat(Vector2[] vertices)
        {
            return vertices.Aggregate("[", (s, v) => s + "(" + v + "), ", s => s.Substring(0, s.Length - 2) + "]");
        }

        private static OverlapsTestCase CreateCircleAndPolygonTestCase(Vector2 circleCenter, double circleRadius, IShape polygon, bool expected)
        {
            return new OverlapsTestCase
            {
                Shape1 = CreateCircle(circleCenter, circleRadius),
                Shape2 = polygon,
                Expected = expected,
                Description =
                    $"Circle(center[{circleCenter}], radius:{circleRadius}) and Polygon({VerticesFormat(polygon.GetVertices())}) should{(expected ? " " : " not ")}overlap."
            };
        }
    }
}