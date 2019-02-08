using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class SpecialAbilityBehaviour : MonoBehaviour
    {
        const float PARTICLE_CLEANUP_DELAY = 20f;
        const string SPECIAL_ABILITY_TRIGGER = "Special Ability";
        const string DEFAULT_SPECIAL_ABILITY = "DEFAULT_SPECIAL_ABILITY";

        protected SpecialAbility config;

        public abstract void Use(GameObject target);

        public void SetConfig(SpecialAbility configToSet) { config = configToSet; }

        protected IEnumerator PlayAbilityEffects(float timeDelayAfterAnimation)
        {
            PlayAnimationClip();
            yield return new WaitForSeconds(timeDelayAfterAnimation);
            PlayParticleEffect();
            PlaySoundEffect();
        }

        void PlayParticleEffect()
        {
            GameObject particleObject = Instantiate(config.GetParticlePrefab(), transform);
            ParticleSystem particles = particleObject.GetComponent<ParticleSystem>();
            particles.Play();
            float destroyDelay = particles.main.duration + particles.main.startLifetime.constantMax;
            StartCoroutine(DestroyParticleObject(particleObject));
        }

        void PlayAnimationClip()
        {
            Character character = GetComponent<Character>();
            Animator animator = GetComponent<Animator>();
            AnimationClip abilityAnimation = config.GetAnimationClip();
            abilityAnimation.events = new AnimationEvent[0];
            character.runtimeAnimatorController[DEFAULT_SPECIAL_ABILITY] = abilityAnimation;
            animator.SetTrigger(SPECIAL_ABILITY_TRIGGER);
        }

        void PlaySoundEffect()
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


