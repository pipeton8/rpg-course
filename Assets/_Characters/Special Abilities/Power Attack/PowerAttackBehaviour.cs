using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility
    {
        PowerAttack config;

        public void SetConfig(PowerAttack configToSet)
        {
            config = configToSet;
        }

        public void Use(AbilityUseParams useParams)
        {
            DealDamage(useParams);
            PlayParticleEffect();
        }

        void DealDamage(AbilityUseParams useParams)
        {
            float totalDamage = useParams.baseDamage + config.GetExtraDamage();
            useParams.target.AdjustHealth(totalDamage);
        }

        void PlayParticleEffect()
        {
            GameObject particlePrefab = Instantiate(config.GetParticlePrefab(), transform);
            ParticleSystem particles = particlePrefab.GetComponent<ParticleSystem>();
            particles.Play();
            Destroy(particlePrefab, particles.main.duration);
        }
    }
}
