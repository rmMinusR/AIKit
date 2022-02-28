using Events;

namespace Combat
{
    internal interface ICombatEvent
    {
    }

    public abstract class CombatQuery : Query, ICombatEvent
    {
        public ICombatAffector source;

        protected internal CombatQuery(ICombatAffector source)
        {
            this.source = source;
        }
    }

    public abstract class CombatMessage : Message, ICombatEvent
    {
        public ICombatAffector source;

        protected internal CombatMessage(ICombatAffector source)
        {
            this.source = source;
        }
    }
}
