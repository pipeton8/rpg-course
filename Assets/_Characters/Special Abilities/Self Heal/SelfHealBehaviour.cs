using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;

namespace RPG.Characters
{
    public class SelfHealBehaviour : MonoBehaviour, ISpecialAbility
    {
        SelfHeal config;

        public void SetConfig(SelfHeal configToSet)
        {
            config = configToSet;
        }

        public void Use(AbilityUseParams useParams)
        {
            CureUser();
            PlayParticleEffect();
        }

        private void CureUser()
        {
            IDamageable user = GetComponent<IDamageable>();
            user.AdjustHealth(config.GetCureAmount());
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
