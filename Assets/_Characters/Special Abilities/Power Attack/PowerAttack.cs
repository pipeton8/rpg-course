using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = "RPG/Special Ability/Power Attack")]
    public class PowerAttack : SpecialAbility
    {
        [Header("Power Attack Specific")]
        [SerializeField] float extraDamage = 2f;

        public override SpecialAbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<AreaEffectBehaviour>();
        }

        public float GetExtraDamage() { return extraDamage; }

    }
}