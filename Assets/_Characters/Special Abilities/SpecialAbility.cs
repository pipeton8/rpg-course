﻿using UnityEngine;

using RPG.Core;

namespace RPG.Characters
{
    public struct AbilityUseParams
    {
        public IDamageable target;
        public float baseDamage;

        public AbilityUseParams(IDamageable newTarget, float damage)
        {
            target = newTarget;
            baseDamage = damage;
        }
    }

    public abstract class SpecialAbility : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;
        [SerializeField] GameObject particlePrefab = null;
        [SerializeField] AudioClip[] soundEffects = null;

        protected AbilityBehaviour behaviour;

        public abstract void AttachComponentTo(GameObject gameObjectToAttachTo);

        public float GetEnergyCost() { return energyCost; }

        public GameObject GetParticlePrefab() { return particlePrefab; }

        public AudioClip GetRandomSoundEffect() 
        {
            return soundEffects[Random.Range(0, soundEffects.Length)];
        }

        public void Use(AbilityUseParams useParams) { behaviour.Use(useParams); }
    }
}