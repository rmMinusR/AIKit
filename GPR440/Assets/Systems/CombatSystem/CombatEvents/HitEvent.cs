namespace Combat
{
    //TODO fix coupling: requires on sequential dispatch. Remedied somewhat by API.

    public class HitQuery : CombatQuery
    {
        public readonly ICombatTarget target;
        public readonly ICombatEffect damagingEffect;

        public readonly float originalDamage;
        public float damage;

        internal HitQuery(ICombatAffector source, ICombatTarget target, ICombatEffect damagingEffect, float damage) : base(source)
        {
            this.target = target;
            this.damagingEffect = damagingEffect;

            this.originalDamage = damage;
            this.damage = damage;
        }
    }

    public class HitMessage : CombatMessage
    {
        public readonly ICombatTarget target;
        public readonly ICombatEffect damagingEffect;
        public readonly float damage;

        internal HitMessage(ICombatAffector source, ICombatTarget target, ICombatEffect damagingEffect, float damage) : base(source)
        {
            this.target = target;
            this.damagingEffect = damagingEffect;
            this.damage = damage;
        }
    }
}
