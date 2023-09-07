using Geisha.Engine.Core.Math;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math;

[TestFixture]
public class QuadTests
{
    [Test]
    public void Constructor_ShouldCreateQuadWithCorrectVertices()
    {
        // Arrange
        var v1 = new Vector2(1, 2);
        var v2 = new Vector2(3, 4);
        var v3 = new Vector2(5, 6);
        var v4 = new Vector2(7, 8);

        // Act
        var actual = new Quad(v1, v2, v3, v4);

        // Assert
        Assert.That(actual.V1, Is.EqualTo(v1));
        Assert.That(actual.V2, Is.EqualTo(v2));
        Assert.That(actual.V3, Is.EqualTo(v3));
        Assert.That(actual.V4, Is.EqualTo(v4));
    }

    [TestCase(new double[] { -1, -1, 1, -1, 1, 1, -1, 1 })]
    [TestCase(new[] { 1.2, 2.3, 3.4, 4.5, 5.6, 6.7, 7.8, 8.9 })]
    public void Transform_ShouldTransformEachVertexOfQuad(double[] q)
    {
        // Arrange
        var quad = new Quad(
            new Vector2(q[0], q[1]),
            new Vector2(q[2], q[3]),
            new Vector2(q[4], q[5]),
            new Vector2(q[6], q[7])
        );
        var transform = new Matrix3x3(1, 2, 3, 4, 5, 6, 7, 8, 9);

        var expectedV1 = (transform * quad.V1.Homogeneous).ToVector2();
        var expectedV2 = (transform * quad.V2.Homogeneous).ToVector2();
        var expectedV3 = (transform * quad.V3.Homogeneous).ToVector2();
        var expectedV4 = (transform * quad.V4.Homogeneous).ToVector2();

        // Act
        var actual = quad.Transform(transform);

        // Assert
        Assert.That(actual.V1, Is.EqualTo(expectedV1));
        Assert.That(actual.V2, Is.EqualTo(expectedV2));
        Assert.That(actual.V3, Is.EqualTo(expectedV3));
        Assert.That(actual.V4, Is.EqualTo(expectedV4));
    }

    [Test]
    public void GetBoundingRectangle_ShouldReturnMinimalAxisAlignedRectangleContainingThisQuad_WhenQuadRepresentsAxisAlignedRectangle()
    {
        // Arrange
        // TODO
        var quad = new Quad();

        // Act
        var boundingRectangle = quad.GetBoundingRectangle();

        // Assert
        Assert.That(boundingRectangle.Center, Is.EqualTo(new Vector2(4, 2)));
        Assert.That(boundingRectangle.Dimensions, Is.EqualTo(new Vector2(10, 6)));
    }

    [Test]
    public void GetBoundingRectangle_ShouldReturnMinimalAxisAlignedRectangleContainingThisQuad_WhenQuadIsArbitrarySetOfVertices()
    {
        // Arrange
        // TODO
        var quad = new Quad();

        // Act
        var boundingRectangle = quad.GetBoundingRectangle();

        // Assert
        Assert.That(boundingRectangle.Center, Is.EqualTo(new Vector2(4, 2)));
        Assert.That(boundingRectangle.Dimensions, Is.EqualTo(new Vector2(10, 6)));
    }

    [TestCase(new double[] { 0, 0, 0, 0, 0, 0, 0, 0 }, "V1: X: 0, Y: 0, V2: X: 0, Y: 0, V3: X: 0, Y: 0, V4: X: 0, Y: 0")]
    [TestCase(new[] { 1.2, 2.3, 3.4, 4.5, 5.6, 6.7, 7.8, 8.9 }, "V1: X: 1.2, Y: 2.3, V2: X: 3.4, Y: 4.5, V3: X: 5.6, Y: 6.7, V4: X: 7.8, Y: 8.9")]
    [SetCulture("")]
    public void ToString_Test(double[] q, string expected)
    {
        // Arrange
        var quad = new Quad(
            new Vector2(q[0], q[1]),
            new Vector2(q[2], q[3]),
            new Vector2(q[4], q[5]),
            new Vector2(q[6], q[7])
        );

        // Act
        var actual = quad.ToString();

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase(new double[] { 0, 0, 0, 0, 0, 0, 0, 0 }, new double[] { 0, 0, 0, 0, 0, 0, 0, 0 }, true)]
    [TestCase(new double[] { 1, 2, 3, 4, 5, 6, 7, 8 }, new double[] { 1, 2, 3, 4, 5, 6, 7, 8 }, true)]
    [TestCase(new double[] { 1, 2, 3, 4, 5, 6, 7, 8 }, new double[] { 0, 2, 3, 4, 5, 6, 7, 8 }, false)]
    [TestCase(new double[] { 1, 2, 3, 4, 5, 6, 7, 8 }, new double[] { 1, 0, 3, 4, 5, 6, 7, 8 }, false)]
    [TestCase(new double[] { 1, 2, 3, 4, 5, 6, 7, 8 }, new double[] { 1, 2, 0, 4, 5, 6, 7, 8 }, false)]
    [TestCase(new double[] { 1, 2, 3, 4, 5, 6, 7, 8 }, new double[] { 1, 2, 3, 0, 5, 6, 7, 8 }, false)]
    [TestCase(new double[] { 1, 2, 3, 4, 5, 6, 7, 8 }, new double[] { 1, 2, 3, 4, 0, 6, 7, 8 }, false)]
    [TestCase(new double[] { 1, 2, 3, 4, 5, 6, 7, 8 }, new double[] { 1, 2, 3, 4, 5, 0, 7, 8 }, false)]
    [TestCase(new double[] { 1, 2, 3, 4, 5, 6, 7, 8 }, new double[] { 1, 2, 3, 4, 5, 6, 0, 8 }, false)]
    [TestCase(new double[] { 1, 2, 3, 4, 5, 6, 7, 8 }, new double[] { 1, 2, 3, 4, 5, 6, 7, 0 }, false)]
    public void EqualityMembers_ShouldEqualQuad_WhenAllVerticesAreEqual(double[] q1, double[] q2, bool expectedIsEqual)
    {
        // Arrange
        Assume.That(q1, Has.Length.EqualTo(8));
        Assume.That(q2, Has.Length.EqualTo(8));

        var quad1 = new Quad(
            new Vector2(q1[0], q1[1]),
            new Vector2(q1[2], q1[3]),
            new Vector2(q1[4], q1[5]),
            new Vector2(q1[6], q1[7])
        );

        var quad2 = new Quad(
            new Vector2(q2[0], q2[1]),
            new Vector2(q2[2], q2[3]),
            new Vector2(q2[4], q2[5]),
            new Vector2(q2[6], q2[7])
        );

        // Act
        // Assert
        AssertEqualityMembers
            .ForValues(quad1, quad2)
            .UsingEqualityOperator((x, y) => x == y)
            .UsingInequalityOperator((x, y) => x != y)
            .EqualityIsExpectedToBe(expectedIsEqual);
    }
}