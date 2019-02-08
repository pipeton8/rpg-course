using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : SpecialAbilityBehaviour
    {
        float delayToParticleEffect = 0.1f;

        public override void Use(GameObject target)
        {
            CureUser();
            StartCoroutine(PlayAbilityEffects(delayToParticleEffect));
        }

        private void CureUser()
        {
            HealthSystem userHealthSystem = GetComponent<HealthSystem>();
            userHealthSystem.Heal((config as SelfHeal).GetCureAmount());
        }
    }
}
