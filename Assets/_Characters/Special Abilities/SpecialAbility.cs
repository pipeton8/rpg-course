using UnityEngine;

using RPG.Core;

namespace RPG.Characters
{
    public abstract class SpecialAbility : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;
        [SerializeField] GameObject particlePrefab = null;
        [SerializeField] AudioClip[] soundEffects = null;

        protected SpecialAbilityBehaviour behaviour;

        public abstract SpecialAbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo);

        public void AttachAbilityTo(GameObject objectToAttachTo)
        {
            SpecialAbilityBehaviour behaviourComponent = GetBehaviourComponent(objectToAttachTo);
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
        }

        public float GetEnergyCost() { return energyCost; }

        public GameObject GetParticlePrefab() { return particlePrefab; }

        public AudioClip GetRandomSoundEffect() 
        {
            return soundEffects[Random.Range(0, soundEffects.Length)];
        }

        public void Use(GameObject target) { behaviour.Use(target); }
    }
}