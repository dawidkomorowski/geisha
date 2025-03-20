using System.Collections.Generic;
using Geisha.Engine.Core.Math;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math;

[TestFixture]
[DefaultFloatingPointTolerance(Epsilon)]
public class OverlapTests
{
    private const double Epsilon = 1e-6;
    private static IEqualityComparer<Vector2> Vector2Comparer => CommonEqualityComparer.Vector2(Epsilon);


    // Circle outside of Rectangle
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 70, 20, 10, /*E*/ false, 0, 0, 0,
        TestName = $"01_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ -10, 20, 10, /*E*/ false, 0, 0, 0,
        TestName = $"02_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 30, 50, 10, /*E*/ false, 0, 0, 0,
        TestName = $"03_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 30, -10, 10, /*E*/ false, 0, 0, 0,
        TestName = $"04_{nameof(RectangleAndCircle)}")]
    // Circle touching edge of Rectangle
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 60, 20, 10, /*E*/ true, -1, 0, 0,
        TestName = $"05_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 0, 20, 10, /*E*/ true, 1, 0, 0,
        TestName = $"06_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 30, 40, 10, /*E*/ true, 0, -1, 0,
        TestName = $"07_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 30, 0, 10, /*E*/ true, 0, 1, 0,
        TestName = $"08_{nameof(RectangleAndCircle)}")]
    // Circle contains vertex of Rectangle
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 52.5, 32.5, 5, /*E*/ true, -0.707106, -0.707106, 1.464466,
        TestName = $"09_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 7.5, 32.5, 5, /*E*/ true, 0.707106, -0.707106, 1.464466,
        TestName = $"10_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 7.5, 7.5, 5, /*E*/ true, 0.707106, 0.707106, 1.464466,
        TestName = $"11_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 52.5, 7.5, 5, /*E*/ true, -0.707106, 0.707106, 1.464466,
        TestName = $"12_{nameof(RectangleAndCircle)}")]
    // Circle overlaps edge of Rectangle
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 52.5, 20, 5, /*E*/ true, -1, 0, 2.5,
        TestName = $"13_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 7.5, 20, 5, /*E*/ true, 1, 0, 2.5,
        TestName = $"14_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 30, 32.5, 5, /*E*/ true, 0, -1, 2.5,
        TestName = $"15_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 30, 7.5, 5, /*E*/ true, 0, 1, 2.5,
        TestName = $"16_{nameof(RectangleAndCircle)}")]
    // Circle outside of Rectangle but with overlapping projection onto Rectangle axes
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 58, 38, 10, /*E*/ false, 0, 0, 0,
        TestName = $"17_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 2, 38, 10, /*E*/ false, 0, 0, 0,
        TestName = $"18_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 2, 2, 10, /*E*/ false, 0, 0, 0,
        TestName = $"19_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 58, 2, 10, /*E*/ false, 0, 0, 0,
        TestName = $"20_{nameof(RectangleAndCircle)}")]
    // Circle inside of Rectangle
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 30, 20, 5, /*E*/ true, 0, 1, 15,
        TestName = $"21_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 43, 20, 5, /*E*/ true, -1, 0, 12,
        TestName = $"22_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 17, 20, 5, /*E*/ true, 1, 0, 12,
        TestName = $"23_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 30, 23, 5, /*E*/ true, 0, -1, 12,
        TestName = $"24_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 30, 17, 5, /*E*/ true, 0, 1, 12,
        TestName = $"25_{nameof(RectangleAndCircle)}")]
    // Circle and rotated Rectangle
    [TestCase( /*R*/ 10, 20, 100, 50, 45, /*C*/ 65, 75, 25, /*E*/ false, 0, 0, 0,
        TestName = $"26_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 10, 20, 100, 50, 45, /*C*/ 60, 70, 25, /*E*/ true, -0.707106, -0.707106, 4.289322,
        TestName = $"27_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 10, 20, 100, 50, 45, /*C*/ 85, 40, 25, /*E*/ true, -0.994458, -0.105133, 2.910592,
        TestName = $"28_{nameof(RectangleAndCircle)}")]
    [TestCase( /*R*/ 10, 20, 100, 50, 45, /*C*/ 50, -15, 25, /*E*/ false, 0, 0, 0,
        TestName = $"29_{nameof(RectangleAndCircle)}")]
    public void RectangleAndCircle(
        double rx, double ry, double rw, double rh, double rotation,
        double cx, double cy, double cr,
        bool overlap, double mtvX, double mtvY, double mtvLength
    )
    {
        // Arrange
        var rotationMatrix = Matrix3x3.CreateTranslation(new Vector2(rx, ry)) *
                             Matrix3x3.CreateRotation(Angle.Deg2Rad(rotation)) *
                             Matrix3x3.CreateTranslation(new Vector2(-rx, -ry));

        var rectangle = new Rectangle(new Vector2(rx, ry), new Vector2(rw, rh)).Transform(rotationMatrix);
        var circle = new Circle(new Vector2(cx, cy), cr);

        using var visualOutput = TestKit.CreateVisualOutput(scale: 5, enabled: false);
        visualOutput.DrawCircle(circle, Color.Red);
        visualOutput.DrawRectangle(rectangle, Color.Blue);
        visualOutput.SaveToFile();

        // Act
        var actual1 = rectangle.Overlaps(circle);
        var actual2 = circle.Overlaps(rectangle);
        var actual3 = rectangle.Overlaps(circle, out var mtv1);
        var actual4 = circle.Overlaps(rectangle, out var mtv2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actual1, Is.EqualTo(overlap));
            Assert.That(actual2, Is.EqualTo(overlap));
            Assert.That(actual3, Is.EqualTo(overlap));
            Assert.That(actual4, Is.EqualTo(overlap));

            Assert.That(mtv1.Direction, Is.EqualTo(new Vector2(mtvX, mtvY)).Using(Vector2Comparer));
            Assert.That(mtv1.Length, Is.EqualTo(mtvLength));

            Assert.That(mtv2.Direction, Is.EqualTo(new Vector2(mtvX, mtvY).Opposite).Using(Vector2Comparer));
            Assert.That(mtv2.Length, Is.EqualTo(mtvLength));
        });
    }
}