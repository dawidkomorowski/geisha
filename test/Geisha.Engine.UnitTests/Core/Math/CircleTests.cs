﻿using Geisha.Engine.Core.Math;
using Geisha.TestUtils;
using NUnit.Framework;
using System.Collections.Generic;

namespace Geisha.Engine.UnitTests.Core.Math
{
    [TestFixture]
    [DefaultFloatingPointTolerance(Epsilon)]
    public class CircleTests
    {
        private const double Epsilon = 1e-6;
        private static IEqualityComparer<Vector2> Vector2Comparer => CommonEqualityComparer.Vector2(Epsilon);

        #region Constructors

        [TestCase(0)]
        [TestCase(45.173)]
        public void Constructor_FromRadius_ShouldCreateCircleAtOriginWithGivenRadius(double radius)
        {
            // Arrange
            // Act
            var circle = new Circle(radius);

            // Assert
            Assert.That(circle.Center, Is.EqualTo(Vector2.Zero));
            Assert.That(circle.Radius, Is.EqualTo(radius));
        }

        [TestCase(0, 0, 0)]
        [TestCase(58.373, -82.671, 45.654)]
        public void Constructor_FromCenterAndRadius_ShouldCreateCircleWithGivenCenterAndRadius(double centerX, double centerY, double radius)
        {
            // Arrange
            // Act
            var circle = new Circle(new Vector2(centerX, centerY), radius);

            // Assert
            Assert.That(circle.Center.X, Is.EqualTo(centerX));
            Assert.That(circle.Center.Y, Is.EqualTo(centerY));
            Assert.That(circle.Radius, Is.EqualTo(radius));
        }

        #endregion

        #region Methods

        [Test]
        public void Transform_ShouldTransformCenterAndRadius()
        {
            // Arrange
            var circle = new Circle(new Vector2(2, 1), 2);
            var transform = Matrix3x3.CreateTranslation(new Vector2(2, 1))
                            * Matrix3x3.CreateRotation(Angle.Deg2Rad(90))
                            * Matrix3x3.CreateScale(new Vector2(2, 2));

            // Act
            var actual = circle.Transform(transform);

            // Assert
            Assert.That(actual.Center, Is.EqualTo(new Vector2(0, 5)).Using(Vector2Comparer));
            Assert.That(actual.Radius, Is.EqualTo(4d));
        }

        [TestCase( /*C*/ 0, 0, 10, /*P*/ 15, 0, /*E*/ false)]
        [TestCase( /*C*/ 0, 0, 10, /*P*/ 10, 0, /*E*/ true)]
        [TestCase( /*C*/ 0, 0, 10, /*P*/ 5, 0, /*E*/ true)]
        [TestCase( /*C*/ 0, 0, 10, /*P*/ 0, 15, /*E*/ false)]
        [TestCase( /*C*/ 0, 0, 10, /*P*/ 0, 10, /*E*/ true)]
        [TestCase( /*C*/ 0, 0, 10, /*P*/ 0, 5, /*E*/ true)]
        [TestCase( /*C*/ 0, 0, 10, /*P*/ 7.5, 7.5, /*E*/ false)]
        [TestCase( /*C*/ 0, 0, 10, /*P*/ 7, 7, /*E*/ true)]
        [TestCase( /*C*/ 0, 0, 3, /*P*/ 4, 0, /*E*/ false)]
        [TestCase( /*C*/ 0, 0, 3, /*P*/ 3, 0, /*E*/ true)]
        [TestCase( /*C*/ 0, 0, 3, /*P*/ 2, 0, /*E*/ true)]
        [TestCase( /*C*/ 5, -3, 10, /*P*/ 12.5, 4.5, /*E*/ false)]
        [TestCase( /*C*/ 5, -3, 10, /*P*/ 12, 4, /*E*/ true)]
        public void Contains(double cx, double cy, double r, double px, double py, bool expected)
        {
            // Arrange
            var circle = new Circle(new Vector2(cx, cy), r);
            var point = new Vector2(px, py);

            // Act
            var actual = circle.Contains(point);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ToEllipse_ShouldReturnEllipseRepresentingThisCircle()
        {
            // Arrange
            var circle = new Circle(new Vector2(47.196, 75.639), 15.627);

            // Act
            var ellipse = circle.ToEllipse();

            // Assert
            Assert.That(ellipse.Center, Is.EqualTo(circle.Center));
            Assert.That(ellipse.RadiusX, Is.EqualTo(circle.Radius));
            Assert.That(ellipse.RadiusY, Is.EqualTo(circle.Radius));
        }

        [Test]
        public void GetBoundingRectangle_ShouldReturnMinimalAxisAlignedRectangleContainingThisCircle()
        {
            // Arrange
            var circle = new Circle(new Vector2(47.196, 75.639), 15.627);

            // Act
            var boundingRectangle = circle.GetBoundingRectangle();

            // Assert
            Assert.That(boundingRectangle.Center, Is.EqualTo(new Vector2(47.196, 75.639)));
            Assert.That(boundingRectangle.Dimensions, Is.EqualTo(new Vector2(31.254, 31.254)));
        }

        [TestCase(0, 0, 0, "Center: X: 0, Y: 0, Radius: 0")]
        [TestCase(74.025, -27.169, 15.627, "Center: X: 74.025, Y: -27.169, Radius: 15.627")]
        [SetCulture("")]
        public void ToString_Test(double x, double y, double radius, string expected)
        {
            // Arrange
            var circle = new Circle(new Vector2(x, y), radius);

            // Act
            var actual = circle.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase( /*C1*/0, 0, 0, /*C2*/ 0, 0, 0, /*E*/ true)]
        [TestCase( /*C1*/1, 2, 3, /*C2*/ 1, 2, 3, /*E*/ true)]
        [TestCase( /*C1*/1, 2, 3, /*C2*/ 0, 2, 3, /*E*/ false)]
        [TestCase( /*C1*/1, 2, 3, /*C2*/ 1, 0, 3, /*E*/ false)]
        [TestCase( /*C1*/1, 2, 3, /*C2*/ 1, 2, 0, /*E*/ false)]
        public void EqualityMembers_ShouldEqualCircle_WhenCenterAndRadiusAreEqual(double x1, double y1, double r1, double x2, double y2, double r2,
            bool expectedIsEqual)
        {
            // Arrange
            var circle1 = new Circle(new Vector2(x1, y1), r1);
            var circle2 = new Circle(new Vector2(x2, y2), r2);

            // Act
            // Assert
            AssertEqualityMembers
                .ForValues(circle1, circle2)
                .UsingEqualityOperator((x, y) => x == y)
                .UsingInequalityOperator((x, y) => x != y)
                .EqualityIsExpectedToBe(expectedIsEqual);
        }

        #endregion
    }
}