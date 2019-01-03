using System.Linq;
using Geisha.Engine.Core.Diagnostics;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests
{
    [TestFixture]
    public class EngineTests
    {
        [SetUp]
        public void SetUp()
        {
            _gameLoop = Substitute.For<IGameLoop>();
            _engineManager = Substitute.For<IEngineManager>();
            _aggregatedDiagnosticInfoRegistry = Substitute.For<IAggregatedDiagnosticInfoRegistry>();
        }

        private IGameLoop _gameLoop;
        private IEngineManager _engineManager;
        private IAggregatedDiagnosticInfoRegistry _aggregatedDiagnosticInfoRegistry;

        private Engine CreateEngine()
        {
            return new Engine(_gameLoop, _engineManager, _aggregatedDiagnosticInfoRegistry, Enumerable.Empty<IDiagnosticInfoProvider>());
        }

        [Test]
        public void Constructor_ShouldRegisterDiagnosticInfoProvidersInRegistry()
        {
            // Arrange
            var provider1 = Substitute.For<IDiagnosticInfoProvider>();
            var provider2 = Substitute.For<IDiagnosticInfoProvider>();
            var provider3 = Substitute.For<IDiagnosticInfoProvider>();

            // Act
            var engine = new Engine(_gameLoop, _engineManager, _aggregatedDiagnosticInfoRegistry, new[] {provider1, provider2, provider3});

            // Assert
            _aggregatedDiagnosticInfoRegistry.Received().Register(provider1);
            _aggregatedDiagnosticInfoRegistry.Received().Register(provider2);
            _aggregatedDiagnosticInfoRegistry.Received().Register(provider3);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void IsScheduledForShutdown_ShouldReturnValueProvidedByEngineManager(bool isScheduledForShutdown)
        {
            // Arrange
            _engineManager.IsEngineScheduledForShutdown.Returns(isScheduledForShutdown);

            var engine = CreateEngine();

            // Act
            var actual = engine.IsScheduledForShutdown;

            // Assert
            Assert.That(actual, Is.EqualTo(isScheduledForShutdown));
        }

        [Test]
        public void Update_ShouldUpdateGameLoop()
        {
            // Arrange
            var engine = CreateEngine();

            // Act
            engine.Update();

            // Assert
            _gameLoop.Received(1).Update();
        }
    }
}