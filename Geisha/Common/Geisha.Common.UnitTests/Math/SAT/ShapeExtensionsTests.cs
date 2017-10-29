﻿using System.Linq;
using Geisha.Common.Math;
using Geisha.Common.Math.SAT;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Math.SAT
{
    [TestFixture]
    public class ShapeExtensionsTests
    {
        [TestCaseSource(nameof(TestCases))]
        public void Overlaps(OverlapsTestCase testCase)
        {
            // Arrange
            var shape1 = testCase.Shape1;
            var shape2 = testCase.Shape2;

            // Act
            var actual1 = shape1.Overlaps(shape2);
            var actual2 = shape2.Overlaps(shape1);

            // Assert
            Assert.That(actual1, Is.EqualTo(testCase.Expected));
            Assert.That(actual2, Is.EqualTo(testCase.Expected));
        }

        public class OverlapsTestCase
        {
            public IShape Shape1 { get; set; }
            public IShape Shape2 { get; set; }

            public bool Expected { get; set; }

            public string Description { get; set; }

            public override string ToString()
            {
                return Description;
            }
        }

        private static readonly OverlapsTestCase[] TestCases =
        {
            // Axis aligned rectangles
            CreateAxisAlignedRectangleTestCase(new Vector2(0, 0), new Vector2(10, 5), new Vector2(10, 0), new Vector2(10, 5), false),
            CreateAxisAlignedRectangleTestCase(new Vector2(0, 0), new Vector2(10, 5), new Vector2(20, 0), new Vector2(10, 5), false),
            CreateAxisAlignedRectangleTestCase(new Vector2(0, 0), new Vector2(10, 5), new Vector2(5, 0), new Vector2(10, 5), true),
            CreateAxisAlignedRectangleTestCase(new Vector2(0, 0), new Vector2(10, 5), new Vector2(0, 5), new Vector2(10, 5), false),
            CreateAxisAlignedRectangleTestCase(new Vector2(0, 0), new Vector2(10, 5), new Vector2(0, 10), new Vector2(10, 5), false),
            CreateAxisAlignedRectangleTestCase(new Vector2(0, 0), new Vector2(10, 5), new Vector2(0, 2.5), new Vector2(10, 5), true),
            CreateAxisAlignedRectangleTestCase(new Vector2(0, 0), new Vector2(10, 5), new Vector2(10, 5), new Vector2(10, 5), false),
            CreateAxisAlignedRectangleTestCase(new Vector2(0, 0), new Vector2(10, 5), new Vector2(20, 10), new Vector2(10, 5), false),
            CreateAxisAlignedRectangleTestCase(new Vector2(0, 0), new Vector2(10, 5), new Vector2(8, 4), new Vector2(10, 5), true),
            CreateAxisAlignedRectangleTestCase(new Vector2(0, 0), new Vector2(10, 5), new Vector2(0, 0), new Vector2(4, 2), true),

            // Rotated rectangles
            CreateRotatedRectangleTestCase(new Vector2(0, 0), new Vector2(10, 10), 45, new Vector2(14.5, 0), new Vector2(10, 10), 45, false),
            CreateRotatedRectangleTestCase(new Vector2(0, 0), new Vector2(10, 10), 45, new Vector2(9, 0), new Vector2(10, 10), 45, true),
            CreateRotatedRectangleTestCase(new Vector2(0, 0), new Vector2(10, 10), 45, new Vector2(9, 5.5), new Vector2(10, 10), 45, false),
            CreateRotatedRectangleTestCase(new Vector2(174, 110), new Vector2(100, 100), 102, new Vector2(271, 187), new Vector2(100, 100), 44, false),
            CreateRotatedRectangleTestCase(new Vector2(174, 110), new Vector2(100, 100), 102, new Vector2(271, 187), new Vector2(100, 100), 56, true),

            // Triangles
            CreatePolygonTestCase(CreateTriangle(new Vector2(196, 200), new Vector2(445, 119), new Vector2(328, 49)),
                CreateTriangle(new Vector2(230, 350), new Vector2(414, 199), new Vector2(99, 232)), false),
            CreatePolygonTestCase(CreateTriangle(new Vector2(196, 200), new Vector2(445, 119), new Vector2(328, 49)),
                CreateTriangle(new Vector2(230, 350), new Vector2(414, 199), new Vector2(99, 192)), true),
            CreatePolygonTestCase(CreateTriangle(new Vector2(443, 241), new Vector2(445, 119), new Vector2(328, 49)),
                CreateTriangle(new Vector2(230, 350), new Vector2(414, 199), new Vector2(99, 192)), false),
            CreatePolygonTestCase(CreateTriangle(new Vector2(443, 241), new Vector2(445, 119), new Vector2(328, 49)),
                CreateTriangle(new Vector2(230, 350), new Vector2(423, 199), new Vector2(99, 192)), true),
            CreatePolygonTestCase(CreateTriangle(new Vector2(112, 181), new Vector2(458, 196), new Vector2(445, 119)),
                CreateTriangle(new Vector2(230, 350), new Vector2(423, 199), new Vector2(99, 192)), false),
            CreatePolygonTestCase(CreateTriangle(new Vector2(112, 181), new Vector2(458, 196), new Vector2(445, 119)),
                CreateTriangle(new Vector2(230, 350), new Vector2(423, 199), new Vector2(99, 166)), true),

            // Polygons
            CreatePolygonTestCase(CreatePolygon(new Vector2(157, 211), new Vector2(342, 166), new Vector2(419, 83), new Vector2(222, 42), new Vector2(84, 136)),
                CreatePolygon(new Vector2(382, 371), new Vector2(551, 239), new Vector2(482, 158), new Vector2(414, 143), new Vector2(299, 201),
                    new Vector2(242, 258), new Vector2(285, 330)), false),
            CreatePolygonTestCase(CreatePolygon(new Vector2(157, 211), new Vector2(351, 201), new Vector2(419, 83), new Vector2(222, 42), new Vector2(84, 136)),
                CreatePolygon(new Vector2(382, 371), new Vector2(551, 239), new Vector2(482, 158), new Vector2(414, 143), new Vector2(299, 201),
                    new Vector2(242, 258), new Vector2(285, 330)), true),

            // Rectangles with custom axes
            new OverlapsTestCase
            {
                Shape1 = CreateRectangle(new Vector2(0, 0), new Vector2(10, 5), 0, new[] {new Axis(Vector2.VectorY)}),
                Shape2 = CreateRectangle(new Vector2(50, 0), new Vector2(10, 5), 0, new[] {new Axis(Vector2.VectorY)}),
                Expected = true,
                Description = "Two not colliding rectangles are considered overlapping as both provide only one axis with overlapping projections."
            },

            // Circles
            CreateCircleTestCase(new Vector2(0, 0), 10, new Vector2(50, 0), 20, false),
            CreateCircleTestCase(new Vector2(0, 0), 10, new Vector2(30, 0), 20, false),
            CreateCircleTestCase(new Vector2(0, 0), 10, new Vector2(29, 0), 20, true),
            CreateCircleTestCase(new Vector2(0, 0), 10, new Vector2(0, 50), 20, false),
            CreateCircleTestCase(new Vector2(0, 0), 10, new Vector2(0, 30), 20, false),
            CreateCircleTestCase(new Vector2(0, 0), 10, new Vector2(0, 29), 20, true),

            // Circles and polygons
            CreateCircleAndPolygonTestCase(new Vector2(268, 123), 50, CreatePolygon(new Vector2(371, 115), new Vector2(274, 210), new Vector2(393, 264)),
                false),
            CreateCircleAndPolygonTestCase(new Vector2(288, 137), 50, CreatePolygon(new Vector2(371, 115), new Vector2(274, 210), new Vector2(393, 264)), true),
            CreateCircleAndPolygonTestCase(new Vector2(219, 216), 50, CreatePolygon(new Vector2(371, 115), new Vector2(274, 210), new Vector2(393, 264)),
                false),
            CreateCircleAndPolygonTestCase(new Vector2(279, 207), 50, CreatePolygon(new Vector2(371, 115), new Vector2(274, 210), new Vector2(393, 264)), true),
            CreateCircleAndPolygonTestCase(new Vector2(231, 241), 50, CreatePolygon(new Vector2(371, 115), new Vector2(274, 210), new Vector2(393, 264)), false)
        };

        private static IShape CreateRectangle(Vector2 center, Vector2 dimension, double rotation = 0, Axis[] axes = null)
        {
            var rot = Matrix3.Rotation(Angle.Deg2Rad(rotation));

            var shape = CreatePolygon(
                (rot * new Vector2(-dimension.X / 2, -dimension.Y / 2).Homogeneous).ToVector2() + new Vector2(center.X, center.Y),
                (rot * new Vector2(+dimension.X / 2, -dimension.Y / 2).Homogeneous).ToVector2() + new Vector2(center.X, center.Y),
                (rot * new Vector2(+dimension.X / 2, +dimension.Y / 2).Homogeneous).ToVector2() + new Vector2(center.X, center.Y),
                (rot * new Vector2(-dimension.X / 2, +dimension.Y / 2).Homogeneous).ToVector2() + new Vector2(center.X, center.Y)
            );
            shape.GetAxes().Returns(axes);

            return shape;
        }

        private static IShape CreateTriangle(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return CreatePolygon(v1, v2, v3);
        }

        private static IShape CreatePolygon(params Vector2[] vertices)
        {
            var shape = Substitute.For<IShape>();
            shape.IsCircle.Returns(false);
            shape.Center.Returns(Vector2.Zero);
            shape.Radius.Returns(0);
            shape.GetVertices().Returns(vertices);
            shape.GetAxes().Returns((Axis[]) null);

            return shape;
        }

        private static IShape CreateCircle(Vector2 center, double radius)
        {
            var shape = Substitute.For<IShape>();
            shape.IsCircle.Returns(true);
            shape.Center.Returns(center);
            shape.Radius.Returns(radius);
            shape.GetVertices().Returns((Vector2[]) null);
            shape.GetAxes().Returns((Axis[]) null);
            return shape;
        }

        private static OverlapsTestCase CreateAxisAlignedRectangleTestCase(Vector2 center1, Vector2 dimension1, Vector2 center2, Vector2 dimension2,
            bool expected)
        {
            return new OverlapsTestCase
            {
                Shape1 = CreateRectangle(center1, dimension1),
                Shape2 = CreateRectangle(center2, dimension2),
                Expected = expected,
                Description =
                    $"Rectangle(center[{center1}], dimension[{dimension1}]) and Rectangle(center[{center2}], dimension[{dimension2}]) should{(expected ? " " : " not ")}overlap."
            };
        }

        private static OverlapsTestCase CreateRotatedRectangleTestCase(Vector2 center1, Vector2 dimension1, double rotation1, Vector2 center2,
            Vector2 dimension2, double rotation2, bool expected)
        {
            return new OverlapsTestCase
            {
                Shape1 = CreateRectangle(center1, dimension1, rotation1),
                Shape2 = CreateRectangle(center2, dimension2, rotation2),
                Expected = expected,
                Description =
                    $"Rectangle(center[{center1}], dimension[{dimension1}], rotation[{rotation1}]) and Rectangle(center[{center2}], dimension[{dimension2}], rotation[{rotation2}]) should{(expected ? " " : " not ")}overlap."
            };
        }

        private static OverlapsTestCase CreatePolygonTestCase(IShape shape1, IShape shape2, bool expected)
        {
            return new OverlapsTestCase
            {
                Shape1 = shape1,
                Shape2 = shape2,
                Expected = expected,
                Description =
                    $"Shape({VerticesFormat(shape1.GetVertices())}) and Shape({VerticesFormat(shape2.GetVertices())}) should{(expected ? " " : " not ")}overlap."
            };
        }

        private static string VerticesFormat(Vector2[] vertices)
        {
            return vertices.Aggregate("[", (s, v) => s + "(" + v + "), ", s => s.Substring(0, s.Length - 2) + "]");
        }

        private static OverlapsTestCase CreateCircleTestCase(Vector2 center1, double radius1, Vector2 center2, double radius2, bool expected)
        {
            return new OverlapsTestCase
            {
                Shape1 = CreateCircle(center1, radius1),
                Shape2 = CreateCircle(center2, radius2),
                Expected = expected,
                Description =
                    $"Circle(center[{center1}], radius:{radius1}) and Circle(center[{center2}], dimension:{radius2}) should{(expected ? " " : " not ")}overlap."
            };
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