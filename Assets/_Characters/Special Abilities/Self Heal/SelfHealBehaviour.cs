using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;
using System;

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
            PlaySoundEffect();
        }

        private void PlaySoundEffect()
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(config.GetSoundEffect());
        }

        private void CureUser()
        {
            IDamageable user = GetComponent<IDamageable>();
            user.Heal(config.GetCureAmount());
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
