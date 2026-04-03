using System;
using Geisha.Engine.Core;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core;

[TestFixture]
public class TimeStepTests
{
    [TestCase(0, 1, 0)]
    [TestCase(1, 0, 0)]
    [TestCase(1, 0.5, 0.5)]
    [TestCase(1, 1, 1)]
    [TestCase(1, 2.0, 2.0)]
    [TestCase(0.016, 0, 0)]
    [TestCase(0.016, 0.5, 0.008)]
    [TestCase(0.016, 1, 0.016)]
    [TestCase(0.016, 2.0, 0.032)]
    public void Constructor_ShouldInitializeProperties(double unscaledSeconds, double timeScale, double expectedDeltaSeconds)
    {
        // Arrange
        var unscaledDeltaTime = TimeSpan.FromSeconds(unscaledSeconds);
        var expectedDeltaTime = TimeSpan.FromSeconds(expectedDeltaSeconds);

        // Act
        var timeStep = new TimeStep(unscaledDeltaTime, timeScale);

        // Assert
        Assert.That(timeStep.DeltaTime, Is.EqualTo(expectedDeltaTime));
        Assert.That(timeStep.DeltaTimeSeconds, Is.EqualTo(expectedDeltaSeconds));
        Assert.That(timeStep.UnscaledDeltaTime, Is.EqualTo(unscaledDeltaTime));
        Assert.That(timeStep.UnscaledDeltaTimeSeconds, Is.EqualTo(unscaledSeconds));
        Assert.That(timeStep.TimeScale, Is.EqualTo(timeScale));
    }
}