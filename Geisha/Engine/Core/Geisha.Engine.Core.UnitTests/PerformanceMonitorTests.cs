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
        private ISystem VariableSystem { get; } = new VariableSystemImpl();
        private ISystem FixedSystem { get; } = new FixedSystemImpl();
        private Action Sleep50 { get; } = () => { Thread.Sleep(50); };
        private Action Sleep100 { get; } = () => { Thread.Sleep(100); };

        [SetUp]
        public void SetUp()
        {
            PerformanceMonitor.Reset();
            Sleep50();
        }

        #region Reset

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
            //PerformanceMonitor.Reset();

            // Assert
            Assert.That(PerformanceMonitor.TotalTime, Is.Zero);
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
        public void Reset_ShouldClearTotalSystemsShare()
        {
            // Arrange
            PerformanceMonitor.RecordVariableSystemExecution(VariableSystem, Sleep50);
            PerformanceMonitor.RecordFixedSystemExecution(FixedSystem, Sleep50);
            PerformanceMonitor.AddFrame();

            // Act
            PerformanceMonitor.Reset();

            // Assert
            Assert.That(PerformanceMonitor.GetTotalSystemsShare(), Is.Empty);
        }

        #endregion

        #region AddFrame

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
        public void AddFrame_ShouldTotalTimeBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            PerformanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.TotalTime, Is.EqualTo(0));
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
        public void AddFrame_ShouldFrameTimeBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            PerformanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.FrameTime, Is.EqualTo(0));
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
        public void AddFrame_ShouldSmoothedFrameTimeBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            PerformanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.SmoothedFrameTime, Is.EqualTo(0));
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
        public void AddFrame_ShouldFpsBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            PerformanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.Fps, Is.Zero);
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
        public void AddFrame_ShouldRealFpsBeZeroAfterFirstFrameAddition()
        {
            // Arrange
            // Act
            PerformanceMonitor.AddFrame();

            // Assert
            Assert.That(PerformanceMonitor.RealFps, Is.Zero);
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

        #endregion

        #region GetTotalSystemsShare

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResult_WhenOneTypeOfVariableSystemRecorded()
        {
            // Arrange
            PerformanceMonitor.AddFrame();
            PerformanceMonitor.RecordVariableSystemExecution(VariableSystem, Sleep50);
            Sleep100();
            PerformanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnTwoResults_WhenTwoTypesOfVariableSystemRecorded()
        {
            // Arrange
            PerformanceMonitor.AddFrame();
            PerformanceMonitor.RecordVariableSystemExecution(VariableSystem, Sleep50);
            PerformanceMonitor.RecordVariableSystemExecution(FixedSystem, Sleep50);
            Sleep100();
            Sleep50();
            PerformanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(2));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResult_WhenOneTypeOfFixedSystemRecorded()
        {
            // Arrange
            PerformanceMonitor.AddFrame();
            PerformanceMonitor.RecordFixedSystemExecution(FixedSystem, Sleep50);
            Sleep100();
            PerformanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnTwoResults_WhenTwoTypesOfFixedSystemRecorded()
        {
            // Arrange
            PerformanceMonitor.AddFrame();
            PerformanceMonitor.RecordFixedSystemExecution(VariableSystem, Sleep50);
            PerformanceMonitor.RecordFixedSystemExecution(FixedSystem, Sleep50);
            Sleep100();
            Sleep50();
            PerformanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(2));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnTwoResults_WhenTwoTypesOfSystemRecorded_OneVariable_OneFixed()
        {
            // Arrange
            PerformanceMonitor.AddFrame();
            PerformanceMonitor.RecordVariableSystemExecution(VariableSystem, Sleep50);
            PerformanceMonitor.RecordFixedSystemExecution(FixedSystem, Sleep50);
            Sleep100();
            Sleep50();
            PerformanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare, Has.Count.EqualTo(2));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResultOfCorrectType_WhenVariableSystemRecorded()
        {
            // Arrange
            PerformanceMonitor.AddFrame();
            PerformanceMonitor.RecordVariableSystemExecution(VariableSystem, Sleep50);
            Sleep100();
            PerformanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare.Single().Key, Is.EqualTo(VariableSystem.GetType()));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResultOfCorrectType_WhenFixedSystemRecorded()
        {
            // Arrange
            PerformanceMonitor.AddFrame();
            PerformanceMonitor.RecordFixedSystemExecution(FixedSystem, Sleep50);
            Sleep100();
            PerformanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare.Single().Key, Is.EqualTo(FixedSystem.GetType()));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResultWithShareGreaterThanZero_WhenVariableSystemRecorded()
        {
            // Arrange
            PerformanceMonitor.AddFrame();
            PerformanceMonitor.RecordVariableSystemExecution(VariableSystem, Sleep50);
            Sleep100();
            PerformanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare.Single().Value, Is.GreaterThan(0));
        }

        [Test]
        public void GetTotalSystemsShare_ShouldReturnOneResultWithShareGreaterThanZero_WhenFixedSystemRecorded()
        {
            // Arrange
            PerformanceMonitor.AddFrame();
            PerformanceMonitor.RecordFixedSystemExecution(FixedSystem, Sleep50);
            Sleep100();
            PerformanceMonitor.AddFrame();

            // Act
            var totalSystemsShare = PerformanceMonitor.GetTotalSystemsShare();

            // Assert
            Assert.That(totalSystemsShare.Single().Value, Is.GreaterThan(0));
        }

        #endregion

        #region Dummy systems implementation

        private class VariableSystemImpl : ISystem
        {
            public int Priority { get; set; }
            public UpdateMode UpdateMode { get; set; }

            public void Update(Scene scene, double deltaTime)
            {
                throw new NotImplementedException();
            }

            public void FixedUpdate(Scene scene)
            {
                throw new NotImplementedException();
            }
        }

        private class FixedSystemImpl : ISystem
        {
            public int Priority { get; set; }
            public UpdateMode UpdateMode { get; set; }

            public void Update(Scene scene, double deltaTime)
            {
                throw new NotImplementedException();
            }

            public void FixedUpdate(Scene scene)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}