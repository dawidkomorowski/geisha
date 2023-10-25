using Geisha.TestUtils;
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

    [Test]
    public void ToString_ShouldReturn_Ulong()
    {
        // Arrange
        var runtimeId = new RuntimeId(42);

        // Act
        var actual = runtimeId.ToString();

        // Assert
        Assert.That(actual, Is.EqualTo("42"));
    }

    [TestCase(1ul, 1ul, 0)]
    [TestCase(1ul, 2ul, -1)]
    [TestCase(2ul, 1ul, 1)]
    public void CompareTo_ShouldCompareRuntimeIdByUlong(ulong ulong1, ulong ulong2, int expected)
    {
        // Arrange
        var runtimeId1 = new RuntimeId(ulong1);
        var runtimeId2 = new RuntimeId(ulong2);

        // Act
        var actual = runtimeId1.CompareTo(runtimeId2);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase(1ul, 1ul, true)]
    [TestCase(1ul, 2ul, false)]
    public void EqualityMembers_ShouldEqualRuntimeId_WhenUlongIsEqual(ulong ulong1, ulong ulong2, bool expectedIsEqual)
    {
        // Arrange
        var runtimeId1 = new RuntimeId(ulong1);
        var runtimeId2 = new RuntimeId(ulong2);

        // Act
        // Assert
        AssertEqualityMembers
            .ForValues(runtimeId1, runtimeId2)
            .UsingEqualityOperator((x, y) => x == y)
            .UsingInequalityOperator((x, y) => x != y)
            .EqualityIsExpectedToBe(expectedIsEqual);
    }
}