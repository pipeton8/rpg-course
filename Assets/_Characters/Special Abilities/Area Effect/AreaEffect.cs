using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = "RPG/Special Ability/Area Effect")]
    public class AreaEffect : SpecialAbility
    {
        [Header("Area Effect Specific")]
        [SerializeField] float damageToEachTarget = 20f;
        [SerializeField] float effectRadius = 5f;

        public override SpecialAbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<AreaEffectBehaviour>();
        }

        public float GetDamage() { return damageToEachTarget; }

        public float GetEffectRadius() { return effectRadius; }

    }
}