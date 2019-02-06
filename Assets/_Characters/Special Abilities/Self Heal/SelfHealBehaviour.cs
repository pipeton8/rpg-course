using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : SpecialAbilityBehaviour
    {
        public override void Use(GameObject target)
        {
            CureUser();
            PlayParticleEffect();
            PlaySoundEffect();
        }

        private void CureUser()
        {
            HealthSystem userHealthSystem = GetComponent<HealthSystem>();
            userHealthSystem.Heal((config as SelfHeal).GetCureAmount());
        }
    }
}
