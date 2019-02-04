using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;
using System;

namespace RPG.Characters
{
    public class AreaEffectBehaviour : MonoBehaviour, ISpecialAbility
    {
        AreaEffect config;

        public void SetConfig(AreaEffect configToSet)
        {
            config = configToSet;
        }

        public void Use(AbilityUseParams useParams)
        {
            float totalDamage = useParams.baseDamage + config.GetDamage(); // TODO is this reasonable?
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, config.GetEffectRadius());

            foreach (Collider hitCollider in hitColliders)
            {
                GameObject target = hitCollider.gameObject;
                if (target != useParams.user)
                {
                    AttemptToDealDamage(target, totalDamage);
                }
            }
        }

        void AttemptToDealDamage(GameObject target, float damage)
        {
            var possibleTarget = target.GetComponent<IDamageable>();
            if (possibleTarget != null) { possibleTarget.TakeDamage(damage); }
        }

        void OnDrawGizmos()
        {
            // Draw area sphere 
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(gameObject.transform.position, config.GetEffectRadius());
        }
    }
}
