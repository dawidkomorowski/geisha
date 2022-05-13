using System;
using System.Diagnostics;
using System.Threading;
using Geisha.Engine.Core.Diagnostics;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Diagnostics
{
    [TestFixture]
    public class PerformanceStatisticsRecorderTests
    {
        private IPerformanceStatisticsStorage _performanceStatisticsStorage = null!;
        private PerformanceStatisticsRecorder _performanceStatisticsRecorder = null!;

        [SetUp]
        public void SetUp()
        {
            _performanceStatisticsStorage = Substitute.For<IPerformanceStatisticsStorage>();
            _performanceStatisticsRecorder = new PerformanceStatisticsRecorder(_performanceStatisticsStorage);
        }

        [Test]
        public void RecordFrame_ShouldAddFrameToStorageWithZeroFrameTime_WhenCallIsTheFirstOne()
        {
            // Arrange
            // Act
            _performanceStatisticsRecorder.RecordFrame();

            // Assert
            _performanceStatisticsStorage.AddFrame(TimeSpan.Zero);
        }

        [Test]
        public void RecordFrame_ShouldAddFrameToStorageWithNonZeroFrameTime_WhenCallIsNotTheFirstOne()
        {
            // Arrange
            // Act
            _performanceStatisticsRecorder.RecordFrame();
            var stopwatch = Stopwatch.StartNew();
            Thread.Sleep(50);
            _performanceStatisticsRecorder.RecordFrame();

            // Assert
            stopwatch.Stop();
            _performanceStatisticsStorage.AddFrame(Arg.Is<TimeSpan>(ts => ts.TotalMilliseconds >= 50 && ts < stopwatch.Elapsed));
        }

        [Test]
        public void RecordStepDuration_ShouldAddStepFrameTimeToStorage()
        {
            // Arrange
            const string stepName = "Sleep 50 milliseconds system";
            var stopwatch = Stopwatch.StartNew();

            // Act
            using (_performanceStatisticsRecorder.RecordStepDuration(stepName))
            {
                Thread.Sleep(50);
            }

            // Assert
            stopwatch.Stop();
            _performanceStatisticsStorage.Received()
                .AddStepFrameTime(stepName, Arg.Is<TimeSpan>(ts => ts.TotalMilliseconds >= 50 && ts < stopwatch.Elapsed));
        }
    }
}