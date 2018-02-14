using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests
{
    [TestFixture]
    public class DeltaTimeProviderTests
    {
        [Test]
        public void GetDeltaTime_ShouldReturnZero_WhenCalledFirstTime()
        {
            // Arrange
            var deltaTimeProvider = new DeltaTimeProvider();

            // Act
            var deltaTime = deltaTimeProvider.GetDeltaTime();

            // Assert
            Assert.That(deltaTime, Is.Zero);
        }

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void GetDeltaTime_ShouldReturnAtLeastDeltaTimeButLessThanTwiceDeltaTime_WhenCalledAfterDeltaTimeMillisecondsFromPreviousCall(int sleep)
        {
            // Arrange
            var deltaTimeProvider = new DeltaTimeProvider();
            var stopwatch = new Stopwatch();

            var sleepTimeSpan = TimeSpan.FromMilliseconds(sleep);
            var sleepSeconds = sleepTimeSpan.TotalSeconds;

            // Act
            stopwatch.Start();
            deltaTimeProvider.GetDeltaTime();
            Thread.Sleep(sleep);
            var deltaTime = deltaTimeProvider.GetDeltaTime();
            stopwatch.Stop();

            // Assert
            Assert.That(deltaTime, Is.GreaterThan(sleepSeconds));
            Assert.That(deltaTime, Is.LessThan(stopwatch.Elapsed.TotalSeconds));
        }
    }
}