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
        }

        private IGameLoop _gameLoop;
        private IEngineManager _engineManager;

        private Engine CreateEngine()
        {
            return new Engine(_gameLoop, _engineManager);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void IsScheduledForShutdown(bool isScheduledForShutdown)
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
        public void Update()
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