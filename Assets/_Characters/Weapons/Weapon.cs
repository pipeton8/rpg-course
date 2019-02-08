using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = "RPG/Weapon")]
    public class Weapon : ScriptableObject
    {
        public Transform gripTransform;

        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] AnimationClip attackAnimation = null;

        [SerializeField] float minTimeBetweenHits = .5f;
        [SerializeField] float maxAttackRange = 2f;
        [SerializeField] float additionalDamage = 10f;
        [SerializeField] float timeOfHit = 2f;

        public float GetMinTimeBetweenHits() { return minTimeBetweenHits; }

        public float GetMaxAttackRange() { return maxAttackRange; }

        public float GetAdditionalDamage() { return additionalDamage; }

        public float GetTimeOfHit() { return timeOfHit; }

        public GameObject GetWeapon() { return weaponPrefab; }

        public AnimationClip GetAnimClip()
        {
            RemoveAnimationEvents();
            return attackAnimation;
        }

        private void RemoveAnimationEvents()
        {
            attackAnimation.events = new AnimationEvent[0];
        }
    }
}