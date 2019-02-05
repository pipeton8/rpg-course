using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;
using System;

namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        public override void Use(AbilityUseParams useParams)
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
            user.Heal((config as SelfHeal).GetCureAmount());
        }
    }
}
