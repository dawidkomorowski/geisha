﻿using System;
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
        public void BeginStepDuration_ThrowsException_WhenCalledTwiceWithoutCallToEndStepDuration()
        {
            // Arrange
            _performanceStatisticsRecorder.BeginStepDuration();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _performanceStatisticsRecorder.BeginStepDuration());
        }

        [Test]
        public void EndStepDuration_ThrowsException_WhenCalledWithoutCallToBeginStepDuration()
        {
            // Arrange
            const string stepName = "Test step";
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _performanceStatisticsRecorder.EndStepDuration(stepName));
        }

        [Test]
        public void EndStepDuration_ThrowsException_WhenCalledTwiceWithoutCallToBeginStepDuration()
        {
            // Arrange
            const string stepName = "Test step";
            _performanceStatisticsRecorder.BeginStepDuration();
            _performanceStatisticsRecorder.EndStepDuration(stepName);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _performanceStatisticsRecorder.EndStepDuration(stepName));
        }

        [Test]
        public void BeginStepDuration_EndStepDuration_CanBeCalledMultipleTimes()
        {
            // Arrange
            const string stepName = "Test step";

            // Act
            _performanceStatisticsRecorder.BeginStepDuration();
            _performanceStatisticsRecorder.EndStepDuration(stepName);
            _performanceStatisticsRecorder.BeginStepDuration();
            _performanceStatisticsRecorder.EndStepDuration(stepName);

            // Assert
            _performanceStatisticsStorage.Received(2).AddStepFrameTime(stepName, Arg.Any<TimeSpan>());
        }

        [Test]
        public void BeginStepDuration_EndStepDuration_ShouldAddStepFrameTimeToStorage()
        {
            // Arrange
            const string stepName = "Sleep 50 milliseconds step";
            var stopwatch = Stopwatch.StartNew();

            // Act
            _performanceStatisticsRecorder.BeginStepDuration();
            Thread.Sleep(50);
            _performanceStatisticsRecorder.EndStepDuration(stepName);

            // Assert
            stopwatch.Stop();
            _performanceStatisticsStorage.Received()
                .AddStepFrameTime(stepName, Arg.Is<TimeSpan>(ts => ts.TotalMilliseconds >= 50 && ts < stopwatch.Elapsed));
        }
    }
}