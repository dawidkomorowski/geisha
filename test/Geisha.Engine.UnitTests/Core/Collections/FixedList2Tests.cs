using Geisha.Engine.Core.Collections;
using NUnit.Framework;
using System;

namespace Geisha.Engine.UnitTests.Core.Collections;

[TestFixture]
public class FixedList2Tests
{
    [Test]
    public void DefaultValue_ShouldBeListWithZeroItems()
    {
        // Arrange
        // Act
        FixedList2<int> list = default;

        // Assert
        Assert.That(list, Has.Count.Zero);
        Assert.That(() => _ = list[-1], Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => list[-1] = -1, Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => _ = list[0], Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => list[0] = -1, Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void DefaultConstructor_ShouldCreateListWithZeroItems()
    {
        // Arrange
        // Act
        var list = new FixedList2<int>();

        // Assert
        Assert.That(list, Has.Count.Zero);
        Assert.That(() => _ = list[-1], Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => list[-1] = -1, Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => _ = list[0], Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => list[0] = -1, Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Add_ShouldAddOneItemToTheList()
    {
        // Arrange
        const int item0 = 123;
        var list = new FixedList2<int>();

        // Act
        list.Add(item0);

        // Assert
        Assert.That(list, Has.Count.EqualTo(1));
        Assert.That(() => _ = list[-1], Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => list[-1] = -1, Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(list[0], Is.EqualTo(item0));
        Assert.That(() => _ = list[1], Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => list[1] = -1, Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Add_ShouldAddSecondItemToTheList_WhenOneItemIsAlreadyInTheList()
    {
        // Arrange
        const int item0 = 123;
        const int item1 = 456;
        var list = new FixedList2<int>();
        list.Add(item0);

        // Act
        list.Add(item1);

        // Assert
        Assert.That(list, Has.Count.EqualTo(2));
        Assert.That(() => _ = list[-1], Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => list[-1] = -1, Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(list[0], Is.EqualTo(item0));
        Assert.That(list[1], Is.EqualTo(item1));
        Assert.That(() => _ = list[2], Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => list[2] = -1, Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Add_ShouldThrowException_WhenListIsAlreadyFull()
    {
        // Arrange
        const int item0 = 123;
        const int item1 = 456;
        const int item2 = 789;
        var list = new FixedList2<int>();
        list.Add(item0);
        list.Add(item1);

        // Act
        // Assert
        Assert.That(() => list.Add(item2), Throws.InvalidOperationException);
    }

    [Test]
    public void Indexer_ShouldGetAndSetItemsByIndex()
    {
        // Arrange
        const int item0 = 123;
        const int item1 = 456;
        const int item2 = 78;
        const int item3 = 90;
        var list = new FixedList2<int>();
        list.Add(item0);
        list.Add(item1);

        // Assume
        Assert.That(list, Has.Count.EqualTo(2));

        // Act
        var actual0 = list[0];
        var actual1 = list[1];
        list[0] = item2;
        list[1] = item3;

        // Assert
        Assert.That(actual0, Is.EqualTo(item0));
        Assert.That(actual1, Is.EqualTo(item1));
        Assert.That(list[0], Is.EqualTo(item2));
        Assert.That(list[1], Is.EqualTo(item3));
    }

    [Test]
    public void ToReadOnly_ShouldCreateReadOnlyFixedList_WhenListIsEmpty()
    {
        // Arrange
        var list = new FixedList2<int>();

        // Act
        var actual = list.ToReadOnly();

        // Assert
        Assert.That(actual, Has.Count.Zero);
    }

    [Test]
    public void ToReadOnly_ShouldCreateReadOnlyFixedList_WhenListHasOneItem()
    {
        // Arrange
        const int item0 = 123;
        var list = new FixedList2<int>();
        list.Add(item0);

        // Act
        var actual = list.ToReadOnly();

        // Assert
        Assert.That(actual, Has.Count.EqualTo(1));
        Assert.That(actual[0], Is.EqualTo(item0));
    }

    [Test]
    public void ToReadOnly_ShouldCreateReadOnlyFixedList_WhenListHasTwoItems()
    {
        // Arrange
        const int item0 = 123;
        const int item1 = 456;
        var list = new FixedList2<int>();
        list.Add(item0);
        list.Add(item1);

        // Act
        var actual = list.ToReadOnly();

        // Assert
        Assert.That(actual, Has.Count.EqualTo(2));
        Assert.That(actual[0], Is.EqualTo(item0));
        Assert.That(actual[1], Is.EqualTo(item1));
    }
}