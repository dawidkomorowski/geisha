using System;
using System.Linq;
using Geisha.Engine.Core.Diagnostics;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Diagnostics
{
    [TestFixture]
    public class PerformanceStatisticsStorageTests
    {
        private PerformanceStatisticsStorage _performanceStatisticsStorage;

        [SetUp]
        public void SetUp()
        {
            _performanceStatisticsStorage = new PerformanceStatisticsStorage();
        }

        [Test]
        public void Constructor_ShouldCreateStorageWithTotalFramesEqualZero()
        {
            // Arrange
            // Act
            var storage = new PerformanceStatisticsStorage();

            // Assert
            Assert.That(storage.TotalFrames, Is.Zero);
        }

        [Test]
        public void Constructor_ShouldCreateStorageWithFramesListContaining100FrameTimesAllEqualZero()
        {
            // Arrange
            // Act
            var storage = new PerformanceStatisticsStorage();

            // Assert
            Assert.That(storage.Frames, Has.Count.EqualTo(100));
            Assert.That(storage.Frames, Is.EqualTo(Enumerable.Range(0, 100).Select(i => TimeSpan.Zero)));
        }
    }
}