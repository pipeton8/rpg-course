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
        [SerializeField] WaypointContainer patrolPath = null;
        [SerializeField] float patrolWaitTime = 0.5f;

        enum State { idle, patrolling, attacking, chasing }
        State state = State.idle;
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
            if (ChangeState())
            {
                StopAllCoroutines(); 
                UpdateEnemyBehaviour();
            }
        }

        private bool ChangeState()
        {
            bool isInAttackRing = distanceToPlayer <= weaponSystem.attackRadius;
            bool isInChaseRing = distanceToPlayer <= chaseRadius;
            if (PlayerIsDead() || healthSystem.isDead) { return ChangeIfDifferent(State.idle); }
            if (!isInAttackRing && !isInChaseRing) { return ChangeIfDifferent(State.patrolling); }
            if (isInAttackRing) { return ChangeIfDifferent(State.attacking); }
            if (!isInAttackRing && isInChaseRing) { return ChangeIfDifferent(State.chasing); }
            return false;
        }

        private bool ChangeIfDifferent(State newState)
        {
            if (state != newState) { state = newState;  return true; }
            return false;
        }

        void UpdateEnemyBehaviour()
        {
            switch (state)
            {
                case State.idle: IdleBehaviour(); break;
                case State.patrolling: PatrolBehaviour(); break;
                case State.attacking: AttackBehaviour(); break;
                case State.chasing: ChaseBehaviour(); break;
            }
        }

        void IdleBehaviour()
        {
            character.SetDestination(transform.position);
        }

        void PatrolBehaviour()
        {
            StartCoroutine(FollowWaypoints());
        }

        void AttackBehaviour()
        {
            StartCoroutine(ChasePlayer());
            weaponSystem.SetTarget(player);
        }

        void ChaseBehaviour() 
        {
            StartCoroutine(ChasePlayer());
        }

        bool PlayerIsDead() { return player.GetComponent<HealthSystem>().isDead; }

        IEnumerator ChasePlayer()
        {
            while (true)
            {
                if (distanceToPlayer <= chaseRadius) 
                {
                    character.SetDestination(player.transform.position);
                }
                yield return null;
            }
        }

        IEnumerator FollowWaypoints()
        {
            if (patrolPath == null)
            {
                character.SetDestination(transform.position);
                yield break;
            }
            int waypointIndex = FindCloserWaypoint();
            while (true)
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