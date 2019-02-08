using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : SpecialAbilityBehaviour
    {
        float delayToParticleEffect = 0.5f;

        public override void Use(GameObject target)
        {
            DealDamage(target);
            StartCoroutine(PlayAbilityEffects(delayToParticleEffect));
        }

        void DealDamage(GameObject target)
        {
            float totalDamage = (config as PowerAttack).GetDamage();
            target.GetComponent<HealthSystem>().TakeDamage(totalDamage);
        }
    }
}
