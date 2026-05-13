using Geisha.Engine.Physics;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics;

[TestFixture]
public class CollisionBitmaskTests
{
    [Test]
    public void FromValue_Test()
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
        var actual = CollisionBitmask.FromBits(bits);

        // Assert
        Assert.That(actual.Value, Is.EqualTo(value));
    }
}