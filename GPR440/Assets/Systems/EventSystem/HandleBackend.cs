using Events;
using System;
using System.Reflection;

namespace Events
{
    public abstract class EventCallback
    {
        public readonly Type eventType;
        public readonly IListener owner;
        public EventCallback(Type eventType, IListener owner)
        {
            this.eventType = eventType;
            this.owner = owner;
        }

        internal abstract void Dispatch(AEvent e);
    }

    internal sealed class StaticCallback : EventCallback
    {
        public StaticCallback(Type eventType, IListener owner, MethodInfo target) : base(eventType, owner)
        {
            this.target = target;
        }

        public readonly MethodInfo target;

        internal override void Dispatch(AEvent e) => target.Invoke(owner, new object[] { e });
    }

    internal sealed class DynamicCallback<TEvent> : EventCallback where TEvent : AEvent
    {
        public DynamicCallback(IListener owner, EventAPI.HandlerFunction<TEvent> target) : base(typeof(TEvent), owner)
        {
            this.target = target;
        }

        public readonly EventAPI.HandlerFunction<TEvent> target;

        internal override void Dispatch(AEvent e) => target((TEvent)e);
    }

}