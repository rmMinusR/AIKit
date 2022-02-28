namespace Combat
{
    //TODO fix coupling: requires on sequential dispatch. Remedied somewhat by API.

    public class HealQuery : CombatQuery
    {
        public readonly ICombatTarget target;
        public readonly ICombatEffect healingEffect;

        public readonly float originalHeal;
        public float heal;

        internal HealQuery(ICombatAffector source, ICombatTarget target, ICombatEffect healingEffect, float heal) : base(source)
        {
            this.target = target;
            this.healingEffect = healingEffect;

            this.originalHeal = heal;
            this.heal = heal;
        }
    }

    public class HealMessage : CombatMessage
    {
        public readonly ICombatTarget target;
        public readonly ICombatEffect healingEffect;
        public readonly float heal;

        internal HealMessage(ICombatAffector source, ICombatTarget target, ICombatEffect healingEffect, float heal) : base(source)
        {
            this.target = target;
            this.healingEffect = healingEffect;
            this.heal = heal;
        }
    }
}
