using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = "RPG/Weapon")]
    public class Weapon : ScriptableObject
    {
        public Transform gripTransform;

        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] AnimationClip attackAnimation = null;

        [SerializeField] float minTimeBetweenHits = .5f; // TODO consider wheter we take anumation time into account
        [SerializeField] float maxAttackRange = 2f;
        [SerializeField] float additionalDamage = 10f;

        public float GetMinTimeBetweenHits() { return minTimeBetweenHits; }

        public float GetMaxAttackRange() { return maxAttackRange; }

        public float GetAdditionalDamage() { return additionalDamage; }

        public GameObject GetWeapon() { return weaponPrefab; }

        public AnimationClip GetAnimClip()
        {
            RemoveAnimationEvents();
            return attackAnimation;
        }

        // So that asset packs don't cause crashes
        private void RemoveAnimationEvents()  {
            attackAnimation.events = new AnimationEvent[0];
        }
    }
}