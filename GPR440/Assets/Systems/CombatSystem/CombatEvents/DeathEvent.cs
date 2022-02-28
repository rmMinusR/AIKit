using System;

namespace Combat
{
    public class DeathMessage : CombatMessage
    {
        public readonly ICombatTarget whoDied;
        public readonly ICombatEffect damagingEffect;

        internal DeathMessage(ICombatAffector source, ICombatTarget whoDied, ICombatEffect damagingEffect) : base(source)
        {
            this.whoDied = whoDied;
            this.damagingEffect = damagingEffect;
        }
    }
}
