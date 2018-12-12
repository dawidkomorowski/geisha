using System;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.Systems;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Diagnostics
{
    [TestFixture]
    public class PerformanceStatisticsRecorderTests
    {
        private PerformanceStatisticsRecorder _performanceStatisticsRecorder;
        private IFixedTimeStepSystem _fixedTimeStepSystem1;
        private IFixedTimeStepSystem _fixedTimeStepSystem2;
        private IVariableTimeStepSystem _variableTimeStepSystem1;
        private IVariableTimeStepSystem _variableTimeStepSystem2;

        [SetUp]
        public void SetUp()
        {
            _performanceStatisticsRecorder = new PerformanceStatisticsRecorder();

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
    }
}