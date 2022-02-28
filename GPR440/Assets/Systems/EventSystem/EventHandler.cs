using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Events
{

    /// <summary>
    /// Static event handler marker
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class EventHandlerAttribute : Attribute
    {
        public Priority priority;

        public EventHandlerAttribute(Priority priority)
        {
            this.priority = priority;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class QueryHandlerAttribute : EventHandlerAttribute
    {
        public QueryHandlerAttribute(Priority priority) : base(priority) { }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MessageHandlerAttribute : EventHandlerAttribute
    {
        public MessageHandlerAttribute(Priority priority) : base(priority) { }
    }


    internal static class StaticHandlerCache
    {
        /*

        private static Dictionary<Type, HashSet<Record>> cache = new Dictionary<Type, HashSet<Record>>();

        private static HashSet<Record> GetOrScan(IListener listener)
        {
            Type listenerType = listener.GetType();
            
            //Try to fetch existing, if it exists
            HashSet<Record> records = null;
            if(!cache.TryGetValue(listenerType, out records))
            {
                //If it doesn't exist yet, default
                records = StaticScan(listener);
                cache.Add(listenerType, records);
            }

            return records;
        }

        internal static IEnumerable<MethodInfo> GetHandlers(IListener listener)
        {
            HashSet<Record> records = GetOrScan(listener);

            return records.Select(r => r.callInfo);
        }

        internal static IEnumerable<MethodInfo> GetHandlers(IListener listener, AEvent @event)
        {
            HashSet<Record> records = GetOrScan(listener);

            Type eventType = @event.GetType();
            return records.Where(r => r.eventType.IsAssignableFrom(@eventType)).Select(r => r.callInfo);
        }
        */

        public struct Record
        {
            public Type eventType;
            public MethodInfo callInfo;
            public Priority priority;
        }

        public static HashSet<Record> StaticScan(IListener listener)
        {
            HashSet<Record> records = new HashSet<Record>();

            foreach(MethodInfo i in listener.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                EventHandlerAttribute attr = i.GetCustomAttribute<EventHandlerAttribute>();
                if (attr != null)
                {

                    if (i.ReturnType == typeof(void)
                     && i.GetParameters().Length == 1
                     && typeof(AEvent).IsAssignableFrom(i.GetParameters()[0].ParameterType))
                    {
                        records.Add(new Record
                        {
                            callInfo = i,
                            eventType = i.GetParameters()[0].ParameterType,
                            priority = attr.priority
                        });
                    } else UnityEngine.Debug.LogError("Invalid target for EventHandler: "+listener.GetType().Name+"."+i.Name);

                }
            }

            UnityEngine.Debug.Log("Scanned "+listener.GetType().Name+". Found "+records.Count+" handlers.");
            return records;
        }
    }

}