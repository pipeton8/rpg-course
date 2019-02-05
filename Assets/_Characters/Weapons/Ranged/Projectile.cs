using UnityEngine;

namespace RPG.Characters
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 10f;
        [SerializeField] GameObject shooter;

        float damage;

        public void SetDamage(float newDamage) { damage = newDamage; }

        public float GetSpeed() { return speed; }

        public void SetShooter(GameObject newShooter) { shooter = newShooter; }

        void OnCollisionEnter(Collision collision)
        {
            if (!shooter) { Destroy(gameObject); return; }

            if (collision.gameObject.layer != shooter.layer)
            {
                DamageIfDamageable(collision);
                Destroy(gameObject);
            }
        }

        private void DamageIfDamageable(Collision collision)
        {
            Component damageableComponent = collision.gameObject.GetComponent(typeof(HealthSystem));
            if (damageableComponent)  { (damageableComponent as HealthSystem).TakeDamage(damage); }
        }
    }
}
