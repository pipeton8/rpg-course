using UnityEngine;

using RPG.Core;

namespace RPG.Characters
{
    public class AreaEffectBehaviour : SpecialAbilityBehaviour
    {
        public override void Use(AbilityUseParams useParams)
        {
            DealRadialDamage(useParams);
            PlayParticleEffect();
            PlaySoundEffect();
        }

        void DealRadialDamage(AbilityUseParams useParams)
        {
            float totalDamage = useParams.baseDamage + (config as AreaEffect).GetDamage(); // TODO is this reasonable?
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, (config as AreaEffect).GetEffectRadius());

            foreach (Collider hitCollider in hitColliders)
            {
                GameObject target = hitCollider.gameObject;
                if (target != gameObject) { AttemptToDealDamage(target, totalDamage); }
            }
        }

        void AttemptToDealDamage(GameObject target, float damage)
        {
            var possibleTarget = target.GetComponent<HealthSystem>();
            if (possibleTarget != null) { possibleTarget.TakeDamage(damage); }
        }

        void OnDrawGizmos()
        {
            // Draw area sphere 
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(gameObject.transform.position, (config as AreaEffect).GetEffectRadius());
        }
    }
}
