using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System.Linq;

namespace Events
{

    internal sealed class EventBus : MonoBehaviour
    {
        #region Singleton

        private static EventBus __instance = null;
        internal static EventBus Instance => __instance != null ? __instance : (__instance = new GameObject("EventBus").AddComponent<EventBus>()); //NOTE: This causes bad cleanup message.

        #endregion

        #region Listeners

        private Dictionary<System.Type, SimplePriorityQueue<EventCallback, Priority>> buses = new Dictionary<System.Type, SimplePriorityQueue<EventCallback, Priority>>();

        internal void AddCallbackStatic(IListener listener, System.Type eventType, System.Reflection.MethodInfo target, Priority priority)
        {
            SimplePriorityQueue<EventCallback, Priority> pq = buses.GetOrCreate(eventType);
            if(!pq.Any(h => h.owner == listener && h is StaticCallback s && s.target == target)) pq.Enqueue(new StaticCallback(eventType, listener, target), priority);
        }

        internal EventCallback AddCallbackDynamic<TEvent>(IListener listener, EventAPI.HandlerFunction<TEvent> target, Priority priority) where TEvent : AEvent
        {
            SimplePriorityQueue<EventCallback, Priority> pq = buses.GetOrCreate(typeof(TEvent));
            if (!pq.Any(h => h.owner == listener && h is DynamicCallback<TEvent> d && d.target == target))
            {
                EventCallback callback = new DynamicCallback<TEvent>(listener, target);
                pq.Enqueue(callback, priority);
                return callback;
            }
            return null;
        }

        internal void RemoveCallback(EventCallback callback)
        {
            buses[callback.eventType].Remove(callback);
        }

        internal void RemoveOwnedCallbacksOfType(IListener listener, System.Type eventType)
        {
            SimplePriorityQueue<EventCallback, Priority> bus = buses[eventType];
            List<EventCallback> toRemove = bus.Where(h => h.owner == listener).ToList();
            foreach (EventCallback h in toRemove) bus.Remove(h);
        }

        internal void RemoveAllOwnedCallbacks(IListener listener)
        {
            foreach (System.Type eventType in buses.Keys) RemoveOwnedCallbacksOfType(listener, eventType);
        }

        #endregion
    
        private void Update()
        {
            DispatchBufferedEvents();
        }

        #region Buffering system

        private Queue<AEvent> eventBuffer = new Queue<AEvent>();

        private void DispatchBufferedEvents()
        {
            while(eventBuffer.Count > 0)
            {
                DispatchImmediately(eventBuffer.Dequeue());
            }
        }

        #endregion

        #region Dispatching

        internal void DispatchAsync(AEvent @event)
        {
            if (!@event.HasBeenDispatched && !eventBuffer.Contains(@event)) eventBuffer.Enqueue(@event);
        }

        internal T DispatchImmediately<T>(T @event) where T : AEvent
        {
            if(@event.HasBeenDispatched) throw new System.InvalidOperationException("Event has already been dispatched!");

            @event.HasBeenDispatched = true;

            foreach (KeyValuePair<System.Type, SimplePriorityQueue<EventCallback, Priority>> pair in buses)
            {
                if (pair.Key.IsAssignableFrom(@event.GetType()))
                {
                    foreach (EventCallback i in pair.Value)
                    {
                        try
                        {
                            i.Dispatch(@event);
                        }
                        catch (System.Exception exc)
                        {
                            Debug.LogException(exc);
                        }
                    }
                }
            }

            return @event;
        }

        #endregion
    }

}