using System;
using System.Linq;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.Systems;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Diagnostics
{
    [TestFixture]
    public class PerformanceStatisticsStorageTests
    {
        private IFixedTimeStepSystem _fixedTimeStepSystem1;
        private IFixedTimeStepSystem _fixedTimeStepSystem2;
        private IVariableTimeStepSystem _variableTimeStepSystem1;
        private IVariableTimeStepSystem _variableTimeStepSystem2;
        private ISystemsProvider _systemsProvider;

        [SetUp]
        public void SetUp()
        {
            _fixedTimeStepSystem1 = Substitute.For<IFixedTimeStepSystem>();
            _fixedTimeStepSystem2 = Substitute.For<IFixedTimeStepSystem>();
            _variableTimeStepSystem1 = Substitute.For<IVariableTimeStepSystem>();
            _variableTimeStepSystem2 = Substitute.For<IVariableTimeStepSystem>();

            _fixedTimeStepSystem1.Name.Returns(Guid.NewGuid().ToString());
            _fixedTimeStepSystem2.Name.Returns(Guid.NewGuid().ToString());
            _variableTimeStepSystem1.Name.Returns(Guid.NewGuid().ToString());
            _variableTimeStepSystem2.Name.Returns(Guid.NewGuid().ToString());

            _systemsProvider = Substitute.For<ISystemsProvider>();
        }

        #region Constructor

        [Test]
        public void Constructor_ShouldCreateStorageWithTotalFramesEqualZero()
        {
            // Arrange
            // Act
            var storage = new PerformanceStatisticsStorage(_systemsProvider);

            // Assert
            Assert.That(storage.TotalFrames, Is.Zero);
        }

        [Test]
        public void Constructor_ShouldCreateStorageWithFramesListContaining100FrameTimesAllEqualZero()
        {
            // Arrange
            // Act
            var storage = new PerformanceStatisticsStorage(_systemsProvider);

            // Assert
            Assert.That(storage.Frames.Count(), Is.EqualTo(100));
            Assert.That(storage.Frames.Select(f => f.Number), Is.EqualTo(Enumerable.Range(0, 100).Select(i => 0)));
            Assert.That(storage.Frames.Select(f => f.Time), Is.EqualTo(Enumerable.Range(0, 100).Select(i => TimeSpan.Zero)));
        }

        [Test]
        public void
            Constructor_ShouldCreateStorageWithSystemsFramesFor4SystemsWithFramesListContaining100FrameTimesAllEqualZero_When4SystemsReturnedBySystemsProvider()
        {
            // Arrange
            var systemsProvider = Substitute.For<ISystemsProvider>();
            systemsProvider.GetFixedTimeStepSystems().Returns(new[] {_fixedTimeStepSystem1, _fixedTimeStepSystem2});
            systemsProvider.GetVariableTimeStepSystems().Returns(new[] {_variableTimeStepSystem1, _variableTimeStepSystem2});

            // Act
            var storage = new PerformanceStatisticsStorage(systemsProvider);

            // Assert
            Assert.That(storage.SystemsFrames.Keys,
                Is.EquivalentTo(new[] {_fixedTimeStepSystem1.Name, _fixedTimeStepSystem2.Name, _variableTimeStepSystem1.Name, _variableTimeStepSystem2.Name}));

            Assert.That(storage.SystemsFrames[_fixedTimeStepSystem1.Name].Select(f => f.Number), Is.EqualTo(Enumerable.Range(0, 100).Select(i => 0)));
            Assert.That(storage.SystemsFrames[_fixedTimeStepSystem1.Name].Select(f => f.Time), Is.EqualTo(Enumerable.Range(0, 100).Select(i => TimeSpan.Zero)));

            Assert.That(storage.SystemsFrames[_fixedTimeStepSystem2.Name].Select(f => f.Number), Is.EqualTo(Enumerable.Range(0, 100).Select(i => 0)));
            Assert.That(storage.SystemsFrames[_fixedTimeStepSystem2.Name].Select(f => f.Time), Is.EqualTo(Enumerable.Range(0, 100).Select(i => TimeSpan.Zero)));

            Assert.That(storage.SystemsFrames[_variableTimeStepSystem1.Name].Select(f => f.Number), Is.EqualTo(Enumerable.Range(0, 100).Select(i => 0)));
            Assert.That(storage.SystemsFrames[_variableTimeStepSystem1.Name].Select(f => f.Time),
                Is.EqualTo(Enumerable.Range(0, 100).Select(i => TimeSpan.Zero)));

            Assert.That(storage.SystemsFrames[_variableTimeStepSystem2.Name].Select(f => f.Number), Is.EqualTo(Enumerable.Range(0, 100).Select(i => 0)));
            Assert.That(storage.SystemsFrames[_variableTimeStepSystem2.Name].Select(f => f.Time),
                Is.EqualTo(Enumerable.Range(0, 100).Select(i => TimeSpan.Zero)));
        }

        #endregion

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
            var lastThreeFrames = storage.Frames.Skip(97);
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

        private PerformanceStatisticsStorage GetStorage()
        {
            return new PerformanceStatisticsStorage(_systemsProvider);
        }
    }
}