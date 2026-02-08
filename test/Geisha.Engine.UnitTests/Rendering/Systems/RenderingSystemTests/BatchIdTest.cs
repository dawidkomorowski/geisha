using System;
using Geisha.Engine.Core;
using Geisha.Engine.Rendering.Systems;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Systems.RenderingSystemTests;

[TestFixture]
public class BatchIdTest
{
    [Test]
    public void Constructor_ShouldCreateEmptyBatchId()
    {
        // Arrange & Act
        var batchId = new BatchId();

        // Assert
        Assert.That(batchId.ResourceId, Is.EqualTo(RuntimeId.Invalid));
        Assert.That(batchId.Flags, Is.EqualTo(0));
    }

    [Test]
    public void Constructor_ShouldCreateInitializedBatchId()
    {
        // Arrange
        var resourceId = RuntimeId.Next();
        const byte flags = 42;

        // Act
        var batchId = new BatchId(resourceId, flags);

        // Assert
        Assert.That(batchId.ResourceId, Is.EqualTo(resourceId));
        Assert.That(batchId.Flags, Is.EqualTo(flags));
    }

    [Test]
    public void Empty_ShouldReturnEmptyBatchId()
    {
        // Arrange & Act
        var emptyBatchId = BatchId.Empty;

        // Assert
        Assert.That(emptyBatchId.ResourceId, Is.EqualTo(RuntimeId.Invalid));
        Assert.That(emptyBatchId.Flags, Is.EqualTo(0));
    }

    [TestCase(-1)]
    [TestCase(8)]
    public void WithFlag_ShouldThrowException_GivenInvalidIndex(int index)
    {
        // Arrange
        var batchId = new BatchId(RuntimeId.Next(), 0);

        // Act & Assert
        Assert.That(() => batchId.WithFlag(index, true), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [TestCase(0, false, 0b_0000_0000, 0b_0000_0000)]
    [TestCase(0, true, 0b_0000_0000, 0b_0000_0001)]
    [TestCase(0, false, 0b_0000_0001, 0b_0000_0000)]
    [TestCase(0, true, 0b_0000_0001, 0b_0000_0001)]
    [TestCase(0, false, 0b_0000_0010, 0b_0000_0010)]
    [TestCase(0, true, 0b_0000_0010, 0b_0000_0011)]
    [TestCase(1, false, 0b_0000_0000, 0b_0000_0000)]
    [TestCase(1, true, 0b_0000_0000, 0b_0000_0010)]
    [TestCase(1, false, 0b_0000_0010, 0b_0000_0000)]
    [TestCase(1, true, 0b_0000_0010, 0b_0000_0010)]
    [TestCase(1, false, 0b_0000_0100, 0b_0000_0100)]
    [TestCase(1, true, 0b_0000_0100, 0b_0000_0110)]
    [TestCase(7, false, 0b_0000_0000, 0b_0000_0000)]
    [TestCase(7, true, 0b_0000_0000, 0b_1000_0000)]
    [TestCase(7, false, 0b_1000_0000, 0b_0000_0000)]
    [TestCase(7, true, 0b_1000_0000, 0b_1000_0000)]
    [TestCase(7, false, 0b_0100_0000, 0b_0100_0000)]
    [TestCase(7, true, 0b_0100_0000, 0b_1100_0000)]
    public void WithFlag_ShouldSetProperBitOfFlags(int index, bool value, byte initialFlags, byte expectedFlags)
    {
        // Arrange
        var batchId = new BatchId(RuntimeId.Next(), initialFlags);

        // Act
        var modifiedBatchId = batchId.WithFlag(index, value);

        // Assert
        Assert.That(modifiedBatchId.Flags, Is.EqualTo(expectedFlags));
    }

    [Test]
    public void CompareTo_ShouldReturnZero_GivenEqualBatchIds()
    {
        // Arrange
        var resourceId = RuntimeId.Next();
        const byte flags = 42;
        var batchId1 = new BatchId(resourceId, flags);
        var batchId2 = new BatchId(resourceId, flags);

        // Act
        var comparison = batchId1.CompareTo(batchId2);

        // Assert
        Assert.That(comparison, Is.EqualTo(0));
    }

    [Test]
    public void CompareTo_ShouldCompareByResourceIdFirst()
    {
        // Arrange
        var smallerResourceId = RuntimeId.Next();
        var greaterResourceId = RuntimeId.Next();
        var batchIdWithSmallerResourceId = new BatchId(smallerResourceId, 255);
        var batchIdWithGreaterResourceId = new BatchId(greaterResourceId, 0);

        // Act
        var comparison = batchIdWithSmallerResourceId.CompareTo(batchIdWithGreaterResourceId);

        // Assert
        Assert.That(comparison, Is.LessThan(0));
    }

    [Test]
    public void CompareTo_ShouldCompareByFlags_GivenEqualResourceIds()
    {
        // Arrange
        var resourceId = RuntimeId.Next();
        var batchIdWithSmallerFlags = new BatchId(resourceId, 1);
        var batchIdWithGreaterFlags = new BatchId(resourceId, 2);

        // Act
        var comparison = batchIdWithSmallerFlags.CompareTo(batchIdWithGreaterFlags);

        // Assert
        Assert.That(comparison, Is.LessThan(0));
    }

    [Test]
    public void CompareTo_ShouldBeAntisymmetric()
    {
        // Arrange
        var resourceId = RuntimeId.Next();
        var batchId1 = new BatchId(resourceId, 1);
        var batchId2 = new BatchId(resourceId, 2);

        // Act
        var comparison1 = batchId1.CompareTo(batchId2);
        var comparison2 = batchId2.CompareTo(batchId1);

        // Assert
        Assert.That(comparison1, Is.LessThan(0));
        Assert.That(comparison2, Is.GreaterThan(0));
    }
}