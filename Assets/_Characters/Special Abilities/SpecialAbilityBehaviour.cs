﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class SpecialAbilityBehaviour : MonoBehaviour
    {
        const float PARTICLE_CLEANUP_DELAY = 20f;

        protected SpecialAbility config;

        public abstract void Use(GameObject target = null);

        public void SetConfig(SpecialAbility configToSet) { config = configToSet; }

        protected void PlayParticleEffect()
        {
            GameObject particleObject = Instantiate(config.GetParticlePrefab(), transform);
            ParticleSystem particles = particleObject.GetComponent<ParticleSystem>();
            particles.Play();
            float destroyDelay = particles.main.duration + particles.main.startLifetime.constantMax;
            StartCoroutine(DestroyParticleObject(particleObject));
        }

        protected void PlaySoundEffect()
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(config.GetRandomSoundEffect());
        }

        IEnumerator DestroyParticleObject(GameObject particleObject)
        {
            while (particleObject.GetComponent<ParticleSystem>().isPlaying)
            {
                yield return new WaitForSeconds(PARTICLE_CLEANUP_DELAY);
            }
            Destroy(particleObject);
            yield return null;
        }
    }
}


