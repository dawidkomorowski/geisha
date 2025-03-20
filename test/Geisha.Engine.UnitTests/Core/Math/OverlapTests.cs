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

    // Horizontal
    [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 20, 0, 10, 5, 0, /*E*/ false, 0, 0, 0,
        TestName = $"01_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 10, 0, 10, 5, 0, /*E*/ true, -1, 0, 0,
        TestName = $"02_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 9, 0, 10, 5, 0, /*E*/ true, -1, 0, 1,
        TestName = $"03_{nameof(RectangleAndRectangle)}")]
    // Vertical
    [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 0, 10, 10, 5, 0, /*E*/ false, 0, 0, 0,
        TestName = $"04_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 0, 5, 10, 5, 0, /*E*/ true, 0, -1, 0,
        TestName = $"05_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 0, 4, 10, 5, 0, /*E*/ true, 0, -1, 1,
        TestName = $"06_{nameof(RectangleAndRectangle)}")]
    // Diagonal
    [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 20, 10, 10, 5, 0, /*E*/ false, 0, 0, 0,
        TestName = $"07_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 10, 5, 10, 5, 0, /*E*/ true, -1, 0, 0,
        TestName = $"08_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 9, 4, 10, 5, 0, /*E*/ true, -1, 0, 1,
        TestName = $"09_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 8, 4, 10, 5, 0, /*E*/ true, 0, -1, 1,
        TestName = $"10_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 9, 3, 10, 5, 0, /*E*/ true, -1, 0, 1,
        TestName = $"11_{nameof(RectangleAndRectangle)}")]
    // Rectangle inside Rectangle
    [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 3, 0, 2, 4, 0, /*E*/ true, -1, 0, 3,
        TestName = $"12_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ -3, 0, 2, 4, 0, /*E*/ true, 1, 0, 3,
        TestName = $"13_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 2, 1, 4, 2, 0, /*E*/ true, 0, -1, 2.5,
        TestName = $"14_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 2, -1, 4, 2, 0, /*E*/ true, 0, 1, 2.5,
        TestName = $"15_{nameof(RectangleAndRectangle)}")]
    // One rotated
    [TestCase( /*R1*/ 0, 0, 10, 10, 0, /*R2*/ 12.5, 0, 10, 10, 45, /*E*/ false, 0, 0, 0,
        TestName = $"16_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 0, 0, 10, 10, 0, /*R2*/ 12, 0, 10, 10, 45, /*E*/ true, -1, 0, 0.071067,
        TestName = $"17_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 0, 0, 10, 10, 0, /*R2*/ 0, 12.5, 10, 10, 45, /*E*/ false, 0, 0, 0,
        TestName = $"18_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 0, 0, 10, 10, 0, /*R2*/ 0, 12, 10, 10, 45, /*E*/ true, 0, -1, 0.071067,
        TestName = $"19_{nameof(RectangleAndRectangle)}")]
    // Two rotated
    [TestCase( /*R1*/ 0, 0, 10, 10, 45, /*R2*/ 14.5, 0, 10, 10, 45, /*E*/ false, 0, 0, 0,
        TestName = $"20_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 0, 0, 10, 10, 45, /*R2*/ 9, 0, 10, 10, 45, /*E*/ true, -0.707106, 0.707106, 3.636038,
        TestName = $"21_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 0, 0, 10, 10, 45, /*R2*/ 10, 1, 10, 10, 45, /*E*/ true, -0.707106, -0.707106, 2.221825,
        TestName = $"22_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 0, 0, 10, 10, 45, /*R2*/ 10, -1, 10, 10, 45, /*E*/ true, -0.707106, 0.707106, 2.221825,
        TestName = $"23_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 0, 0, 10, 10, 45, /*R2*/ 9, 5.5, 10, 10, 45, /*E*/ false, 0, 0, 0,
        TestName = $"24_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 174, 110, 100, 100, 102, /*R2*/ 271, 187, 100, 100, 44, /*E*/ false, 0, 0, 0,
        TestName = $"25_{nameof(RectangleAndRectangle)}")]
    [TestCase( /*R1*/ 174, 110, 100, 100, 102, /*R2*/ 271, 187, 100, 100, 56, /*E*/ true, -0.559192, -0.829037, 2.622303,
        TestName = $"26_{nameof(RectangleAndRectangle)}")]
    public void RectangleAndRectangle(
        double x1, double y1, double w1, double h1, double rotation1,
        double x2, double y2, double w2, double h2, double rotation2,
        bool overlap, double mtvX, double mtvY, double mtvLength
    )
    {
        // Arrange
        var rotationMatrix1 = Matrix3x3.CreateTranslation(new Vector2(x1, y1)) *
                              Matrix3x3.CreateRotation(Angle.Deg2Rad(rotation1)) *
                              Matrix3x3.CreateTranslation(new Vector2(-x1, -y1));

        var rotationMatrix2 = Matrix3x3.CreateTranslation(new Vector2(x2, y2)) *
                              Matrix3x3.CreateRotation(Angle.Deg2Rad(rotation2)) *
                              Matrix3x3.CreateTranslation(new Vector2(-x2, -y2));

        var rectangle1 = new Rectangle(new Vector2(x1, y1), new Vector2(w1, h1)).Transform(rotationMatrix1);
        var rectangle2 = new Rectangle(new Vector2(x2, y2), new Vector2(w2, h2)).Transform(rotationMatrix2);

        using var visualOutput = TestKit.CreateVisualOutput(scale: 20, enabled: false);
        visualOutput.DrawRectangle(rectangle1, Color.Red);
        visualOutput.DrawRectangle(rectangle2, Color.Blue);
        visualOutput.SaveToFile();

        // Act
        var actual1 = rectangle1.Overlaps(rectangle2);
        var actual2 = rectangle2.Overlaps(rectangle1);
        var actual3 = rectangle1.Overlaps(rectangle2, out var mtv1);
        var actual4 = rectangle2.Overlaps(rectangle1, out var mtv2);

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

    [Test]
    public void RectangleAndRectangle_WhenRectanglesHaveTheSameCenter()
    {
        // Arrange
        var rectangle1 = new Rectangle(new Vector2(20, 20));
        var rectangle2 = new Rectangle(new Vector2(10, 10));

        // Act
        var actual1 = rectangle1.Overlaps(rectangle2, out var mtv1);
        var actual2 = rectangle2.Overlaps(rectangle1, out var mtv2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actual1, Is.True);
            Assert.That(actual2, Is.True);

            Assert.That(mtv1.Direction, Is.EqualTo(Vector2.UnitX.Opposite));
            Assert.That(mtv1.Length, Is.EqualTo(15));

            Assert.That(mtv2.Direction, Is.EqualTo(Vector2.UnitX.Opposite));
            Assert.That(mtv2.Length, Is.EqualTo(15));
        });
    }

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
    // TODO Rectangle inside of Circle?
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