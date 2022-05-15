using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.Diagnostics;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Diagnostics
{
    [TestFixture]
    public class PerformanceStatisticsProviderTests
    {
        private IPerformanceStatisticsStorage _performanceStatisticsStorage = null!;
        private PerformanceStatisticsProvider _performanceStatisticsProvider = null!;

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

        [Test]
        [Description("Issue #139")]
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
            _performanceStatisticsStorage.Frames.Returns(new[] { new Frame(1, frameTime) });

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

        #region GetGameLoopStatistics

        [Test]
        public void GetGameLoopStatistics_ShouldReturnEmptyEnumerable_WhenStorageHasNoStepsFrames()
        {
            // Arrange
            _performanceStatisticsStorage.Frames.Returns(Array.Empty<Frame>());
            _performanceStatisticsStorage.StepsFrames.Returns(new Dictionary<string, IReadOnlyCollection<Frame>>());

            // Act
            var actual = _performanceStatisticsProvider.GetGameLoopStatistics();

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GetGameLoopStatistics_ShouldReturnResultForEachStepInStorageStepsFrames()
        {
            var step1 = GetRandomString();
            var step2 = GetRandomString();
            var step3 = GetRandomString();

            // Arrange
            _performanceStatisticsStorage.Frames.Returns(new[]
            {
                new Frame(1, TimeSpan.FromMilliseconds(1)),
                new Frame(2, TimeSpan.FromMilliseconds(1)),
                new Frame(3, TimeSpan.FromMilliseconds(1)),
            });

            _performanceStatisticsStorage.StepsFrames.Returns(new Dictionary<string, IReadOnlyCollection<Frame>>
            {
                [step1] = new[] { new Frame(1, TimeSpan.Zero) },
                [step2] = new[] { new Frame(1, TimeSpan.Zero) },
                [step3] = new[] { new Frame(1, TimeSpan.Zero) }
            });

            // Act
            var actual = _performanceStatisticsProvider.GetGameLoopStatistics().ToArray();

            // Assert
            Assert.That(actual.Length, Is.EqualTo(3));
            Assert.That(actual.Any(t => t.StepName == step1), Is.True);
            Assert.That(actual.Any(t => t.StepName == step2), Is.True);
            Assert.That(actual.Any(t => t.StepName == step3), Is.True);
        }

        [Test]
        public void GetGameLoopStatistics_ShouldReturnResultWithCorrectAvgFrameTime()
        {
            var step1 = GetRandomString();
            var step2 = GetRandomString();
            var step3 = GetRandomString();

            // Arrange
            _performanceStatisticsStorage.Frames.Returns(new[]
            {
                new Frame(1, TimeSpan.FromMilliseconds(1)),
                new Frame(2, TimeSpan.FromMilliseconds(1)),
                new Frame(3, TimeSpan.FromMilliseconds(1)),
            });

            _performanceStatisticsStorage.StepsFrames.Returns(new Dictionary<string, IReadOnlyCollection<Frame>>
            {
                [step1] = new[]
                {
                    new Frame(1, TimeSpan.FromMilliseconds(10)),
                    new Frame(2, TimeSpan.FromMilliseconds(20)),
                    new Frame(3, TimeSpan.FromMilliseconds(30))
                },
                [step2] = new[]
                {
                    new Frame(1, TimeSpan.FromMilliseconds(50)),
                    new Frame(2, TimeSpan.FromMilliseconds(50)),
                    new Frame(3, TimeSpan.FromMilliseconds(200))
                },
                [step3] = new[]
                {
                    new Frame(1, TimeSpan.FromMilliseconds(1.5)),
                    new Frame(2, TimeSpan.FromMilliseconds(2)),
                    new Frame(3, TimeSpan.FromMilliseconds(1))
                },
            });

            // Act
            var actual = _performanceStatisticsProvider.GetGameLoopStatistics().ToArray();

            // Assert
            var stepStatistics1 = actual.Single(t => t.StepName == step1);
            var stepStatistics2 = actual.Single(t => t.StepName == step2);
            var stepStatistics3 = actual.Single(t => t.StepName == step3);

            Assert.That(stepStatistics1.AvgFrameTime, Is.EqualTo(TimeSpan.FromMilliseconds(20)));
            Assert.That(stepStatistics2.AvgFrameTime, Is.EqualTo(TimeSpan.FromMilliseconds(100)).Within(TimeSpan.FromMilliseconds(1)));
            Assert.That(stepStatistics3.AvgFrameTime, Is.EqualTo(TimeSpan.FromMilliseconds(1.5)));
        }

        [Test]
        public void GetGameLoopStatistics_ShouldReturnResultWithCorrectAvgFrameTimeShare()
        {
            var step1 = GetRandomString();
            var step2 = GetRandomString();
            var step3 = GetRandomString();

            // Arrange
            _performanceStatisticsStorage.Frames.Returns(new[]
            {
                new Frame(1, TimeSpan.FromMilliseconds(30)),
                new Frame(2, TimeSpan.FromMilliseconds(70)),
                new Frame(3, TimeSpan.FromMilliseconds(200)),
            });

            _performanceStatisticsStorage.StepsFrames.Returns(new Dictionary<string, IReadOnlyCollection<Frame>>
            {
                [step1] = new[]
                {
                    new Frame(1, TimeSpan.FromMilliseconds(10)),
                    new Frame(2, TimeSpan.FromMilliseconds(10)),
                    new Frame(3, TimeSpan.FromMilliseconds(10))
                },
                [step2] = new[]
                {
                    new Frame(1, TimeSpan.FromMilliseconds(50)),
                    new Frame(2, TimeSpan.FromMilliseconds(50)),
                    new Frame(3, TimeSpan.FromMilliseconds(50))
                },
                [step3] = new[]
                {
                    new Frame(1, TimeSpan.FromMilliseconds(25)),
                    new Frame(2, TimeSpan.FromMilliseconds(25)),
                    new Frame(3, TimeSpan.FromMilliseconds(25))
                },
            });

            // Act
            var actual = _performanceStatisticsProvider.GetGameLoopStatistics().ToArray();

            // Assert
            var stepStatistics1 = actual.Single(t => t.StepName == step1);
            var stepStatistics2 = actual.Single(t => t.StepName == step2);
            var stepStatistics3 = actual.Single(t => t.StepName == step3);

            Assert.That(stepStatistics1.AvgFrameTimeShare, Is.EqualTo(0.1).Within(0.000001));
            Assert.That(stepStatistics2.AvgFrameTimeShare, Is.EqualTo(0.5));
            Assert.That(stepStatistics3.AvgFrameTimeShare, Is.EqualTo(0.25));
        }

        [Test]
        [Description("Issue #139")]
        public void GetGameLoopStatistics_ShouldReturnResultWithAvgFrameTimeWithPrecisionOfSingleTick()
        {
            var step = GetRandomString();

            // Arrange
            _performanceStatisticsStorage.Frames.Returns(new[]
            {
                new Frame(1, TimeSpan.FromMilliseconds(1)),
                new Frame(2, TimeSpan.FromMilliseconds(1)),
                new Frame(3, TimeSpan.FromMilliseconds(1)),
            });

            _performanceStatisticsStorage.StepsFrames.Returns(new Dictionary<string, IReadOnlyCollection<Frame>>
            {
                [step] = new[]
                {
                    new Frame(1, TimeSpan.FromMilliseconds(8)),
                    new Frame(2, TimeSpan.FromMilliseconds(16)),
                    new Frame(3, TimeSpan.FromMilliseconds(32))
                }
            });

            // Act
            var actual = _performanceStatisticsProvider.GetGameLoopStatistics();

            // Assert
            var stepStatistics = actual.Single(t => t.StepName == step);

            Assert.That(stepStatistics.AvgFrameTime.TotalMilliseconds, Is.EqualTo(18.6666).Within(0.0001));
        }

        #endregion

        private static string GetRandomString()
        {
            return Utils.Random.GetString();
        }
    }
}