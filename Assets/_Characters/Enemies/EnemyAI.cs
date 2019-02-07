using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
    [RequireComponent(typeof(WeaponSystem))]
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(HealthSystem))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 6f;

        GameObject player;
        WeaponSystem weaponSystem;
        HealthSystem healthSystem;
        Character character;
        float distanceToPlayer;

        void Start()
        {
            character = GetComponent<Character>();
            weaponSystem = GetComponent<WeaponSystem>();
            healthSystem = GetComponent<HealthSystem>();

            SetPlayerAsTarget();
            StartCoroutine(ChasePlayer());
        }

        void Update()
        {
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if (healthSystem.isDead || PlayerIsDead()) { GoIdle(); }
        }

        void SetPlayerAsTarget()
        {
            player = FindObjectOfType<PlayerControl>().gameObject;
            weaponSystem.SetTarget(player);
        }

        void GoIdle()
        {
            weaponSystem.StopAttacking();
            StopAllCoroutines();
        }

        IEnumerator ChasePlayer()
        {
            while (true)
            {
                if (PlayerIsDead()) { yield return null; }
                if (distanceToPlayer <= chaseRadius) { character.SetDestination(player.transform.position); }
                else { character.SetDestination(transform.position); }
                yield return null;
            }
        }

        bool PlayerIsDead() { return player.GetComponent<HealthSystem>().isDead; }

        void OnDrawGizmos()
        {
            // Draw chase sphere 
            Gizmos.color = new Color(0, 0, 255, .5f);
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }
    }
}