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

        public void SetConfig(AreaEffect configToSet) { config = configToSet; }

        public void Use(AbilityUseParams useParams)
        {
            DealRadialDamage(useParams);
            PlayParticleEffect();
            PlaySoundEffect();
        }

        void DealRadialDamage(AbilityUseParams useParams)
        {
            float totalDamage = useParams.baseDamage + config.GetDamage(); // TODO is this reasonable?
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, config.GetEffectRadius());

            foreach (Collider hitCollider in hitColliders)
            {
                GameObject target = hitCollider.gameObject;
                if (target != gameObject) { AttemptToDealDamage(target, totalDamage); }
            }
        }

        void PlayParticleEffect()
        {
            GameObject particlePrefab = Instantiate(config.GetParticlePrefab(), transform);
            ParticleSystem particles = particlePrefab.GetComponent<ParticleSystem>();
            particles.Play();
            float destroyDelay = particles.main.duration + particles.main.startLifetime.constantMax;
            Destroy(particlePrefab, destroyDelay);
        }

        private void PlaySoundEffect()
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(config.GetSoundEffect());
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
