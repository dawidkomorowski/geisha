using System;
using System.Linq;
using System.Threading;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.Systems;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Diagnostics
{
    [TestFixture]
    public class PerformanceStatisticsRecorderAndProviderTests
    {
        private PerformanceStatisticsRecorderAndProvider _performanceStatisticsRecorderAndProvider;
        private IVariableTimeStepSystem _variableTimeStepSystem1;
        private IVariableTimeStepSystem _variableTimeStepSystem2;
        private IFixedTimeStepSystem _fixedTimeStepSystem1;
        private IFixedTimeStepSystem _fixedTimeStepSystem2;
        private Action Sleep50 { get; } = () => { Thread.Sleep(50); };
        private Action Sleep100 { get; } = () => { Thread.Sleep(100); };

        [SetUp]
        public void SetUp()
        {
            _performanceStatisticsRecorderAndProvider = new PerformanceStatisticsRecorderAndProvider();

            Sleep50();

            _variableTimeStepSystem1 = Substitute.For<IVariableTimeStepSystem>();
            _variableTimeStepSystem2 = Substitute.For<IVariableTimeStepSystem>();
            _fixedTimeStepSystem1 = Substitute.For<IFixedTimeStepSystem>();
            _fixedTimeStepSystem2 = Substitute.For<IFixedTimeStepSystem>();

            _variableTimeStepSystem1.Name.Returns(Guid.NewGuid().ToString());
            _variableTimeStepSystem2.Name.Returns(Guid.NewGuid().ToString());
            _fixedTimeStepSystem1.Name.Returns(Guid.NewGuid().ToString());
            _fixedTimeStepSystem2.Name.Returns(Guid.NewGuid().ToString());
        }

        #region Constructor

        [Test]
        public void Constructor_ShouldCreateInstanceWithTotalFramesEqualZero()
        {
            // Arrange
            // Act
            var performanceMonitor = new PerformanceStatisticsRecorderAndProvider();

            // Assert
            Assert.That(performanceMonitor.TotalFrames, Is.Zero);
        }

        #endregion

        #region AddFrame

        [Test]
        public void AddFrame_ShouldFpsBeGreaterThanZeroAfterSecondFrameAddition()
        {
            // Arrange
            // Act
            _performanceStatisticsRecorderAndProvider.RecordFrame();
            Sleep50();
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Assert
            Assert.That(PerformanceStatisticsRecorderAndProvider.Fps, Is.GreaterThan(0));
        }

        [Test]
        public void AddFrame_ShouldFpsBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Assert
            Assert.That(PerformanceStatisticsRecorderAndProvider.Fps, Is.Zero);
        }

        [Test]
        public void AddFrame_ShouldFrameTimeBeGreaterThanZeroAfterSecondFrameAddition()
        {
            // Arrange
            // Act
            _performanceStatisticsRecorderAndProvider.RecordFrame();
            Sleep50();
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Assert
            Assert.That(PerformanceStatisticsRecorderAndProvider.FrameTime, Is.GreaterThan(0));
        }

        [Test]
        public void AddFrame_ShouldFrameTimeBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Assert
            Assert.That(PerformanceStatisticsRecorderAndProvider.FrameTime, Is.EqualTo(0));
        }

        [Test]
        public void AddFrame_ShouldIncrementTotalFramesByOne()
        {
            // Arrange
            // Act
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Assert
            Assert.That(_performanceStatisticsRecorderAndProvider.TotalFrames, Is.EqualTo(1));
        }

        [Test]
        public void AddFrame_ShouldRealFpsBeGreaterThanZeroAfterSecondFrameAddition()
        {
            // Arrange
            // Act
            _performanceStatisticsRecorderAndProvider.RecordFrame();
            Sleep50();
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Assert
            Assert.That(PerformanceStatisticsRecorderAndProvider.RealFps, Is.GreaterThan(0));
        }

        [Test]
        public void AddFrame_ShouldRealFpsBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Assert
            Assert.That(PerformanceStatisticsRecorderAndProvider.RealFps, Is.Zero);
        }

        [Test]
        public void AddFrame_ShouldSmoothedFrameTimeBeGreaterThanZeroAfterSecondFrameAddition()
        {
            // Arrange
            // Act
            _performanceStatisticsRecorderAndProvider.RecordFrame();
            Sleep50();
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Assert
            Assert.That(PerformanceStatisticsRecorderAndProvider.SmoothedFrameTime, Is.GreaterThan(0));
        }

        [Test]
        public void AddFrame_ShouldSmoothedFrameTimeBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Assert
            Assert.That(PerformanceStatisticsRecorderAndProvider.SmoothedFrameTime, Is.EqualTo(0));
        }

        [Test]
        public void AddFrame_ShouldTotalTimeBeGreaterThanZeroAfterSecondFrameAddition()
        {
            // Arrange
            // Act
            _performanceStatisticsRecorderAndProvider.RecordFrame();
            Sleep50();
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Assert
            Assert.That(PerformanceStatisticsRecorderAndProvider.TotalTime, Is.GreaterThan(0));
        }

        [Test]
        public void AddFrame_ShouldTotalTimeBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Assert
            Assert.That(PerformanceStatisticsRecorderAndProvider.TotalTime, Is.EqualTo(0));
        }

        #endregion

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResult_WhenOneTypeOfFixedTimeStepSystemRecorded()
        {
            // Arrange
            _performanceStatisticsRecorderAndProvider.RecordFrame();
            _performanceStatisticsRecorderAndProvider.RecordSystemExecution(_fixedTimeStepSystem1, Sleep50);
            Sleep100();
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Act
            var totalSystemsShare = PerformanceStatisticsRecorderAndProvider.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResult_WhenOneTypeOfVariableTimeStepSystemRecorded()
        {
            // Arrange
            _performanceStatisticsRecorderAndProvider.RecordFrame();
            _performanceStatisticsRecorderAndProvider.RecordSystemExecution(_variableTimeStepSystem1, Sleep50);
            Sleep100();
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Act
            var totalSystemsShare = PerformanceStatisticsRecorderAndProvider.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResultForCorrectSystem_WhenFixedTimeStepSystemRecorded()
        {
            // Arrange
            _performanceStatisticsRecorderAndProvider.RecordFrame();
            _performanceStatisticsRecorderAndProvider.RecordSystemExecution(_fixedTimeStepSystem1, Sleep50);
            Sleep100();
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Act
            var totalSystemsShare = PerformanceStatisticsRecorderAndProvider.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare.Single().Key, Is.EqualTo(_fixedTimeStepSystem1.Name));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResultForCorrectSystem_WhenVariableTimeStepSystemRecorded()
        {
            // Arrange
            _performanceStatisticsRecorderAndProvider.RecordFrame();
            _performanceStatisticsRecorderAndProvider.RecordSystemExecution(_variableTimeStepSystem1, Sleep50);
            Sleep100();
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Act
            var totalSystemsShare = PerformanceStatisticsRecorderAndProvider.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare.Single().Key, Is.EqualTo(_variableTimeStepSystem1.Name));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResultWithShareGreaterThanZero_WhenFixedTimeStepSystemRecorded()
        {
            // Arrange
            _performanceStatisticsRecorderAndProvider.RecordFrame();
            _performanceStatisticsRecorderAndProvider.RecordSystemExecution(_fixedTimeStepSystem1, Sleep50);
            Sleep100();
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Act
            var totalSystemsShare = PerformanceStatisticsRecorderAndProvider.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare.Single().Value, Is.GreaterThan(0));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResultWithShareGreaterThanZero_WhenVariableTimeStepSystemRecorded()
        {
            // Arrange
            _performanceStatisticsRecorderAndProvider.RecordFrame();
            _performanceStatisticsRecorderAndProvider.RecordSystemExecution(_variableTimeStepSystem1, Sleep50);
            Sleep100();
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Act
            var totalSystemsShare = PerformanceStatisticsRecorderAndProvider.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare.Single().Value, Is.GreaterThan(0));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnTwoResults_WhenTwoTypesOfFixedTimeStepSystemsRecorded()
        {
            // Arrange
            _performanceStatisticsRecorderAndProvider.RecordFrame();
            _performanceStatisticsRecorderAndProvider.RecordSystemExecution(_fixedTimeStepSystem1, Sleep50);
            _performanceStatisticsRecorderAndProvider.RecordSystemExecution(_fixedTimeStepSystem2, Sleep50);
            Sleep100();
            Sleep50();
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Act
            var totalSystemsShare = PerformanceStatisticsRecorderAndProvider.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(2));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnTwoResults_WhenTwoTypesOfSystemRecorded_OneVariableTimeStep_OneFixedTimeStep()
        {
            // Arrange
            _performanceStatisticsRecorderAndProvider.RecordFrame();
            _performanceStatisticsRecorderAndProvider.RecordSystemExecution(_variableTimeStepSystem1, Sleep50);
            _performanceStatisticsRecorderAndProvider.RecordSystemExecution(_fixedTimeStepSystem1, Sleep50);
            Sleep100();
            Sleep50();
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Act
            var totalSystemsShare = PerformanceStatisticsRecorderAndProvider.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(2));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnTwoResults_WhenTwoTypesOfVariableTimeStepSystemsRecorded()
        {
            // Arrange
            _performanceStatisticsRecorderAndProvider.RecordFrame();
            _performanceStatisticsRecorderAndProvider.RecordSystemExecution(_variableTimeStepSystem1, Sleep50);
            _performanceStatisticsRecorderAndProvider.RecordSystemExecution(_variableTimeStepSystem2, Sleep50);
            Sleep100();
            Sleep50();
            _performanceStatisticsRecorderAndProvider.RecordFrame();

            // Act
            var totalSystemsShare = PerformanceStatisticsRecorderAndProvider.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(2));
        }
    }
}