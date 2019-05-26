using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests
{
    [TestFixture]
    public class EngineManagerTests
    {
        [Test]
        public void ScheduleEngineShutdown_ShouldMake_IsEngineScheduledForShutdown_True()
        {
            // Arrange
            var engineManager = new EngineManager();

            // Assume
            Assume.That(engineManager.IsEngineScheduledForShutdown, Is.False);

            // Act
            engineManager.ScheduleEngineShutdown();

            // Assert
            Assert.That(engineManager.IsEngineScheduledForShutdown, Is.True);
        }
    }
}