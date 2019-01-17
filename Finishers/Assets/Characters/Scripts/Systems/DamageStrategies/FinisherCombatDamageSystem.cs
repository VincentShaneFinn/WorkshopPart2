namespace Finisher.Characters.Systems.Strategies
{
    public class FinisherCombatDamageSystem : FinisherModeDamageSystem
    {
        public override void Hit(HealthSystem target)
        {
            if (target)
            {
                DealDamage(target);
                DealKnockback(target);
                DealVolatilityDamage(target);
            }
        }
    }
}