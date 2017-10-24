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
            CreateRotatedRectangleTestCase(new Vector2(0, 0), new Vector2(10, 5), 0, new Vector2(10, 0), new Vector2(10, 5), 0, false) // TODO
        };

        private static IShape CreateRectangle(Vector2 center, Vector2 dimension, double rotation = 0)
        {
            var rot = Matrix3.Rotation(rotation);

            var shape = Substitute.For<IShape>();
            shape.GetVertices().Returns(new[]
            {
                (rot * new Vector2(center.X - dimension.X / 2, center.Y - dimension.Y / 2).Homogeneous).ToVector2(),
                (rot * new Vector2(center.X + dimension.X / 2, center.Y - dimension.Y / 2).Homogeneous).ToVector2(),
                (rot * new Vector2(center.X + dimension.X / 2, center.Y + dimension.Y / 2).Homogeneous).ToVector2(),
                (rot * new Vector2(center.X - dimension.X / 2, center.Y + dimension.Y / 2).Homogeneous).ToVector2()
            });
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
    }
}