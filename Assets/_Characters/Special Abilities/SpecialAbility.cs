using UnityEngine;

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

        protected ISpecialAbility behaviour;

        public abstract void AttachComponentTo(GameObject gameObjectToAttachTo);

        public float GetEnergyCost() { return energyCost; }

        public GameObject GetParticlePrefab() { return particlePrefab; }

        public void Use(AbilityUseParams useParams) { behaviour.Use(useParams); }
    }

    public interface ISpecialAbility
    {
        void Use(AbilityUseParams useParams);
    }
}