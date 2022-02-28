using Events;

namespace Combat
{
    public static class CombatAPI
    {
        /// <summary>
        /// Deals damage, but may do more in future, such as crowd control and status effects.
        /// </summary>
        public static void Hit(ICombatAffector from, ICombatTarget to, ICombatEffect how, float damage)
        {
            HitQuery query = new HitQuery(from, to, how, damage);
            EventAPI.Dispatch(query);

            HitMessage message = new HitMessage(from, to, how, query.damage);
            EventAPI.Dispatch(message);

            if (!message.isCancelled) to.DirectApplyDamage(query.damage, from, how);
        }

        /// <summary>
        /// Heals the target.
        /// </summary>
        public static void Heal(ICombatAffector from, ICombatTarget to, ICombatEffect how, float heal)
        {
            HealQuery query = new HealQuery(from, to, how, heal);
            EventAPI.Dispatch(query);

            HealMessage message = new HealMessage(from, to, how, query.heal);
            EventAPI.Dispatch(message);

            if (!message.isCancelled) to.DirectApplyHeal(query.heal, from, how);
        }

        /// <summary>
        /// Instantly kills the target, no matter what its current health is.
        /// 
        /// This function is also called when a CombatantEntity's health reaches zero,
        /// which allows other effects to be triggered or for death to be cancelled altogether.
        /// </summary>
        public static void Kill(ICombatTarget target, ICombatAffector killer, ICombatEffect how)
        {
            DeathMessage message = new DeathMessage(killer, target, how);
            EventAPI.Dispatch(message);

            if (!message.isCancelled) target.DirectKill(killer, how);
        }
    }
}
