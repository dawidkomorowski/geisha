using Geisha.Engine.Core;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading;

namespace Geisha.Engine.UnitTests.Core;

[TestFixture]
public class TimeSystemTests
{
    private static DateTime Now() => new(2023, 6, 15, 14, 30, 12);
    private static Func<DateTime> Now(DateTime now) => () => now;

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(int.MinValue)]
    public void Constructor_ShouldThrowException_WhenFixedUpdatesPerSecondIsNotPositive(int fixedUpdatesPerSecond)
    {
        // Arrange
        var configuration = new CoreConfiguration { FixedUpdatesPerSecond = fixedUpdatesPerSecond };

        // Act & Assert
        Assert.That(() => new TimeSystem(configuration, Now), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Constructor_ShouldInitialize_StartUpTime()
    {
        // Arrange
        var expectedStartUpTime = new DateTime(2026, 4, 3, 17, 28, 37);
        var configuration = new CoreConfiguration();

        // Act
        var timeSystem = new TimeSystem(configuration, Now(expectedStartUpTime));

        // Assert
        Assert.That(timeSystem.StartUpTime, Is.EqualTo(expectedStartUpTime));
    }

    [TestCase(1, 1.0)]
    [TestCase(30, 1.0 / 30.0)]
    [TestCase(60, 1.0 / 60.0)]
    [TestCase(120, 1.0 / 120.0)]
    public void Constructor_ShouldInitialize_FixedDeltaTime_BasedOnFixedUpdatesPerSecond(int fixedUpdatesPerSecond, double expectedSeconds)
    {
        // Arrange
        var configuration = new CoreConfiguration { FixedUpdatesPerSecond = fixedUpdatesPerSecond };
        var expectedFixedDeltaTime = TimeSpan.FromSeconds(expectedSeconds);

        TimeStep.FixedDeltaTime = TimeSpan.Zero; // Reset static property before test.

        // Act
        var timeSystem = new TimeSystem(configuration, Now);

        // Assert
        Assert.That(timeSystem.FixedDeltaTime, Is.EqualTo(expectedFixedDeltaTime));
        Assert.That(TimeStep.FixedDeltaTime, Is.EqualTo(expectedFixedDeltaTime));
    }

    [Test]
    public void Constructor_ShouldInitialize_FramesSinceStartUp_ToDefaultValue()
    {
        // Arrange
        var configuration = new CoreConfiguration();

        // Act
        var timeSystem = new TimeSystem(configuration, Now);

        // Assert
        Assert.That(timeSystem.FramesSinceStartUp, Is.Zero);
    }

    [Test]
    public void Constructor_ShouldInitialize_TimeScale_ToDefaultValue()
    {
        // Arrange
        var configuration = new CoreConfiguration();

        // Act
        var timeSystem = new TimeSystem(configuration, Now);

        // Assert
        Assert.That(timeSystem.TimeScale, Is.EqualTo(1.0));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(5)]
    public void NextTimeStep_ShouldIncrementFramesSinceStartUp(int numberOfCalls)
    {
        // Arrange
        var configuration = new CoreConfiguration();
        var timeSystem = new TimeSystem(configuration);

        // Act
        for (var i = 0; i < numberOfCalls; i++)
        {
            timeSystem.NextTimeStep();
        }

        // Assert
        Assert.That(timeSystem.FramesSinceStartUp, Is.EqualTo(numberOfCalls));
    }

    [TestCase(0.0)]
    [TestCase(0.5)]
    [TestCase(1.0)]
    [TestCase(2.0)]
    public void NextTimeStep_ShouldReturnTimeStepWithCorrectTimeScale(double timeScale)
    {
        // Arrange
        var configuration = new CoreConfiguration();
        var timeSystem = new TimeSystem(configuration) { TimeScale = timeScale };

        // Act
        var timeStep = timeSystem.NextTimeStep();

        // Assert
        Assert.That(timeStep.TimeScale, Is.EqualTo(timeScale));
    }

    [Test]
    public void NextTimeStep_ShouldReturnTimeStepWithZeroDeltaTime_WhenCalledForTheFirstTime()
    {
        // Arrange
        var configuration = new CoreConfiguration();
        var timeSystem = new TimeSystem(configuration);

        // Act
        var timeStep = timeSystem.NextTimeStep();

        // Assert
        Assert.That(timeStep.TimeScale, Is.EqualTo(1.0));
        Assert.That(timeStep.UnscaledDeltaTime, Is.EqualTo(TimeSpan.Zero));
        Assert.That(timeStep.DeltaTime, Is.EqualTo(TimeSpan.Zero));
    }

    [TestCase(16)]
    [TestCase(33)]
    [TestCase(100)]
    public void NextTimeStep_ShouldReturnTimeStepWithDeltaTimeEqualElapsedTimeSinceLastCallToNextTimeStep(int elapsedMilliseconds)
    {
        // Arrange
        var configuration = new CoreConfiguration();
        var timeSystem = new TimeSystem(configuration);
        var stopwatch = new Stopwatch();

        // Act
        stopwatch.Start();

        _ = timeSystem.NextTimeStep();
        Thread.Sleep(elapsedMilliseconds);
        var timeStep = timeSystem.NextTimeStep();

        stopwatch.Stop();

        // Assert
        Assert.That(timeStep.DeltaTime, Is.EqualTo(timeStep.UnscaledDeltaTime));

        Assert.That(timeStep.UnscaledDeltaTime, Is.GreaterThan(TimeSpan.FromMilliseconds(elapsedMilliseconds)));
        Assert.That(timeStep.UnscaledDeltaTime, Is.LessThan(stopwatch.Elapsed));
    }

    [TestCase(0)]
    [TestCase(0.016)] // 16 milliseconds
    [TestCase(1.0)] // 1 second
    [TestCase(60.0)] // 1 minute
    [TestCase(3600.0)] // 1 hour
    public void TimeSinceStartUp_WhenTimeHasPassed_ReturnsCorrectTimeSpan(double secondsElapsed)
    {
        // Arrange
        var startTime = new DateTime(2024, 1, 1, 12, 0, 0);
        var currentTime = startTime;
        var configuration = new CoreConfiguration();
        // ReSharper disable once AccessToModifiedClosure
        var timeSystem = new TimeSystem(configuration, () => currentTime);

        // Act
        currentTime = startTime.AddSeconds(secondsElapsed);

        // Assert
        Assert.That(timeSystem.TimeSinceStartUp, Is.EqualTo(TimeSpan.FromSeconds(secondsElapsed)));
    }
}