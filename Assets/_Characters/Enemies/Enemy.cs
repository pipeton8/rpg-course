using System.Collections;
using UnityEngine;

using RPG.Core; // TODO consider re-wire
using RPG.Weapons;

namespace RPG.Characters
{
    public class Enemy : MonoBehaviour, IDamageable
    {

        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float chaseRadius = 6f;

        [SerializeField] float attackRadius = 4f;
        [SerializeField] float damagePerShot = 9f;
        [SerializeField] float secondsBetweenShots = 0.5f;
        [SerializeField] float variation = 0.1f;
        [SerializeField] GameObject projectileToUse = null;
        [SerializeField] GameObject projectileSocket = null;
        [SerializeField] Vector3 aimOffset = new Vector3(0, 1f, 0);

        bool isAttacking = false;
        float currentHealthPoints;
        AICharacterControl aiCharacterControl = null;
        GameObject player = null;

        public bool IsDead() { return currentHealthPoints <= 0; }

        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        public void AdjustHealth(float healthChange)
        {
            if (IsDead()) { return; }
            ChangeHealthPoints(healthChange);
            if (IsDead()) { Destroy(gameObject); }
        }

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            aiCharacterControl = GetComponent<AICharacterControl>();
            currentHealthPoints = maxHealthPoints;
        }

        void Update()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            AttackProtocol(distanceToPlayer);
            ChasePlayer(distanceToPlayer);
        }

        void ChasePlayer(float distanceToPlayer)
        {
            if (distanceToPlayer <= chaseRadius)
            {
                aiCharacterControl.SetTarget(player.transform);
                return;
            }
            aiCharacterControl.SetTarget(transform);
        }

        void AttackProtocol(float distanceToPlayer)
        {
            if (PlayerIsDead()) { StopAttacking(); return; }
            bool closeEnoughToAttack = distanceToPlayer <= attackRadius;
            if (closeEnoughToAttack && !isAttacking) { StartAttacking(); }
            if (!closeEnoughToAttack) { StopAttacking(); }
        }

        void StartAttacking() { isAttacking = true; StartCoroutine(SpawnProjectile()); }

        void StopAttacking() { isAttacking = false; StopAllCoroutines(); }

        bool PlayerIsDead() { return player.GetComponent<IDamageable>().IsDead(); }

        void ChangeHealthPoints(float healthChange)
        {
            float newHealthPoints = currentHealthPoints + healthChange;
            currentHealthPoints = Mathf.Clamp(newHealthPoints, 0f, maxHealthPoints);
        }

        // TODO separate our character firing logic (in different class)
        IEnumerator SpawnProjectile()
        {
            while (true)
            {
                GameObject newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.identity);
                Projectile projectileComponent = newProjectile.GetComponent<Projectile>();
                projectileComponent.SetDamage(damagePerShot);
                projectileComponent.SetShooter(gameObject);

                Vector3 unitVectorToPlayer = (player.transform.position + aimOffset - projectileSocket.transform.position).normalized;
                float projectileSpeed = projectileComponent.GetSpeed();
                newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;

                float waitTime = Random.Range(secondsBetweenShots - variation, secondsBetweenShots + variation);
                yield return new WaitForSeconds(waitTime);
            }
        }

        void OnDrawGizmos()
        {
            // Draw attack sphere 
            Gizmos.color = new Color(255f, 0, 0, .5f);
            Gizmos.DrawWireSphere(transform.position, attackRadius);

            // Draw chase sphere 
            Gizmos.color = new Color(0, 0, 255, .5f);
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }
    }
}