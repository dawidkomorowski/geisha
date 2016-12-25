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

        [TestCase(0.001, 1)]
        [TestCase(0.01, 10)]
        [TestCase(0.1, 100)]
        public void
            GetDeltaTime_ShouldReturnAtLeastDeltaTimeButLessThanTwiceDeltaTime_WhenCalledAfterDeltaTimeMillisecondsFromPreviousCall
            (
                double expected, int sleep)
        {
            // Arrange
            var deltaTimeProvider = new DeltaTimeProvider();

            // Act
            deltaTimeProvider.GetDeltaTime();
            Thread.Sleep(sleep);
            var deltaTime = deltaTimeProvider.GetDeltaTime();

            // Assert
            Assert.That(deltaTime, Is.GreaterThan(expected));
            Assert.That(deltaTime, Is.LessThan(2*expected));
        }
    }
}