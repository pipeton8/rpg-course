using UnityEngine;

namespace RPG.Characters
{
    public class AreaEffectBehaviour : SpecialAbilityBehaviour
    {
        float timeDelayAfterAnimation = 0.5f;

        public override void Use(GameObject target)
        {
            DealRadialDamage();
            StartCoroutine(PlayAbilityEffects(timeDelayAfterAnimation));
        }

        void DealRadialDamage()
        {
            float totalDamage = (config as AreaEffect).GetDamage();
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
