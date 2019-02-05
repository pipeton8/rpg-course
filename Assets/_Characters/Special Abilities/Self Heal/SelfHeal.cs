using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = "RPG/Special Ability/Self Heal")]
    public class SelfHeal : SpecialAbility
    {
        [Header("Self Heal Specific")]
        [SerializeField] float amountToHeal = 20f;

        public override SpecialAbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<AreaEffectBehaviour>();
        }

        public float GetCureAmount() { return amountToHeal; }
    }
}