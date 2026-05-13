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
    public void FromUInt_Test()
    {
        // Arrange
        const uint value = 123;

        // Act
        var actual = CollisionBitmask.FromUInt(value);

        // Assert
        Assert.That(actual.Value, Is.EqualTo(value));
    }

    // TODO: Add more test cases.
    [TestCase(new int[0], 0b_00000000_00000000_00000000_00000000u)]
    [TestCase(new[] { 0 }, 0b_00000000_00000000_00000000_00000001u)]
    [TestCase(new[] { 1 }, 0b_00000000_00000000_00000000_00000010u)]
    [TestCase(new[] { 30 }, 0b_01000000_00000000_00000000_00000000u)]
    [TestCase(new[] { 31 }, 0b_10000000_00000000_00000000_00000000u)]
    public void FromBits_Test(int[] bits, uint value)
    {
        // Arrange
        // Act
        var fromParams = CollisionBitmask.FromBits(bits);
        var fromSpan = CollisionBitmask.FromBits(bits.AsSpan());

        // Assert
        Assert.That(fromParams.Value, Is.EqualTo(value));
        Assert.That(fromSpan.Value, Is.EqualTo(value));
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