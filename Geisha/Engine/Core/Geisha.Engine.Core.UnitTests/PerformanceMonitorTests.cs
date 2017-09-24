using System;
using System.Linq;
using System.Threading;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests
{
    [TestFixture]
    public class PerformanceMonitorTests
    {
        [SetUp]
        public void SetUp()
        {
            PerformanceMonitor.Reset();
            Sleep50();
        }

        private IVariableTimeStepSystem VariableTimeStepSystem1 { get; } = new VariableTimeStepSystemImpl1();
        private IVariableTimeStepSystem VariableTimeStepSystem2 { get; } = new VariableTimeStepSystemImpl2();
        private IFixedTimeStepSystem FixedTimeStepSystem1 { get; } = new FixedTimeStepSystemImpl1();
        private IFixedTimeStepSystem FixedTimeStepSystem2 { get; } = new FixedTimeStepSystemImpl2();
        private Action Sleep50 { get; } = () => { Thread.Sleep(50); };
        private Action Sleep100 { get; } = () => { Thread.Sleep(100); };

        private class VariableTimeStepSystemImpl1 : IVariableTimeStepSystem
        {
            public int Priority { get; set; }

            public void Update(Scene scene, double deltaTime)
            {
                throw new NotSupportedException();
            }
        }

        private class VariableTimeStepSystemImpl2 : IVariableTimeStepSystem
        {
            public int Priority { get; set; }

            public void Update(Scene scene, double deltaTime)
            {
                throw new NotSupportedException();
            }
        }

        private class FixedTimeStepSystemImpl1 : IFixedTimeStepSystem
        {
            public int Priority { get; set; }

            public void FixedUpdate(Scene scene)
            {
                throw new NotSupportedException();
            }
        }

        private class FixedTimeStepSystemImpl2 : IFixedTimeStepSystem
        {
            public int Priority { get; set; }

            public void FixedUpdate(Scene scene)
            {
                throw new NotSupportedException();
            }
        }

        [Test]
        public void AddFrame_ShouldFpsBeGreaterThanZeroAfterSecondFrameAddition()
        {
            // Arrange
            // Act
            PerformanceMonitor.AddFrame();
            Sleep50();
            PerformanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.Fps, Is.GreaterThan(0));
        }

        [Test]
        public void AddFrame_ShouldFpsBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            PerformanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.Fps, Is.Zero);
        }

        [Test]
        public void AddFrame_ShouldFrameTimeBeGreaterThanZeroAfterSecondFrameAddition()
        {
            // Arrange
            // Act
            PerformanceMonitor.AddFrame();
            Sleep50();
            PerformanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.FrameTime, Is.GreaterThan(0));
        }

        [Test]
        public void AddFrame_ShouldFrameTimeBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            PerformanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.FrameTime, Is.EqualTo(0));
        }

        [Test]
        public void AddFrame_ShouldIncrementTotalFramesByOne()
        {
            // Arrange
            // Act
            PerformanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.TotalFrames, Is.EqualTo(1));
        }

        [Test]
        public void AddFrame_ShouldRealFpsBeGreaterThanZeroAfterSecondFrameAddition()
        {
            // Arrange
            // Act
            PerformanceMonitor.AddFrame();
            Sleep50();
            PerformanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.RealFps, Is.GreaterThan(0));
        }

        [Test]
        public void AddFrame_ShouldRealFpsBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            PerformanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.RealFps, Is.Zero);
        }

        [Test]
        public void AddFrame_ShouldSmoothedFrameTimeBeGreaterThanZeroAfterSecondFrameAddition()
        {
            // Arrange
            // Act
            PerformanceMonitor.AddFrame();
            Sleep50();
            PerformanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.SmoothedFrameTime, Is.GreaterThan(0));
        }

        [Test]
        public void AddFrame_ShouldSmoothedFrameTimeBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            PerformanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.SmoothedFrameTime, Is.EqualTo(0));
        }

        [Test]
        public void AddFrame_ShouldTotalTimeBeGreaterThanZeroAfterSecondFrameAddition()
        {
            // Arrange
            // Act
            PerformanceMonitor.AddFrame();
            Sleep50();
            PerformanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.TotalTime, Is.GreaterThan(0));
        }

        [Test]
        public void AddFrame_ShouldTotalTimeBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            PerformanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.TotalTime, Is.EqualTo(0));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResult_WhenOneTypeOfFixedTimeStepSystemRecorded()
        {
            // Arrange
            PerformanceMonitor.AddFrame();
            PerformanceMonitor.RecordSystemExecution(FixedTimeStepSystem1, Sleep50);
            Sleep100();
            PerformanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResult_WhenOneTypeOfVariableTimeStepSystemRecorded()
        {
            // Arrange
            PerformanceMonitor.AddFrame();
            PerformanceMonitor.RecordSystemExecution(VariableTimeStepSystem1, Sleep50);
            Sleep100();
            PerformanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResultOfCorrectType_WhenFixedTimeStepSystemRecorded()
        {
            // Arrange
            PerformanceMonitor.AddFrame();
            PerformanceMonitor.RecordSystemExecution(FixedTimeStepSystem1, Sleep50);
            Sleep100();
            PerformanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare.Single().Key, Is.EqualTo(FixedTimeStepSystem1.GetType()));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResultOfCorrectType_WhenVariableTimeStepSystemRecorded()
        {
            // Arrange
            PerformanceMonitor.AddFrame();
            PerformanceMonitor.RecordSystemExecution(VariableTimeStepSystem1, Sleep50);
            Sleep100();
            PerformanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare.Single().Key, Is.EqualTo(VariableTimeStepSystem1.GetType()));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResultWithShareGreaterThanZero_WhenFixedTimeStepSystemRecorded()
        {
            // Arrange
            PerformanceMonitor.AddFrame();
            PerformanceMonitor.RecordSystemExecution(FixedTimeStepSystem1, Sleep50);
            Sleep100();
            PerformanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare.Single().Value, Is.GreaterThan(0));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResultWithShareGreaterThanZero_WhenVariableTimeStepSystemRecorded()
        {
            // Arrange
            PerformanceMonitor.AddFrame();
            PerformanceMonitor.RecordSystemExecution(VariableTimeStepSystem1, Sleep50);
            Sleep100();
            PerformanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare.Single().Value, Is.GreaterThan(0));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnTwoResults_WhenTwoTypesOfFixedTimeStepSystemsRecorded()
        {
            // Arrange
            PerformanceMonitor.AddFrame();
            PerformanceMonitor.RecordSystemExecution(FixedTimeStepSystem1, Sleep50);
            PerformanceMonitor.RecordSystemExecution(FixedTimeStepSystem2, Sleep50);
            Sleep100();
            Sleep50();
            PerformanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(2));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnTwoResults_WhenTwoTypesOfSystemRecorded_OneVariableTimeStep_OneFixedTimeStep()
        {
            // Arrange
            PerformanceMonitor.AddFrame();
            PerformanceMonitor.RecordSystemExecution(VariableTimeStepSystem1, Sleep50);
            PerformanceMonitor.RecordSystemExecution(FixedTimeStepSystem1, Sleep50);
            Sleep100();
            Sleep50();
            PerformanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(2));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnTwoResults_WhenTwoTypesOfVariableTimeStepSystemsRecorded()
        {
            // Arrange
            PerformanceMonitor.AddFrame();
            PerformanceMonitor.RecordSystemExecution(VariableTimeStepSystem1, Sleep50);
            PerformanceMonitor.RecordSystemExecution(VariableTimeStepSystem2, Sleep50);
            Sleep100();
            Sleep50();
            PerformanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(2));
        }

        [Test]
        public void Reset_ShouldClearTotalSystemsShare()
        {
            // Arrange
            PerformanceMonitor.RecordSystemExecution(VariableTimeStepSystem1, Sleep50);
            PerformanceMonitor.RecordSystemExecution(FixedTimeStepSystem1, Sleep50);
            PerformanceMonitor.AddFrame();

            // Act
            PerformanceMonitor.Reset();

            // Assert
            Assert.That(PerformanceMonitor.GetTotalSystemsShare(), Is.Empty);
        }

        [Test]
        public void Reset_ShouldZeroFps()
        {
            // Arrange
            PerformanceMonitor.AddFrame();

            // Act
            PerformanceMonitor.Reset();

            // Assert
            Assert.That(PerformanceMonitor.Fps, Is.Zero);
        }

        [Test]
        public void Reset_ShouldZeroFrameTime()
        {
            // Arrange
            PerformanceMonitor.AddFrame();

            // Act
            PerformanceMonitor.Reset();

            // Assert
            Assert.That(PerformanceMonitor.FrameTime, Is.Zero);
        }

        [Test]
        public void Reset_ShouldZeroRealFps()
        {
            // Arrange
            PerformanceMonitor.AddFrame();

            // Act
            PerformanceMonitor.Reset();

            // Assert
            Assert.That(PerformanceMonitor.RealFps, Is.Zero);
        }

        [Test]
        public void Reset_ShouldZeroSmoothedFrameTime()
        {
            // Arrange
            PerformanceMonitor.AddFrame();

            // Act
            PerformanceMonitor.Reset();

            // Assert
            Assert.That(PerformanceMonitor.SmoothedFrameTime, Is.Zero);
        }

        [Test]
        public void Reset_ShouldZeroTotalFrames()
        {
            // Arrange
            PerformanceMonitor.AddFrame();

            // Act
            PerformanceMonitor.Reset();

            // Assert
            Assert.That(PerformanceMonitor.TotalFrames, Is.Zero);
        }

        [Test]
        public void Reset_ShouldZeroTotalTime()
        {
            // Arrange
            PerformanceMonitor.AddFrame();

            // Act
            PerformanceMonitor.Reset();

            // Assert
            Assert.That(PerformanceMonitor.TotalTime, Is.Zero);
        }
    }
}