namespace Events
{
    /// <summary>
    /// Highest priority will execute first and Lowest will execute last.
    /// </summary>
    public enum Priority
    {
        Highest = 0,
        High = 1,
        Normal = 2,
        Low = 3,
        Lowest = 4
    }

    /// <summary>
    /// Internal class. Should never be used outside Event module.
    /// </summary>
    public abstract class AEvent
    {
        protected AEvent()
        {
            HasBeenDispatched = false;
        }

        public bool HasBeenDispatched { get; internal set; }
    }

    /// <summary>
    /// Queries are always executed IMMEDIATELY, and are guaranteed to have a valid
    /// result before the Dispatch() function exits.
    /// 
    /// Variables that listeners should modify should be accompanied by a readonly
    /// 'original' value to keep calculations consistent. However, due to overhead,
    /// they should be used sparingly. They are used to poll for updates such as
    /// calculating modified damage or movement.
    /// </summary>
    public abstract class Query : AEvent
    {
    }

    /// <summary>
    /// Messages do not execute immediately, but at some unspecified time in the
    /// near future when it is convenient to do so.
    /// 
    /// Their data is read only, but they can interacted with by being cancelled.
    /// Note that an event will reach all listeners, whether it was cancelled or not.
    /// </summary>
    public abstract class Message : AEvent
    {
        public bool isCancelled;

        protected Message() : base()
        {
            isCancelled = false;
        }
    }
}