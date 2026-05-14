using System;
using Geisha.Engine.Physics;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics;

[TestFixture]
public class CollisionBitmaskTests
{
    [Test]
    public void Constructor_Test()
    {
        // Arrange
        const uint value = 123;

        // Act
        var actual = new CollisionBitmask(value);

        // Assert
        Assert.That(actual.Value, Is.EqualTo(value));
    }

    [Test]
    public void FromValue_Test()
    {
        // Arrange
        const uint value = 123;

        // Act
        var actual = CollisionBitmask.FromValue(value);

        // Assert
        Assert.That(actual.Value, Is.EqualTo(value));
    }

    [TestCase(0, 0b_00000000_00000000_00000000_00000001u)]
    [TestCase(1, 0b_00000000_00000000_00000000_00000010u)]
    [TestCase(15, 0b_00000000_00000000_10000000_00000000u)]
    [TestCase(30, 0b_01000000_00000000_00000000_00000000u)]
    [TestCase(31, 0b_10000000_00000000_00000000_00000000u)]
    public void FromBit_Test(int bit, uint expected)
    {
        // Arrange
        // Act
        var actual = CollisionBitmask.FromBit(bit);

        // Assert
        Assert.That(actual.Value, Is.EqualTo(expected));
    }

    [TestCase(new int[0], 0b_00000000_00000000_00000000_00000000u)]
    [TestCase(new[] { 0 }, 0b_00000000_00000000_00000000_00000001u)]
    [TestCase(new[] { 1 }, 0b_00000000_00000000_00000000_00000010u)]
    [TestCase(new[] { 30 }, 0b_01000000_00000000_00000000_00000000u)]
    [TestCase(new[] { 31 }, 0b_10000000_00000000_00000000_00000000u)]
    [TestCase(new[] { 0, 1 }, 0b_00000000_00000000_00000000_00000011u)]
    [TestCase(new[] { 0, 15, 31 }, 0b_10000000_00000000_10000000_00000001u)]
    public void FromBits_Test(int[] bits, uint expected)
    {
        // Arrange
        // Act
        var fromParams = CollisionBitmask.FromBits(bits);
        var fromSpan = CollisionBitmask.FromBits(bits.AsSpan());

        // Assert
        Assert.That(fromParams.Value, Is.EqualTo(expected));
        Assert.That(fromSpan.Value, Is.EqualTo(expected));
    }

    [TestCase(new int[0],          0,  false)]
    [TestCase(new[] { 0 },         0,  true)]
    [TestCase(new[] { 0 },         1,  false)]
    [TestCase(new[] { 31 },        31, true)]
    [TestCase(new[] { 31 },        30, false)]
    [TestCase(new[] { 0, 15, 31 }, 15, true)]
    [TestCase(new[] { 0, 15, 31 }, 7,  false)]
    public void HasBit_Test(int[] bits, int bit, bool expected)
    {
        // Arrange
        var collisionBitmask = CollisionBitmask.FromBits(bits);

        // Act
        var actual = collisionBitmask.HasBit(bit);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase(new int[0],          0,  0b_00000000_00000000_00000000_00000001u)]
    [TestCase(new[] { 0 },         0,  0b_00000000_00000000_00000000_00000001u)] // idempotent
    [TestCase(new[] { 0 },         1,  0b_00000000_00000000_00000000_00000011u)]
    [TestCase(new[] { 31 },        31, 0b_10000000_00000000_00000000_00000000u)] // idempotent
    [TestCase(new[] { 0, 15, 31 }, 15, 0b_10000000_00000000_10000000_00000001u)] // idempotent
    [TestCase(new[] { 0, 15, 31 }, 7,  0b_10000000_00000000_10000000_10000001u)]
    public void WithBit_Test(int[] bits, int bit, uint expected)
    {
        // Arrange
        var collisionBitmask = CollisionBitmask.FromBits(bits);

        // Act
        var actual = collisionBitmask.WithBit(bit);

        // Assert
        Assert.That(actual.Value, Is.EqualTo(expected));
    }

    [TestCase(new int[0],          0,  0b_00000000_00000000_00000000_00000000u)] // no-op
    [TestCase(new[] { 0 },         0,  0b_00000000_00000000_00000000_00000000u)]
    [TestCase(new[] { 0 },         1,  0b_00000000_00000000_00000000_00000001u)] // no-op
    [TestCase(new[] { 31 },        31, 0b_00000000_00000000_00000000_00000000u)]
    [TestCase(new[] { 0, 15, 31 }, 15, 0b_10000000_00000000_00000000_00000001u)]
    [TestCase(new[] { 0, 15, 31 }, 7,  0b_10000000_00000000_10000000_00000001u)] // no-op
    public void WithoutBit_Test(int[] bits, int bit, uint expected)
    {
        // Arrange
        var collisionBitmask = CollisionBitmask.FromBits(bits);

        // Act
        var actual = collisionBitmask.WithoutBit(bit);

        // Assert
        Assert.That(actual.Value, Is.EqualTo(expected));
    }

    [TestCase(0b_00000000_00000000_00000000_00000000u, 0b_00000000_00000000_00000000_00000000u)]
    [TestCase(0b_00000000_00000000_00000000_00000001u, 0b_00000000_00000000_00000000_00000001u)]
    [TestCase(0b_00000000_00000000_00000000_00000001u, 0b_00000000_00000000_00000000_00000010u)]
    [TestCase(0b_00000000_00000000_00000000_00000011u, 0b_00000000_00000000_00000000_00000010u)]
    public void Operator_BitwiseAnd_Test(uint value1, uint value2)
    {
        // Arrange
        var collisionBitmask1 = new CollisionBitmask(value1);
        var collisionBitmask2 = new CollisionBitmask(value2);

        var expected = value1 & value2;

        // Act
        var actual = collisionBitmask1 & collisionBitmask2;

        // Assert
        Assert.That(actual.Value, Is.EqualTo(expected));
    }

    [TestCase(0b_00000000_00000000_00000000_00000000u, 0b_00000000_00000000_00000000_00000000u)]
    [TestCase(0b_00000000_00000000_00000000_00000001u, 0b_00000000_00000000_00000000_00000001u)]
    [TestCase(0b_00000000_00000000_00000000_00000001u, 0b_00000000_00000000_00000000_00000010u)]
    [TestCase(0b_00000000_00000000_00000000_00000011u, 0b_00000000_00000000_00000000_00000010u)]
    public void Operator_BitwiseOr_Test(uint value1, uint value2)
    {
        // Arrange
        var collisionBitmask1 = new CollisionBitmask(value1);
        var collisionBitmask2 = new CollisionBitmask(value2);

        var expected = value1 | value2;

        // Act
        var actual = collisionBitmask1 | collisionBitmask2;

        // Assert
        Assert.That(actual.Value, Is.EqualTo(expected));
    }

    [Test]
    public void ToString_Test()
    {
        // Arrange
        var collisionBitmask = CollisionBitmask.FromBits(0, 2, 4, 5, 8, 29, 31);

        // Act
        var actual = collisionBitmask.ToString();

        // Assert
        Assert.That(actual, Is.EqualTo("CollisionBitmask { Value = 10100000000000000000000100110101 }"));
    }
}