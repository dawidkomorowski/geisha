using System;
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

        [TestCase(0.05, 51)]
        [TestCase(0.1, 101)]
        [TestCase(0.2, 201)]
        public void GetDeltaTime_ShouldReturnAtLeastDeltaTimeButLessThanTwiceDeltaTime_WhenCalledAfterDeltaTimeMillisecondsFromPreviousCall(double expected,
            int sleep)
        {
            // Arrange
            var deltaTimeProvider = new DeltaTimeProvider();

            // Act
            deltaTimeProvider.GetDeltaTime();
            SpinWait(sleep);
            var deltaTime = deltaTimeProvider.GetDeltaTime();

            // Assert
            Assert.That(deltaTime, Is.GreaterThan(expected));
            Assert.That(deltaTime, Is.LessThan(2 * expected));
        }

        private static void SpinWait(int milliseconds)
        {
            var initialTime = DateTime.Now;
            while (DateTime.Now - initialTime < TimeSpan.FromMilliseconds(milliseconds))
            {
                Thread.SpinWait(1000);
            }
        }
    }
}