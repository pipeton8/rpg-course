using UnityEngine;

using RPG.Core;

namespace RPG.Characters
{
    public struct AbilityUseParams
    {
        public GameObject user;
        public IDamageable target;
        public float baseDamage;

        public AbilityUseParams(GameObject newUser, IDamageable newTarget, float damage)
        {
            user = newUser;
            target = newTarget;
            baseDamage = damage;
        }
    }

    public abstract class SpecialAbility : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;

        protected ISpecialAbility behaviour;

        public abstract void AttachComponentTo(GameObject gameObjectToAttachTo);

        public float GetEnergyCost() { return energyCost; }

        public void Use(AbilityUseParams useParams) { behaviour.Use(useParams); }
    }

    public interface ISpecialAbility
    {
        void Use(AbilityUseParams useParams);
    }
}