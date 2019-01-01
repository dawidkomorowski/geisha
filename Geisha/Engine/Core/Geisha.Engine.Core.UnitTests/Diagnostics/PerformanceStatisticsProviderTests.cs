using System;
using Geisha.Engine.Core.Diagnostics;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Diagnostics
{
    [TestFixture]
    public class PerformanceStatisticsProviderTests
    {
        private IPerformanceStatisticsStorage _performanceStatisticsStorage;
        private PerformanceStatisticsProvider _performanceStatisticsProvider;

        [SetUp]
        public void SetUp()
        {
            _performanceStatisticsStorage = Substitute.For<IPerformanceStatisticsStorage>();
            _performanceStatisticsProvider = new PerformanceStatisticsProvider(_performanceStatisticsStorage);
        }

        [Test]
        public void TotalFrames_ShouldReturnSameValueAsTotalFramesFromStorage()
        {
            // Arrange
            _performanceStatisticsStorage.TotalFrames.Returns(123);

            // Act
            var actual = _performanceStatisticsProvider.TotalFrames;

            // Assert
            Assert.That(actual, Is.EqualTo(123));
        }

        [Test]
        public void TotalTime_ShouldReturnSameValueAsTotalTimeFromStorage()
        {
            // Arrange
            var time = TimeSpan.FromSeconds(123);
            _performanceStatisticsStorage.TotalTime.Returns(time);

            // Act
            var actual = _performanceStatisticsProvider.TotalTime;

            // Assert
            Assert.That(actual, Is.EqualTo(time));
        }

        [Test]
        public void FrameTime_ShouldReturnValueOfLastFrameTimeFromStorage()
        {
            // Arrange
            var frameTime1 = TimeSpan.FromMilliseconds(8);
            var frameTime2 = TimeSpan.FromMilliseconds(16);
            var frameTime3 = TimeSpan.FromMilliseconds(33);
            _performanceStatisticsStorage.Frames.Returns(new[]
            {
                new Frame(1, frameTime1),
                new Frame(2, frameTime2),
                new Frame(3, frameTime3)
            });

            // Act
            var actual = _performanceStatisticsProvider.FrameTime;

            // Assert
            Assert.That(actual, Is.EqualTo(frameTime3));
        }

        [Test]
        public void Fps_ShouldReturnValueOfOneSecondDividedByFrameTime()
        {
            // Arrange
            var frameTime = TimeSpan.FromMilliseconds(16);
            _performanceStatisticsStorage.Frames.Returns(new[] {new Frame(1, frameTime)});

            // Act
            var actual = _performanceStatisticsProvider.Fps;

            // Assert
            Assert.That(actual, Is.EqualTo(62.5));
        }

        [Test]
        public void AvgFps_ShouldReturnAverageValueOfFpsBasedOnAllFramesFromStorage()
        {
            // Arrange
            var frameTime1 = TimeSpan.FromMilliseconds(8);
            var frameTime2 = TimeSpan.FromMilliseconds(16);
            var frameTime3 = TimeSpan.FromMilliseconds(33);
            _performanceStatisticsStorage.Frames.Returns(new[]
            {
                new Frame(1, frameTime1),
                new Frame(2, frameTime2),
                new Frame(3, frameTime3)
            });

            // Act
            var actual = _performanceStatisticsProvider.AvgFps;

            // Assert
            Assert.That(actual, Is.EqualTo(52.631578).Within(0.000001));
        }
    }
}