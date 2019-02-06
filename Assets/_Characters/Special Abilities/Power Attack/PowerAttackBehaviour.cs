using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : SpecialAbilityBehaviour
    {
        public override void Use(GameObject target)
        {
            DealDamage(target);
            PlayParticleEffect();
            PlaySoundEffect();
        }

        void DealDamage(GameObject target)
        {
            float totalDamage = (config as PowerAttack).GetDamage();
            target.GetComponent<HealthSystem>().TakeDamage(totalDamage);
        }
    }
}
