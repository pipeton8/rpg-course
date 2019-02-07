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

        public float timeOfHit { get { return hitTime; } }

        public float GetMinTimeBetweenHits() { return minTimeBetweenHits; }

        public float GetMaxAttackRange() { return maxAttackRange; }

        public float GetAdditionalDamage() { return additionalDamage; }

        public GameObject GetWeapon() { return weaponPrefab; }

        public AnimationClip GetAnimClip()
        {
            if (attackAnimation.events.Length > 0) { hitTime = attackAnimation.events[0].time; }
            RemoveAnimationEvents();
            return attackAnimation;
        }

        private void RemoveAnimationEvents()
        {
            attackAnimation.events = new AnimationEvent[0];
        }
    }
}