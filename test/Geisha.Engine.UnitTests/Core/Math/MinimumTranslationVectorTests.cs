using Geisha.Engine.Core.Math;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math;

[TestFixture]
public class MinimumTranslationVectorTests
{
    [Test]
    public void Constructor_ShouldCreateNewInstance_GivenDirectionAndLength()
    {
        // Arrange
        var direction = new Vector2(0.4472135954999579, 0.8944271909999159);
        var length = 3;

        // Act
        var mtv = new MinimumTranslationVector(direction, length);

        // Assert
        Assert.That(mtv.Direction, Is.EqualTo(direction));
        Assert.That(mtv.Length, Is.EqualTo(length));
    }

    [Test]
    public void Constructor_ShouldCreateNewInstance_GivenVectorRepresentingMTV()
    {
        // Arrange
        var mtvVector = new Vector2(1, 2);

        // Act
        var mtv = new MinimumTranslationVector(mtvVector);

        // Assert
        Assert.That(mtv.Direction, Is.EqualTo(mtvVector.Unit));
        Assert.That(mtv.Length, Is.EqualTo(mtvVector.Length));
    }

    [Test]
    [SetCulture("")]
    public void ToString_Test()
    {
        // Arrange
        var mtv = new MinimumTranslationVector(new Vector2(1, 2));

        // Act
        var actual = mtv.ToString();

        // Assert
        Assert.That(actual, Is.EqualTo("Direction: X: 0.4472135954999579, Y: 0.8944271909999159, Length: 2.23606797749979"));
    }

    [TestCase( /*1*/0.4472135954999579, 0.8944271909999159, 3, /*2*/0.4472135954999579, 0.8944271909999159, 3, true)]
    [TestCase( /*1*/0.4472135954999579, 0.8944271909999159, 3, /*2*/0.4472135954999579, 0.8944271909999159, 4, false)]
    [TestCase( /*1*/0.4472135954999579, 0.8944271909999159, 3, /*2*/0.8944271909999159, 0.4472135954999579, 3, false)]
    public void EqualityMembers_Test(double mtv1X, double mtv1Y, double mtv1L, double mtv2X, double mtv2Y, double mtv2L, bool expectedEqual)
    {
        // Arrange
        var mtv1 = new MinimumTranslationVector(new Vector2(mtv1X, mtv1Y), mtv1L);
        var mtv2 = new MinimumTranslationVector(new Vector2(mtv2X, mtv2Y), mtv2L);

        // Act
        // Assert
        AssertEqualityMembers
            .ForValues(mtv1, mtv2)
            .UsingEqualityOperator((x, y) => x == y)
            .UsingInequalityOperator((x, y) => x != y)
            .EqualityIsExpectedToBe(expectedEqual);
    }
}