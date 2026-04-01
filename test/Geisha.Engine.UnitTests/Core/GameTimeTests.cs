using System;
using Geisha.Engine.Core;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core;

[TestFixture]
public class GameTimeTests
{
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(1.5)]
    [TestCase(-1.5)]
    [TestCase(0.016)]
    public void FixedDeltaTimeSeconds_ShouldReturnFixedDeltaTimeInSeconds(double seconds)
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(seconds);

        // Act
        // Assert
        Assert.That(GameTime.FixedDeltaTimeSeconds, Is.EqualTo(seconds));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(1.5)]
    [TestCase(-1.5)]
    [TestCase(0.016)]
    public void DeltaTimeSeconds_ShouldReturnDeltaTimeInSeconds(double seconds)
    {
        // Arrange
        var gameTime = new GameTime(TimeSpan.FromSeconds(seconds));

        // Act
        // Assert
        Assert.That(gameTime.DeltaTimeSeconds, Is.EqualTo(seconds));
    }

    [Test]
    public void TimeSinceStartUp_ShouldReturnDifferenceBetweenCurrentTimeAndStartUpTime()
    {
        // Arrange
        var currentTime = DateTime.Now;
        var startUpTime = currentTime.AddHours(-1);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Now().Returns(currentTime);

        GameTime.StartUpTime = startUpTime;
        GameTime.DateTimeProvider = dateTimeProvider;

        // Act
        var actual = GameTime.TimeSinceStartUp;

        // Assert
        Assert.That(actual, Is.EqualTo(currentTime - startUpTime));
    }
}