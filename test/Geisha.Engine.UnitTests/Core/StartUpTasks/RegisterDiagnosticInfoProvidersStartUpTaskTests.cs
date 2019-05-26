using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.StartUpTasks;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.StartUpTasks
{
    [TestFixture]
    public class RegisterDiagnosticInfoProvidersStartUpTaskTests
    {
        [Test]
        public void Run_ShouldRegisterDiagnosticInfoProvidersInRegistry()
        {
            // Arrange
            var aggregatedDiagnosticInfoRegistry = Substitute.For<IAggregatedDiagnosticInfoRegistry>();

            var provider1 = Substitute.For<IDiagnosticInfoProvider>();
            var provider2 = Substitute.For<IDiagnosticInfoProvider>();
            var provider3 = Substitute.For<IDiagnosticInfoProvider>();

            var startUpTask = new RegisterDiagnosticInfoProvidersStartUpTask(aggregatedDiagnosticInfoRegistry, new[] {provider1, provider2, provider3});

            // Act
            startUpTask.Run();

            // Assert
            aggregatedDiagnosticInfoRegistry.Received().Register(provider1);
            aggregatedDiagnosticInfoRegistry.Received().Register(provider2);
            aggregatedDiagnosticInfoRegistry.Received().Register(provider3);
        }
    }
}