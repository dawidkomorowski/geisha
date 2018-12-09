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
    public class PerformanceMonitorTests
    {
        private PerformanceMonitor _performanceMonitor;
        private IVariableTimeStepSystem _variableTimeStepSystem1;
        private IVariableTimeStepSystem _variableTimeStepSystem2;
        private IFixedTimeStepSystem _fixedTimeStepSystem1;
        private IFixedTimeStepSystem _fixedTimeStepSystem2;
        private Action Sleep50 { get; } = () => { Thread.Sleep(50); };
        private Action Sleep100 { get; } = () => { Thread.Sleep(100); };

        [SetUp]
        public void SetUp()
        {
            _performanceMonitor = new PerformanceMonitor();

            PerformanceMonitor.Reset();
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
            var performanceMonitor = new PerformanceMonitor();

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
            _performanceMonitor.AddFrame();
            Sleep50();
            _performanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.Fps, Is.GreaterThan(0));
        }

        [Test]
        public void AddFrame_ShouldFpsBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            _performanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.Fps, Is.Zero);
        }

        [Test]
        public void AddFrame_ShouldFrameTimeBeGreaterThanZeroAfterSecondFrameAddition()
        {
            // Arrange
            // Act
            _performanceMonitor.AddFrame();
            Sleep50();
            _performanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.FrameTime, Is.GreaterThan(0));
        }

        [Test]
        public void AddFrame_ShouldFrameTimeBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            _performanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.FrameTime, Is.EqualTo(0));
        }

        [Test]
        public void AddFrame_ShouldIncrementTotalFramesByOne()
        {
            // Arrange
            // Act
            _performanceMonitor.AddFrame();

            // Assert
            Assert.That(_performanceMonitor.TotalFrames, Is.EqualTo(1));
        }

        [Test]
        public void AddFrame_ShouldRealFpsBeGreaterThanZeroAfterSecondFrameAddition()
        {
            // Arrange
            // Act
            _performanceMonitor.AddFrame();
            Sleep50();
            _performanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.RealFps, Is.GreaterThan(0));
        }

        [Test]
        public void AddFrame_ShouldRealFpsBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            _performanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.RealFps, Is.Zero);
        }

        [Test]
        public void AddFrame_ShouldSmoothedFrameTimeBeGreaterThanZeroAfterSecondFrameAddition()
        {
            // Arrange
            // Act
            _performanceMonitor.AddFrame();
            Sleep50();
            _performanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.SmoothedFrameTime, Is.GreaterThan(0));
        }

        [Test]
        public void AddFrame_ShouldSmoothedFrameTimeBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            _performanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.SmoothedFrameTime, Is.EqualTo(0));
        }

        [Test]
        public void AddFrame_ShouldTotalTimeBeGreaterThanZeroAfterSecondFrameAddition()
        {
            // Arrange
            // Act
            _performanceMonitor.AddFrame();
            Sleep50();
            _performanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.TotalTime, Is.GreaterThan(0));
        }

        [Test]
        public void AddFrame_ShouldTotalTimeBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            _performanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.TotalTime, Is.EqualTo(0));
        }

        #endregion

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResult_WhenOneTypeOfFixedTimeStepSystemRecorded()
        {
            // Arrange
            _performanceMonitor.AddFrame();
            _performanceMonitor.RecordSystemExecution(_fixedTimeStepSystem1, Sleep50);
            Sleep100();
            _performanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResult_WhenOneTypeOfVariableTimeStepSystemRecorded()
        {
            // Arrange
            _performanceMonitor.AddFrame();
            _performanceMonitor.RecordSystemExecution(_variableTimeStepSystem1, Sleep50);
            Sleep100();
            _performanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResultForCorrectSystem_WhenFixedTimeStepSystemRecorded()
        {
            // Arrange
            _performanceMonitor.AddFrame();
            _performanceMonitor.RecordSystemExecution(_fixedTimeStepSystem1, Sleep50);
            Sleep100();
            _performanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare.Single().Key, Is.EqualTo(_fixedTimeStepSystem1.Name));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResultForCorrectSystem_WhenVariableTimeStepSystemRecorded()
        {
            // Arrange
            _performanceMonitor.AddFrame();
            _performanceMonitor.RecordSystemExecution(_variableTimeStepSystem1, Sleep50);
            Sleep100();
            _performanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare.Single().Key, Is.EqualTo(_variableTimeStepSystem1.Name));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResultWithShareGreaterThanZero_WhenFixedTimeStepSystemRecorded()
        {
            // Arrange
            _performanceMonitor.AddFrame();
            _performanceMonitor.RecordSystemExecution(_fixedTimeStepSystem1, Sleep50);
            Sleep100();
            _performanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare.Single().Value, Is.GreaterThan(0));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResultWithShareGreaterThanZero_WhenVariableTimeStepSystemRecorded()
        {
            // Arrange
            _performanceMonitor.AddFrame();
            _performanceMonitor.RecordSystemExecution(_variableTimeStepSystem1, Sleep50);
            Sleep100();
            _performanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare.Single().Value, Is.GreaterThan(0));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnTwoResults_WhenTwoTypesOfFixedTimeStepSystemsRecorded()
        {
            // Arrange
            _performanceMonitor.AddFrame();
            _performanceMonitor.RecordSystemExecution(_fixedTimeStepSystem1, Sleep50);
            _performanceMonitor.RecordSystemExecution(_fixedTimeStepSystem2, Sleep50);
            Sleep100();
            Sleep50();
            _performanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(2));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnTwoResults_WhenTwoTypesOfSystemRecorded_OneVariableTimeStep_OneFixedTimeStep()
        {
            // Arrange
            _performanceMonitor.AddFrame();
            _performanceMonitor.RecordSystemExecution(_variableTimeStepSystem1, Sleep50);
            _performanceMonitor.RecordSystemExecution(_fixedTimeStepSystem1, Sleep50);
            Sleep100();
            Sleep50();
            _performanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(2));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnTwoResults_WhenTwoTypesOfVariableTimeStepSystemsRecorded()
        {
            // Arrange
            _performanceMonitor.AddFrame();
            _performanceMonitor.RecordSystemExecution(_variableTimeStepSystem1, Sleep50);
            _performanceMonitor.RecordSystemExecution(_variableTimeStepSystem2, Sleep50);
            Sleep100();
            Sleep50();
            _performanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(2));
        }

        [Test]
        public void Reset_ShouldClearTotalSystemsShare()
        {
            // Arrange
            _performanceMonitor.RecordSystemExecution(_variableTimeStepSystem1, Sleep50);
            _performanceMonitor.RecordSystemExecution(_fixedTimeStepSystem1, Sleep50);
            _performanceMonitor.AddFrame();

            // Act
            PerformanceMonitor.Reset();

            // Assert
            Assert.That(PerformanceMonitor.GetTotalSystemsShare(), Is.Empty);
        }

        [Test]
        public void Reset_ShouldZeroFps()
        {
            // Arrange
            _performanceMonitor.AddFrame();

            // Act
            PerformanceMonitor.Reset();

            // Assert
            Assert.That(PerformanceMonitor.Fps, Is.Zero);
        }

        [Test]
        public void Reset_ShouldZeroFrameTime()
        {
            // Arrange
            _performanceMonitor.AddFrame();

            // Act
            PerformanceMonitor.Reset();

            // Assert
            Assert.That(PerformanceMonitor.FrameTime, Is.Zero);
        }

        [Test]
        public void Reset_ShouldZeroRealFps()
        {
            // Arrange
            _performanceMonitor.AddFrame();

            // Act
            PerformanceMonitor.Reset();

            // Assert
            Assert.That(PerformanceMonitor.RealFps, Is.Zero);
        }

        [Test]
        public void Reset_ShouldZeroSmoothedFrameTime()
        {
            // Arrange
            _performanceMonitor.AddFrame();

            // Act
            PerformanceMonitor.Reset();

            // Assert
            Assert.That(PerformanceMonitor.SmoothedFrameTime, Is.Zero);
        }

        [Test]
        public void Reset_ShouldZeroTotalFrames()
        {
            // Arrange
            _performanceMonitor.AddFrame();

            // Act
            PerformanceMonitor.Reset();

            // Assert
            Assert.That(_performanceMonitor.TotalFrames, Is.Zero);
        }

        [Test]
        public void Reset_ShouldZeroTotalTime()
        {
            // Arrange
            _performanceMonitor.AddFrame();

            // Act
            PerformanceMonitor.Reset();

            // Assert
            Assert.That(PerformanceMonitor.TotalTime, Is.Zero);
        }
    }
}