using System;
using System.Linq;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.GameLoop;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Diagnostics
{
    [TestFixture]
    public class PerformanceStatisticsStorageTests
    {
        private const string StepName1 = nameof(StepName1);
        private const string StepName2 = nameof(StepName2);
        private const string StepName3 = nameof(StepName3);
        private IGameLoopSteps _gameLoopSteps = null!;

        [SetUp]
        public void SetUp()
        {
            _gameLoopSteps = Substitute.For<IGameLoopSteps>();
        }

        #region Constructor

        [Test]
        public void Constructor_ShouldCreateStorageWithTotalFramesEqualZero()
        {
            // Arrange
            // Act
            var storage = new PerformanceStatisticsStorage(_gameLoopSteps);

            // Assert
            Assert.That(storage.TotalFrames, Is.Zero);
        }

        [Test]
        public void Constructor_ShouldCreateStorageWithTotalTimeEqualZero()
        {
            // Arrange
            // Act
            var storage = new PerformanceStatisticsStorage(_gameLoopSteps);

            // Assert
            Assert.That(storage.TotalTime, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void Constructor_ShouldCreateStorageWithFramesListContaining100FrameTimesAllEqualZero()
        {
            // Arrange
            // Act
            var storage = new PerformanceStatisticsStorage(_gameLoopSteps);

            // Assert
            Assert.That(storage.Frames.Count, Is.EqualTo(100));
            Assert.That(storage.Frames.Select(f => f.Number), Is.EqualTo(Enumerable.Range(0, 100).Select(i => 0)));
            Assert.That(storage.Frames.Select(f => f.Time), Is.EqualTo(Enumerable.Range(0, 100).Select(i => TimeSpan.Zero)));
        }

        [Test]
        public void
            Constructor_ShouldCreateStorageWithStepsFramesFor3StepsWithFramesListContaining100FrameTimesAllEqualZero_When3StepsAvailableInGameLoopSteps()
        {
            // Arrange
            _gameLoopSteps.StepsNames.Returns(new[] { StepName1, StepName2, StepName3 });

            // Act
            var storage = new PerformanceStatisticsStorage(_gameLoopSteps);

            // Assert
            Assert.That(storage.StepsFrames.Keys, Is.EquivalentTo(new[] { StepName1, StepName2, StepName3 }));

            Assert.That(storage.StepsFrames[StepName1].Select(f => f.Number), Is.EqualTo(Enumerable.Range(0, 100).Select(i => 0)));
            Assert.That(storage.StepsFrames[StepName1].Select(f => f.Time), Is.EqualTo(Enumerable.Range(0, 100).Select(i => TimeSpan.Zero)));

            Assert.That(storage.StepsFrames[StepName2].Select(f => f.Number), Is.EqualTo(Enumerable.Range(0, 100).Select(i => 0)));
            Assert.That(storage.StepsFrames[StepName2].Select(f => f.Time), Is.EqualTo(Enumerable.Range(0, 100).Select(i => TimeSpan.Zero)));

            Assert.That(storage.StepsFrames[StepName3].Select(f => f.Number), Is.EqualTo(Enumerable.Range(0, 100).Select(i => 0)));
            Assert.That(storage.StepsFrames[StepName3].Select(f => f.Time), Is.EqualTo(Enumerable.Range(0, 100).Select(i => TimeSpan.Zero)));
        }

        #endregion

        #region Frames

        [Test]
        public void AddFrame_ShouldIncrementTotalFramesByOne()
        {
            // Arrange
            var storage = GetStorage();

            // Assume
            Assume.That(storage.TotalFrames, Is.Zero);

            // Act
            storage.AddFrame(TimeSpan.Zero);

            // Assert
            Assert.That(storage.TotalFrames, Is.EqualTo(1));
        }

        [Test]
        public void AddFrame_ShouldIncreaseTotalTimeByAmountOfFrameTime()
        {
            // Arrange
            var storage = GetStorage();
            var frameTime = TimeSpan.FromMilliseconds(16);

            // Assume
            Assume.That(storage.TotalTime, Is.EqualTo(TimeSpan.Zero));

            // Act
            storage.AddFrame(frameTime);

            // Assert
            Assert.That(storage.TotalTime, Is.EqualTo(frameTime));
        }

        [Test]
        public void AddFrame_ShouldIncreaseTotalTimeByAmountOfAllAddedFramesTimes()
        {
            // Arrange
            var storage = GetStorage();
            var frameTime1 = TimeSpan.FromMilliseconds(8);
            var frameTime2 = TimeSpan.FromMilliseconds(16);
            var frameTime3 = TimeSpan.FromMilliseconds(33);
            var expected = frameTime1 + frameTime2 + frameTime3;

            // Assume
            Assume.That(storage.TotalTime, Is.EqualTo(TimeSpan.Zero));

            // Act
            storage.AddFrame(frameTime1);
            storage.AddFrame(frameTime2);
            storage.AddFrame(frameTime3);

            // Assert
            Assert.That(storage.TotalTime, Is.EqualTo(expected));
        }

        [Test]
        public void AddFrame_ShouldAddFrameToFramesWithCorrectFrameNumberAndFrameTime()
        {
            // Arrange
            var storage = GetStorage();
            var frameTime = TimeSpan.FromMilliseconds(33);

            // Act
            storage.AddFrame(frameTime);

            // Assert
            Assert.That(storage.Frames.Last().Number, Is.EqualTo(1));
            Assert.That(storage.Frames.Last().Time, Is.EqualTo(frameTime));
        }

        [Test]
        public void AddFrame_ShouldAddSeveralFramesToFramesWithCorrectFrameNumberAndFrameTime()
        {
            // Arrange
            var storage = GetStorage();
            var frameTime1 = TimeSpan.FromMilliseconds(33);
            var frameTime2 = TimeSpan.FromMilliseconds(16);
            var frameTime3 = TimeSpan.FromMilliseconds(8);

            // Act
            storage.AddFrame(frameTime1);
            storage.AddFrame(frameTime2);
            storage.AddFrame(frameTime3);

            // Assert
            var lastThreeFrames = storage.Frames.Skip(97).ToArray();
            Assert.That(lastThreeFrames.First().Number, Is.EqualTo(1));
            Assert.That(lastThreeFrames.First().Time, Is.EqualTo(frameTime1));
            Assert.That(lastThreeFrames.Skip(1).First().Number, Is.EqualTo(2));
            Assert.That(lastThreeFrames.Skip(1).First().Time, Is.EqualTo(frameTime2));
            Assert.That(lastThreeFrames.Skip(2).First().Number, Is.EqualTo(3));
            Assert.That(lastThreeFrames.Skip(2).First().Time, Is.EqualTo(frameTime3));
        }

        [Test]
        public void AddFrame_ShouldAddNewFrameWithCorrectFrameNumberAndFrameTimeReplacingTheOldestFrame()
        {
            // Arrange
            var storage = GetStorage();
            var frameTimeOfFirst = TimeSpan.FromMilliseconds(33);
            var frameTimeOfSecond = TimeSpan.FromMilliseconds(16);
            var frameTimeOfLast = TimeSpan.FromMilliseconds(8);
            var frameTimeOfNew = TimeSpan.FromMilliseconds(4);

            storage.AddFrame(frameTimeOfFirst);
            storage.AddFrame(frameTimeOfSecond);

            for (var i = 0; i < 97; i++)
            {
                storage.AddFrame(TimeSpan.FromMilliseconds(1));
            }

            storage.AddFrame(frameTimeOfLast);

            // Assume
            Assume.That(storage.Frames.First().Number, Is.EqualTo(1));
            Assume.That(storage.Frames.First().Time, Is.EqualTo(frameTimeOfFirst));
            Assert.That(storage.Frames.Last().Number, Is.EqualTo(100));
            Assert.That(storage.Frames.Last().Time, Is.EqualTo(frameTimeOfLast));

            // Act
            storage.AddFrame(frameTimeOfNew);

            // Assert
            Assume.That(storage.Frames.First().Number, Is.EqualTo(2));
            Assume.That(storage.Frames.First().Time, Is.EqualTo(frameTimeOfSecond));
            Assert.That(storage.Frames.Last().Number, Is.EqualTo(101));
            Assert.That(storage.Frames.Last().Time, Is.EqualTo(frameTimeOfNew));
        }

        #endregion

        #region StepsFrames

        [Test]
        public void AddStepFrameTime_ShouldNotAddStepFrame_WhenAddFrameWasNotCalledAfterAddSStepFrameTime()
        {
            // Arrange
            _gameLoopSteps.StepsNames.Returns(new[] { StepName1 });
            var storage = GetStorage();
            var stepFrameTime = TimeSpan.FromMilliseconds(33);

            // Act
            storage.AddStepFrameTime(StepName1, stepFrameTime);

            // Assert
            Assert.That(storage.StepsFrames[StepName1].Last().Number, Is.Zero);
            Assert.That(storage.StepsFrames[StepName1].Last().Time, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void AddFrame_ShouldAddStepFrameWithIncrementedNumberButZeroTime_WhenAddFrameWasCalledButAddStepFrameTimeWasNotCalledBefore()
        {
            // Arrange
            _gameLoopSteps.StepsNames.Returns(new[] { StepName1 });
            var storage = GetStorage();
            var frameTime = TimeSpan.FromMilliseconds(50);

            // Act
            storage.AddFrame(frameTime);

            // Assert
            Assert.That(storage.StepsFrames[StepName1].Last().Number, Is.EqualTo(1));
            Assert.That(storage.StepsFrames[StepName1].Last().Time, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void AddFrame_ShouldAddSeveralStepFramesWithIncrementedNumbersButZeroTime_WhenAddFrameWasCalledMultipleTimesButAddStepFrameTimeWasNeverCalled()
        {
            // Arrange
            _gameLoopSteps.StepsNames.Returns(new[] { StepName1 });
            var storage = GetStorage();
            var frameTime = TimeSpan.FromMilliseconds(50);

            // Act
            storage.AddFrame(frameTime);
            storage.AddFrame(frameTime);
            storage.AddFrame(frameTime);

            // Assert
            Assert.That(storage.StepsFrames[StepName1].Skip(97).First().Number, Is.EqualTo(1));
            Assert.That(storage.StepsFrames[StepName1].Skip(97).First().Time, Is.EqualTo(TimeSpan.Zero));
            Assert.That(storage.StepsFrames[StepName1].Skip(98).First().Number, Is.EqualTo(2));
            Assert.That(storage.StepsFrames[StepName1].Skip(98).First().Time, Is.EqualTo(TimeSpan.Zero));
            Assert.That(storage.StepsFrames[StepName1].Skip(99).First().Number, Is.EqualTo(3));
            Assert.That(storage.StepsFrames[StepName1].Skip(99).First().Time, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void AddFrame_ShouldAddStepFrame_WhenAddStepFrameTimeWasCalledBefore()
        {
            // Arrange
            _gameLoopSteps.StepsNames.Returns(new[] { StepName1 });
            var storage = GetStorage();
            var systemFrameTime = TimeSpan.FromMilliseconds(33);
            var frameTime = TimeSpan.FromMilliseconds(50);

            // Act
            storage.AddStepFrameTime(StepName1, systemFrameTime);
            storage.AddFrame(frameTime);

            // Assert
            Assert.That(storage.StepsFrames[StepName1].Last().Number, Is.EqualTo(1));
            Assert.That(storage.StepsFrames[StepName1].Last().Time, Is.EqualTo(systemFrameTime));
        }

        [Test]
        public void AddFrame_ShouldAddSingleStepFrameWithTimeBeingSumOfAllStepFrameTimes_WhenAddStepFrameTimeWasCalledBeforeMultipleTimes()
        {
            // Arrange
            _gameLoopSteps.StepsNames.Returns(new[] { StepName1 });
            var storage = GetStorage();
            var stepFrameTime1 = TimeSpan.FromMilliseconds(33);
            var stepFrameTime2 = TimeSpan.FromMilliseconds(16);
            var stepFrameTime3 = TimeSpan.FromMilliseconds(8);
            var frameTime = TimeSpan.FromMilliseconds(50);

            var expectedStepFrameTime = stepFrameTime1 + stepFrameTime2 + stepFrameTime3;

            // Act
            storage.AddStepFrameTime(StepName1, stepFrameTime1);
            storage.AddStepFrameTime(StepName1, stepFrameTime2);
            storage.AddStepFrameTime(StepName1, stepFrameTime3);
            storage.AddFrame(frameTime);

            // Assert
            Assert.That(storage.StepsFrames[StepName1].Last().Number, Is.EqualTo(1));
            Assert.That(storage.StepsFrames[StepName1].Last().Time, Is.EqualTo(expectedStepFrameTime));
        }

        [Test]
        public void AddFrame_ShouldAddStepFrameWithTimeSpecifiedInAddStepFrameTimeCall_WhenAddStepFrameTimeAndAddFrameWasCalledBefore()
        {
            // Arrange
            _gameLoopSteps.StepsNames.Returns(new[] { StepName1 });
            var storage = GetStorage();
            var stepFrameTime1 = TimeSpan.FromMilliseconds(33);
            var stepFrameTime2 = TimeSpan.FromMilliseconds(16);
            var frameTime1 = TimeSpan.FromMilliseconds(50);
            var frameTime2 = TimeSpan.FromMilliseconds(75);

            storage.AddStepFrameTime(StepName1, stepFrameTime1);
            storage.AddFrame(frameTime1);

            // Act
            storage.AddStepFrameTime(StepName1, stepFrameTime2);
            storage.AddFrame(frameTime2);

            // Assert
            Assert.That(storage.StepsFrames[StepName1].Last().Number, Is.EqualTo(2));
            Assert.That(storage.StepsFrames[StepName1].Last().Time, Is.EqualTo(stepFrameTime2));
        }

        #endregion

        private PerformanceStatisticsStorage GetStorage()
        {
            return new PerformanceStatisticsStorage(_gameLoopSteps);
        }
    }
}