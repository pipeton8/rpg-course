namespace RPG.Characters
{
    public class PowerAttackBehaviour : SpecialAbilityBehaviour
    {
        public override void Use(AbilityUseParams useParams)
        {
            DealDamage(useParams);
            PlayParticleEffect();
            PlaySoundEffect();
        }

        void DealDamage(AbilityUseParams useParams)
        {
            float totalDamage = useParams.baseDamage + (config as PowerAttack).GetExtraDamage();
            useParams.target.TakeDamage(totalDamage);
        }
    }
}
