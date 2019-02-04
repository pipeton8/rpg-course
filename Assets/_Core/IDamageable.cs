namespace RPG.Core
{
    public interface IDamageable
    {
        void TakeDamage(float damage);

        void Heal(float amount);

        bool IsDead();
    }
}