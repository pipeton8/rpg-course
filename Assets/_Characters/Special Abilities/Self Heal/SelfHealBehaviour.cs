using RPG.Core;

namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        public override void Use(AbilityUseParams useParams)
        {
            CureUser();
            PlayParticleEffect();
            PlaySoundEffect();
        }

        private void CureUser()
        {
            IDamageable user = GetComponent<IDamageable>();
            user.Heal((config as SelfHeal).GetCureAmount());
        }
    }
}
