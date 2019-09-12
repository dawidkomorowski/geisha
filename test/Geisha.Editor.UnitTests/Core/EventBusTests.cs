using Geisha.Editor.Core;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.Core
{
    [TestFixture]
    public class EventBusTests
    {
        [Test]
        public void SendEvent_ShouldExecuteRegisteredEventHandler()
        {
            // Arrange
            var eventBus = new EventBus();

            TestEvent eventReceivedInHandler = null;
            eventBus.RegisterEventHandler<TestEvent>(e => eventReceivedInHandler = e);

            var @event = new TestEvent();

            // Act
            eventBus.SendEvent(@event);

            // Assert
            Assert.That(eventReceivedInHandler, Is.EqualTo(@event));
        }

        private sealed class TestEvent : IEvent
        {
        }
    }
}