using NUnit.Framework;
using Geisha.Engine.Core;

namespace Geisha.Engine.UnitTests.Core;

[TestFixture]
public class RuntimeIdTests
{
    [Test]
    public void Invalid_ShouldReturnRuntimeId_WhichIsEqualToDefault()
    {
        // Arrange
        var defaultId = new RuntimeId();

        // Act
        var invalid = RuntimeId.Invalid;

        // Assert
        Assert.That(invalid.Id, Is.Zero);
        Assert.That(invalid, Is.EqualTo(defaultId));
        Assert.That(invalid, Is.LessThan(new RuntimeId(1)));
    }

    [Test]
    public void Next_ShouldCreateNextRuntimeId()
    {
        // Arrange
        // Act
        var runtimeId1 = RuntimeId.Next();
        var runtimeId2 = RuntimeId.Next();

        // Assert
        Assert.That(runtimeId1, Is.Not.EqualTo(runtimeId2));
        Assert.That(runtimeId1, Is.LessThan(runtimeId2));
    }

    [TestCase(1ul, 1ul, 0)]
    [TestCase(1ul, 2ul, -1)]
    [TestCase(2ul, 1ul, 1)]
    public void CompareTo_ShouldCompareRuntimeId(ulong id1, ulong id2, int expected)
    {
        // Arrange
        var runtimeId1 = new RuntimeId(id1);
        var runtimeId2 = new RuntimeId(id2);

        // Act
        var actual = runtimeId1.CompareTo(runtimeId2);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }
}