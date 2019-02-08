using System;
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

        enum State { idle, patrolling, attacking, chasing }
        [SerializeField] State state = State.idle;
        [SerializeField] WaypointContainer patrolPath = null;
        [SerializeField] float patrolWaitTime = 0.5f;

        GameObject player;
        WeaponSystem weaponSystem;
        HealthSystem healthSystem;
        Character character;
        float distanceToPlayer;

        void Start()
        {
            player = FindObjectOfType<PlayerControl>().gameObject;
            character = GetComponent<Character>();
            weaponSystem = GetComponent<WeaponSystem>();
            healthSystem = GetComponent<HealthSystem>();
        }

        void Update()
        {
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            EnemyBehaviour();
        }

        void EnemyBehaviour()
        {
            if (PlayerIsDead() || healthSystem.isDead)
            {
                StopAllCoroutines();
                IdleBehaviour();
                return;
            }
            if (distanceToPlayer > chaseRadius && state != State.patrolling)
            {
                PatrolBehaviour();
                return;
            }
            if (distanceToPlayer <= weaponSystem.attackRadius && state != State.attacking)
            {
                StopAllCoroutines();
                AttackBehaviour();
                return;
            }
            if (distanceToPlayer <= chaseRadius && state != State.chasing && state != State.attacking)
            {
                StopAllCoroutines();
                ChaseBehaviour();
                return;
            }
        }

        void IdleBehaviour()
        {
            state = State.idle;
            character.SetDestination(transform.position);
        }

        void PatrolBehaviour()
        {
            state = State.patrolling;
            StartCoroutine(FollowWaypoints());
        }

        void AttackBehaviour()
        {
            state = State.attacking;
            character.SetDestination(player.transform.position);
            weaponSystem.SetTarget(player);
        }

        void ChaseBehaviour() 
        {
            state = State.chasing;
            character.SetDestination(player.transform.position);
        }

        bool PlayerIsDead() { return player.GetComponent<HealthSystem>().isDead; }

        IEnumerator FollowWaypoints()
        {
            int waypointIndex = FindCloserWaypoint();
            while (patrolPath != null)
            {
                Transform waypoint = patrolPath.transform.GetChild(waypointIndex);
                character.SetDestination(waypoint.position);
                float distanceToWaypoint = Vector3.Distance(transform.position, waypoint.position);
                if (distanceToWaypoint <= character.stoppingDistance)
                {
                    waypointIndex = (waypointIndex + 1) % patrolPath.transform.childCount;
                    yield return new WaitForSeconds(patrolWaitTime);
                }
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }

        int FindCloserWaypoint()
        {
            float closerDistance = Mathf.Infinity;
            int closerIndex = 0;
            for (int waypointIndex = 0; waypointIndex < patrolPath.transform.childCount; waypointIndex++)
            {
                Transform waypoint = patrolPath.transform.GetChild(waypointIndex);
                float newDistance = Vector3.Distance(transform.position, waypoint.position);
                if (newDistance <= closerDistance)
                {
                    closerIndex = waypointIndex;
                    closerDistance = newDistance;
                }
            }
            return closerIndex;
        }

        void OnDrawGizmos()
        {
            // Draw chase sphere 
            Gizmos.color = new Color(0, 0, 255, .5f);
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }
    }
}