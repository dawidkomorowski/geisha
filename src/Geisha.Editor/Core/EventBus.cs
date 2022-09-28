using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Logging;

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
        private static readonly ILog Log = LogFactory.Create(typeof(EventBus));
        private static readonly EventBus DefaultInstance = new();
        private readonly Dictionary<Type, object> _eventHandlers = new();

        public static IEventBus Default => DefaultInstance;

        public void RegisterEventHandler<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            _eventHandlers.Add(typeof(TEvent), handler);
        }

        public void SendEvent<TEvent>(TEvent @event) where TEvent : IEvent
        {
            Log.Debug($"Event sent: {typeof(TEvent)}.");
            if (_eventHandlers.TryGetValue(typeof(TEvent), out var eventHandler))
            {
                ((Action<TEvent>)eventHandler).Invoke(@event);
            }
            else
            {
                Log.Warn($"No event handler registered for event of type {typeof(TEvent)}.");
            }
        }
    }
}