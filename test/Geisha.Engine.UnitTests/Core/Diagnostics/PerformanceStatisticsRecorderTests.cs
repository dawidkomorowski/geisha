using System;
using System.Diagnostics;
using System.Threading;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.Systems;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Diagnostics
{
    [TestFixture]
    public class PerformanceStatisticsRecorderTests
    {
        private PerformanceStatisticsRecorder _performanceStatisticsRecorder;
        private IPerformanceStatisticsStorage _performanceStatisticsStorage;
        private IFixedTimeStepSystem _fixedTimeStepSystem1;
        private IFixedTimeStepSystem _fixedTimeStepSystem2;
        private IVariableTimeStepSystem _variableTimeStepSystem1;
        private IVariableTimeStepSystem _variableTimeStepSystem2;

        private Action Sleep50 { get; } = () => { Thread.Sleep(50); };

        [SetUp]
        public void SetUp()
        {
            _performanceStatisticsStorage = Substitute.For<IPerformanceStatisticsStorage>();
            _performanceStatisticsRecorder = new PerformanceStatisticsRecorder(_performanceStatisticsStorage);

            _fixedTimeStepSystem1 = Substitute.For<IFixedTimeStepSystem>();
            _fixedTimeStepSystem2 = Substitute.For<IFixedTimeStepSystem>();
            _variableTimeStepSystem1 = Substitute.For<IVariableTimeStepSystem>();
            _variableTimeStepSystem2 = Substitute.For<IVariableTimeStepSystem>();

            _fixedTimeStepSystem1.Name.Returns(Guid.NewGuid().ToString());
            _fixedTimeStepSystem2.Name.Returns(Guid.NewGuid().ToString());
            _variableTimeStepSystem1.Name.Returns(Guid.NewGuid().ToString());
            _variableTimeStepSystem2.Name.Returns(Guid.NewGuid().ToString());
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
            Sleep50();
            _performanceStatisticsRecorder.RecordFrame();

            // Assert
            stopwatch.Stop();
            _performanceStatisticsStorage.AddFrame(Arg.Is<TimeSpan>(ts => ts.TotalMilliseconds >= 50 && ts < stopwatch.Elapsed));
        }

        [Test]
        public void RecordSystemExecution_FixedTimeStepSystem_ShouldInvokeGivenAction()
        {
            // Arrange
            var wasInvoked = false;

            // Act
            _performanceStatisticsRecorder.RecordSystemExecution(_fixedTimeStepSystem1, () => { wasInvoked = true; });

            // Assert
            Assert.That(wasInvoked, Is.True);
        }

        [Test]
        public void RecordSystemExecution_VariableTimeStepSystem_ShouldInvokeGivenAction()
        {
            // Arrange
            var wasInvoked = false;

            // Act
            _performanceStatisticsRecorder.RecordSystemExecution(_variableTimeStepSystem1, () => { wasInvoked = true; });

            // Assert
            Assert.That(wasInvoked, Is.True);
        }

        [Test]
        public void RecordSystemExecution_FixedTimeStepSystem_ShouldAddSystemFrameTimeToStorage()
        {
            // Arrange
            var stopwatch = Stopwatch.StartNew();

            // Act
            _performanceStatisticsRecorder.RecordSystemExecution(_fixedTimeStepSystem1, Sleep50);

            // Assert
            stopwatch.Stop();
            _performanceStatisticsStorage.Received()
                .AddSystemFrameTime(_fixedTimeStepSystem1.Name, Arg.Is<TimeSpan>(ts => ts.TotalMilliseconds >= 50 && ts < stopwatch.Elapsed));
        }

        [Test]
        public void RecordSystemExecution_VariableTimeStepSystem_ShouldAddSystemFrameTimeToStorage()
        {
            // Arrange
            var stopwatch = Stopwatch.StartNew();

            // Act
            _performanceStatisticsRecorder.RecordSystemExecution(_variableTimeStepSystem1, Sleep50);

            // Assert
            stopwatch.Stop();
            _performanceStatisticsStorage.Received()
                .AddSystemFrameTime(_variableTimeStepSystem1.Name, Arg.Is<TimeSpan>(ts => ts.TotalMilliseconds >= 50 && ts < stopwatch.Elapsed));
        }
    }
}