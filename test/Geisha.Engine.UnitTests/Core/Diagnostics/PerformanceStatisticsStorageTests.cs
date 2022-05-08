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
        private const string SystemName1 = nameof(SystemName1);
        private const string SystemName2 = nameof(SystemName2);
        private const string SystemName3 = nameof(SystemName3);
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
            Constructor_ShouldCreateStorageWithSystemsFramesFor3SystemsWithFramesListContaining100FrameTimesAllEqualZero_When3SystemsAvailableInEngineSystems()
        {
            // Arrange
            _gameLoopSteps.SystemsNames.Returns(new[] {SystemName1, SystemName2, SystemName3});

            // Act
            var storage = new PerformanceStatisticsStorage(_gameLoopSteps);

            // Assert
            Assert.That(storage.SystemsFrames.Keys, Is.EquivalentTo(new[] {SystemName1, SystemName2, SystemName3}));

            Assert.That(storage.SystemsFrames[SystemName1].Select(f => f.Number), Is.EqualTo(Enumerable.Range(0, 100).Select(i => 0)));
            Assert.That(storage.SystemsFrames[SystemName1].Select(f => f.Time), Is.EqualTo(Enumerable.Range(0, 100).Select(i => TimeSpan.Zero)));

            Assert.That(storage.SystemsFrames[SystemName2].Select(f => f.Number), Is.EqualTo(Enumerable.Range(0, 100).Select(i => 0)));
            Assert.That(storage.SystemsFrames[SystemName2].Select(f => f.Time), Is.EqualTo(Enumerable.Range(0, 100).Select(i => TimeSpan.Zero)));

            Assert.That(storage.SystemsFrames[SystemName3].Select(f => f.Number), Is.EqualTo(Enumerable.Range(0, 100).Select(i => 0)));
            Assert.That(storage.SystemsFrames[SystemName3].Select(f => f.Time), Is.EqualTo(Enumerable.Range(0, 100).Select(i => TimeSpan.Zero)));
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

        #region SystemFrames

        [Test]
        public void AddSystemFrameTime_ShouldNotAddSystemFrame_WhenAddFrameWasNotCalledAfterAddSystemFrameTime()
        {
            // Arrange
            _gameLoopSteps.SystemsNames.Returns(new[] {SystemName1});
            var storage = GetStorage();
            var systemFrameTime = TimeSpan.FromMilliseconds(33);

            // Act
            storage.AddSystemFrameTime(SystemName1, systemFrameTime);

            // Assert
            Assert.That(storage.SystemsFrames[SystemName1].Last().Number, Is.Zero);
            Assert.That(storage.SystemsFrames[SystemName1].Last().Time, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void AddFrame_ShouldAddSystemFrameWithIncrementedNumberButZeroTime_WhenAddFrameWasCalledButAddSystemFrameTimeWasNotCalledBefore()
        {
            // Arrange
            _gameLoopSteps.SystemsNames.Returns(new[] {SystemName1});
            var storage = GetStorage();
            var frameTime = TimeSpan.FromMilliseconds(50);

            // Act
            storage.AddFrame(frameTime);

            // Assert
            Assert.That(storage.SystemsFrames[SystemName1].Last().Number, Is.EqualTo(1));
            Assert.That(storage.SystemsFrames[SystemName1].Last().Time, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void
            AddFrame_ShouldAddSeveralSystemFramesWithIncrementedNumbersButZeroTime_WhenAddFrameWasCalledMultipleTimesButAddSystemFrameTimeWasNeverCalled()
        {
            // Arrange
            _gameLoopSteps.SystemsNames.Returns(new[] {SystemName1});
            var storage = GetStorage();
            var frameTime = TimeSpan.FromMilliseconds(50);

            // Act
            storage.AddFrame(frameTime);
            storage.AddFrame(frameTime);
            storage.AddFrame(frameTime);

            // Assert
            Assert.That(storage.SystemsFrames[SystemName1].Skip(97).First().Number, Is.EqualTo(1));
            Assert.That(storage.SystemsFrames[SystemName1].Skip(97).First().Time, Is.EqualTo(TimeSpan.Zero));
            Assert.That(storage.SystemsFrames[SystemName1].Skip(98).First().Number, Is.EqualTo(2));
            Assert.That(storage.SystemsFrames[SystemName1].Skip(98).First().Time, Is.EqualTo(TimeSpan.Zero));
            Assert.That(storage.SystemsFrames[SystemName1].Skip(99).First().Number, Is.EqualTo(3));
            Assert.That(storage.SystemsFrames[SystemName1].Skip(99).First().Time, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void AddFrame_ShouldAddSystemFrame_WhenAddSystemFrameTimeWasCalledBefore()
        {
            // Arrange
            _gameLoopSteps.SystemsNames.Returns(new[] {SystemName1});
            var storage = GetStorage();
            var systemFrameTime = TimeSpan.FromMilliseconds(33);
            var frameTime = TimeSpan.FromMilliseconds(50);

            // Act
            storage.AddSystemFrameTime(SystemName1, systemFrameTime);
            storage.AddFrame(frameTime);

            // Assert
            Assert.That(storage.SystemsFrames[SystemName1].Last().Number, Is.EqualTo(1));
            Assert.That(storage.SystemsFrames[SystemName1].Last().Time, Is.EqualTo(systemFrameTime));
        }

        [Test]
        public void AddFrame_ShouldAddSingleSystemFrameWithTimeBeingSumOfAllSystemFrameTimes_WhenAddSystemFrameTimeWasCalledBeforeMultipleTimes()
        {
            // Arrange
            _gameLoopSteps.SystemsNames.Returns(new[] {SystemName1});
            var storage = GetStorage();
            var systemFrameTime1 = TimeSpan.FromMilliseconds(33);
            var systemFrameTime2 = TimeSpan.FromMilliseconds(16);
            var systemFrameTime3 = TimeSpan.FromMilliseconds(8);
            var frameTime = TimeSpan.FromMilliseconds(50);

            var expectedSystemFrameTime = systemFrameTime1 + systemFrameTime2 + systemFrameTime3;

            // Act
            storage.AddSystemFrameTime(SystemName1, systemFrameTime1);
            storage.AddSystemFrameTime(SystemName1, systemFrameTime2);
            storage.AddSystemFrameTime(SystemName1, systemFrameTime3);
            storage.AddFrame(frameTime);

            // Assert
            Assert.That(storage.SystemsFrames[SystemName1].Last().Number, Is.EqualTo(1));
            Assert.That(storage.SystemsFrames[SystemName1].Last().Time, Is.EqualTo(expectedSystemFrameTime));
        }

        [Test]
        public void AddFrame_ShouldAddSystemFrameWithTimeSpecifiedInAddSystemFrameTimeCall_WhenAddSystemFrameTimeAndAddFrameWasCalledBefore()
        {
            // Arrange
            _gameLoopSteps.SystemsNames.Returns(new[] {SystemName1});
            var storage = GetStorage();
            var systemFrameTime1 = TimeSpan.FromMilliseconds(33);
            var systemFrameTime2 = TimeSpan.FromMilliseconds(16);
            var frameTime1 = TimeSpan.FromMilliseconds(50);
            var frameTime2 = TimeSpan.FromMilliseconds(75);

            storage.AddSystemFrameTime(SystemName1, systemFrameTime1);
            storage.AddFrame(frameTime1);

            // Act
            storage.AddSystemFrameTime(SystemName1, systemFrameTime2);
            storage.AddFrame(frameTime2);

            // Assert
            Assert.That(storage.SystemsFrames[SystemName1].Last().Number, Is.EqualTo(2));
            Assert.That(storage.SystemsFrames[SystemName1].Last().Time, Is.EqualTo(systemFrameTime2));
        }

        #endregion

        private PerformanceStatisticsStorage GetStorage()
        {
            return new PerformanceStatisticsStorage(_gameLoopSteps);
        }
    }
}