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

        public override void AttachComponentTo(GameObject gameObjectToAttachTo)
        {
            SelfHealBehaviour behaviourComponent = gameObjectToAttachTo.AddComponent<SelfHealBehaviour>();
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
        }

        public float GetCureAmount() { return amountToHeal; }
    }
}