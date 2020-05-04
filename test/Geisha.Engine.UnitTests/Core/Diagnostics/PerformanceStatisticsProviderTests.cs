using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Common.TestUtils;
using Geisha.Engine.Core.Diagnostics;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Diagnostics
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
        public void AvgFrameTime_ShouldReturnAverageValueOfFrameTimeBasedOnAllFramesFromStorage()
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
            var actual = _performanceStatisticsProvider.AvgFrameTime;

            // Assert
            Assert.That(actual, Is.EqualTo(TimeSpan.FromMilliseconds(19)));
        }

        // Issue #139
        [Test]
        public void AvgFrameTime_ShouldHavePrecisionOfSingleTick()
        {
            // Arrange
            var frameTime1 = TimeSpan.FromMilliseconds(8);
            var frameTime2 = TimeSpan.FromMilliseconds(16);
            var frameTime3 = TimeSpan.FromMilliseconds(32);
            _performanceStatisticsStorage.Frames.Returns(new[]
            {
                new Frame(1, frameTime1),
                new Frame(2, frameTime2),
                new Frame(3, frameTime3)
            });

            // Act
            var actual = _performanceStatisticsProvider.AvgFrameTime;

            // Assert
            Assert.That(actual.TotalMilliseconds, Is.EqualTo(18.6666).Within(0.0001));
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

        #region GetSystemsExecutionTime

        [Test]
        public void GetSystemsExecutionTime_ShouldReturnEmptyEnumerable_WhenStorageHasNoSystemsFrames()
        {
            // Arrange
            _performanceStatisticsStorage.Frames.Returns(Array.Empty<Frame>());
            _performanceStatisticsStorage.SystemsFrames.Returns(new Dictionary<string, IReadOnlyCollection<Frame>>());

            // Act
            var actual = _performanceStatisticsProvider.GetSystemsExecutionTime();

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GetSystemsExecutionTime_ShouldReturnResultForEachSystemInStorageSystemsFrames()
        {
            var system1 = GetRandomString();
            var system2 = GetRandomString();
            var system3 = GetRandomString();

            // Arrange
            _performanceStatisticsStorage.Frames.Returns(new[]
            {
                new Frame(1, TimeSpan.FromMilliseconds(1)),
                new Frame(2, TimeSpan.FromMilliseconds(1)),
                new Frame(3, TimeSpan.FromMilliseconds(1)),
            });

            _performanceStatisticsStorage.SystemsFrames.Returns(new Dictionary<string, IReadOnlyCollection<Frame>>
            {
                [system1] = new[] {new Frame(1, TimeSpan.Zero)},
                [system2] = new[] {new Frame(1, TimeSpan.Zero)},
                [system3] = new[] {new Frame(1, TimeSpan.Zero)}
            });

            // Act
            var actual = _performanceStatisticsProvider.GetSystemsExecutionTime();

            // Assert
            Assert.That(actual.Count(), Is.EqualTo(3));
            Assert.That(actual.Any(t => t.SystemName == system1), Is.True);
            Assert.That(actual.Any(t => t.SystemName == system2), Is.True);
            Assert.That(actual.Any(t => t.SystemName == system3), Is.True);
        }

        [Test]
        public void GetSystemsExecutionTime_ShouldReturnResultWithCorrectAvgFrameTime()
        {
            var system1 = GetRandomString();
            var system2 = GetRandomString();
            var system3 = GetRandomString();

            // Arrange
            _performanceStatisticsStorage.Frames.Returns(new[]
            {
                new Frame(1, TimeSpan.FromMilliseconds(1)),
                new Frame(2, TimeSpan.FromMilliseconds(1)),
                new Frame(3, TimeSpan.FromMilliseconds(1)),
            });

            _performanceStatisticsStorage.SystemsFrames.Returns(new Dictionary<string, IReadOnlyCollection<Frame>>
            {
                [system1] = new[]
                {
                    new Frame(1, TimeSpan.FromMilliseconds(10)),
                    new Frame(2, TimeSpan.FromMilliseconds(20)),
                    new Frame(3, TimeSpan.FromMilliseconds(30))
                },
                [system2] = new[]
                {
                    new Frame(1, TimeSpan.FromMilliseconds(50)),
                    new Frame(2, TimeSpan.FromMilliseconds(50)),
                    new Frame(3, TimeSpan.FromMilliseconds(200))
                },
                [system3] = new[]
                {
                    new Frame(1, TimeSpan.FromMilliseconds(1.5)),
                    new Frame(2, TimeSpan.FromMilliseconds(2)),
                    new Frame(3, TimeSpan.FromMilliseconds(1))
                },
            });

            // Act
            var actual = _performanceStatisticsProvider.GetSystemsExecutionTime();

            // Assert
            var systemExecutionTime1 = actual.Single(t => t.SystemName == system1);
            var systemExecutionTime2 = actual.Single(t => t.SystemName == system2);
            var systemExecutionTime3 = actual.Single(t => t.SystemName == system3);

            Assert.That(systemExecutionTime1.AvgFrameTime, Is.EqualTo(TimeSpan.FromMilliseconds(20)));
            Assert.That(systemExecutionTime2.AvgFrameTime, Is.EqualTo(TimeSpan.FromMilliseconds(100)).Within(TimeSpan.FromMilliseconds(1)));
            Assert.That(systemExecutionTime3.AvgFrameTime, Is.EqualTo(TimeSpan.FromMilliseconds(1.5)));
        }

        [Test]
        public void GetSystemsExecutionTime_ShouldReturnResultWithCorrectAvgFrameTimeShare()
        {
            var system1 = GetRandomString();
            var system2 = GetRandomString();
            var system3 = GetRandomString();

            // Arrange
            _performanceStatisticsStorage.Frames.Returns(new[]
            {
                new Frame(1, TimeSpan.FromMilliseconds(30)),
                new Frame(2, TimeSpan.FromMilliseconds(70)),
                new Frame(3, TimeSpan.FromMilliseconds(200)),
            });

            _performanceStatisticsStorage.SystemsFrames.Returns(new Dictionary<string, IReadOnlyCollection<Frame>>
            {
                [system1] = new[]
                {
                    new Frame(1, TimeSpan.FromMilliseconds(10)),
                    new Frame(2, TimeSpan.FromMilliseconds(10)),
                    new Frame(3, TimeSpan.FromMilliseconds(10))
                },
                [system2] = new[]
                {
                    new Frame(1, TimeSpan.FromMilliseconds(50)),
                    new Frame(2, TimeSpan.FromMilliseconds(50)),
                    new Frame(3, TimeSpan.FromMilliseconds(50))
                },
                [system3] = new[]
                {
                    new Frame(1, TimeSpan.FromMilliseconds(25)),
                    new Frame(2, TimeSpan.FromMilliseconds(25)),
                    new Frame(3, TimeSpan.FromMilliseconds(25))
                },
            });

            // Act
            var actual = _performanceStatisticsProvider.GetSystemsExecutionTime();

            // Assert
            var systemExecutionTime1 = actual.Single(t => t.SystemName == system1);
            var systemExecutionTime2 = actual.Single(t => t.SystemName == system2);
            var systemExecutionTime3 = actual.Single(t => t.SystemName == system3);

            Assert.That(systemExecutionTime1.AvgFrameTimeShare, Is.EqualTo(0.1).Within(0.000001));
            Assert.That(systemExecutionTime2.AvgFrameTimeShare, Is.EqualTo(0.5));
            Assert.That(systemExecutionTime3.AvgFrameTimeShare, Is.EqualTo(0.25));
        }

        // Issue #139
        [Test]
        public void GetSystemsExecutionTime_ShouldReturnResultWithAvgFrameTimeWithPrecisionOfSingleTick()
        {
            var system = GetRandomString();

            // Arrange
            _performanceStatisticsStorage.Frames.Returns(new[]
            {
                new Frame(1, TimeSpan.FromMilliseconds(1)),
                new Frame(2, TimeSpan.FromMilliseconds(1)),
                new Frame(3, TimeSpan.FromMilliseconds(1)),
            });

            _performanceStatisticsStorage.SystemsFrames.Returns(new Dictionary<string, IReadOnlyCollection<Frame>>
            {
                [system] = new[]
                {
                    new Frame(1, TimeSpan.FromMilliseconds(8)),
                    new Frame(2, TimeSpan.FromMilliseconds(16)),
                    new Frame(3, TimeSpan.FromMilliseconds(32))
                }
            });

            // Act
            var actual = _performanceStatisticsProvider.GetSystemsExecutionTime();

            // Assert
            var systemExecutionTime1 = actual.Single(t => t.SystemName == system);

            Assert.That(systemExecutionTime1.AvgFrameTime.TotalMilliseconds, Is.EqualTo(18.6666).Within(0.0001));
        }

        #endregion

        private static string GetRandomString()
        {
            return Utils.Random.GetString();
        }
    }
}