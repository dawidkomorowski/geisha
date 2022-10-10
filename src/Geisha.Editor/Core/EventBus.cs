using System;
using System.Collections.Generic;
using NLog;

namespace Geisha.Editor.Core
{
    public interface IEvent
    {
    }

    public interface IEventBus
    {
        void RegisterEventHandler<TEvent>(Action<TEvent> handler) where TEvent : IEvent;
        void SendEvent<TEvent>(TEvent @event) where TEvent : IEvent;
    }

    public sealed class EventBus : IEventBus
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly EventBus DefaultInstance = new();
        private readonly Dictionary<Type, object> _eventHandlers = new();

        public static IEventBus Default => DefaultInstance;

        public void RegisterEventHandler<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            _eventHandlers.Add(typeof(TEvent), handler);
        }

        public void SendEvent<TEvent>(TEvent @event) where TEvent : IEvent
        {
            Logger.Debug("Event sent: {0}.", typeof(TEvent));
            if (_eventHandlers.TryGetValue(typeof(TEvent), out var eventHandler))
            {
                ((Action<TEvent>)eventHandler).Invoke(@event);
            }
            else
            {
                Logger.Warn("No event handler registered for event of type {0}.", typeof(TEvent));
            }
        }
    }
}