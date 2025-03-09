using System;
using Geisha.Engine.Core.Collections;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Collections;

[TestFixture]
public class ReadOnlyFixedList2Tests
{
    [Test]
    public void DefaultValue_ShouldBeListWithZeroItems()
    {
        // Arrange
        // Act
        ReadOnlyFixedList2<int> list = default;

        // Assert
        Assert.That(list, Has.Count.Zero);
        Assert.That(() => _ = list[-1], Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => _ = list[0], Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void DefaultConstructor_ShouldCreateListWithZeroItems()
    {
        // Arrange
        // Act
        var list = new ReadOnlyFixedList2<int>();

        // Assert
        Assert.That(list, Has.Count.Zero);
        Assert.That(() => _ = list[-1], Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => _ = list[0], Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void ConstructorWithOneParameter_ShouldCreateListWithOneItem()
    {
        // Arrange
        const int item0 = 123;

        // Act
        var list = new ReadOnlyFixedList2<int>(item0);

        // Assert
        Assert.That(list, Has.Count.EqualTo(1));
        Assert.That(() => _ = list[-1], Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(list[0], Is.EqualTo(item0));
        Assert.That(() => _ = list[1], Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void ConstructorWithTwoParameters_ShouldCreateListWithTwoItems()
    {
        // Arrange
        const int item0 = 123;
        const int item1 = 456;

        // Act
        var list = new ReadOnlyFixedList2<int>(item0, item1);

        // Assert
        Assert.That(list, Has.Count.EqualTo(2));
        Assert.That(() => _ = list[-1], Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(list[0], Is.EqualTo(item0));
        Assert.That(list[1], Is.EqualTo(item1));
        Assert.That(() => _ = list[2], Throws.TypeOf<ArgumentOutOfRangeException>());
    }
}