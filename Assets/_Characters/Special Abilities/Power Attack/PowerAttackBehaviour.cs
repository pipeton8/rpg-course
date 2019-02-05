using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : AbilityBehaviour
    {
        PowerAttack config;

        public void SetConfig(PowerAttack configToSet)
        {
            config = configToSet;
        }

        public override void Use(AbilityUseParams useParams)
        {
            DealDamage(useParams);
            PlayParticleEffect();
        }

        void DealDamage(AbilityUseParams useParams)
        {
            float totalDamage = useParams.baseDamage + config.GetExtraDamage();
            useParams.target.TakeDamage(totalDamage);
        }

        void PlayParticleEffect()
        {
            GameObject particlePrefab = Instantiate(config.GetParticlePrefab(), transform);
            ParticleSystem particles = particlePrefab.GetComponent<ParticleSystem>();
            particles.Play();
            float destroyDelay = particles.main.duration + particles.main.startLifetime.constantMax;
            Destroy(particlePrefab, destroyDelay);
        }
    }
}
