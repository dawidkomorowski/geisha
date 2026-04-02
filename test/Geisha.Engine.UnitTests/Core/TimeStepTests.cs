using System;
using Geisha.Engine.Core;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core;

[TestFixture]
public class TimeStepTests
{
    [TestCase(0, 1)]
    [TestCase(1, 1)]
    [TestCase(1.5, 1)]
    [TestCase(-1.5, 1)]
    [TestCase(0.016, 1)]
    [TestCase(1, 0.5)]
    [TestCase(1, 2.0)]
    [TestCase(1, 0)]
    [TestCase(0.016, 0.5)]
    [TestCase(0.016, -1.0)]
    public void Constructor_ShouldInitializeProperties(double unscaledSeconds, double timeScale)
    {
        // Arrange
        var unscaledDeltaTime = TimeSpan.FromSeconds(unscaledSeconds);

        // Act
        var timeStep = new TimeStep(unscaledDeltaTime, timeScale);

        // Assert
        Assert.That(timeStep.DeltaTime, Is.EqualTo(unscaledDeltaTime * timeScale));
        Assert.That(timeStep.DeltaTimeSeconds, Is.EqualTo(unscaledSeconds * timeScale));
        Assert.That(timeStep.UnscaledDeltaTime, Is.EqualTo(unscaledDeltaTime));
        Assert.That(timeStep.UnscaledDeltaTimeSeconds, Is.EqualTo(unscaledSeconds));
        Assert.That(timeStep.TimeScale, Is.EqualTo(timeScale));
    }
}