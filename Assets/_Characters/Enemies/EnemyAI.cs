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
            CheckForNewState();
            EnemyBehaviour();
        }

        void EnemyBehaviour()
        {
            switch (state)
            {
                case State.idle: IdleBehaviour(); break;
                case State.patrolling: PatrolBehaviour(); break;
                case State.attacking: AttackBehaviour(); break;
                case State.chasing: ChaseBehaviour(); break;
            }
        }

        void CheckForNewState()
        {
            if (PlayerIsDead() || healthSystem.isDead) { state = State.idle; return; }
            if (distanceToPlayer > chaseRadius) { state = State.patrolling; return; }
            if (distanceToPlayer <= weaponSystem.attackRadius) { state = State.attacking; return; }
            if (distanceToPlayer <= chaseRadius) { state = State.chasing; return; }
        }

        void ChaseBehaviour() 
        {
            character.SetDestination(player.transform.position);
        }

        void AttackBehaviour()
        {
            character.SetDestination(player.transform.position);
            weaponSystem.SetTarget(player);
        }

        void PatrolBehaviour()
        {
            character.SetDestination(transform.position);
            print("I should be patrolling but the programmer is to lazy to do that now");
        }

        void IdleBehaviour()
        {
            character.SetDestination(transform.position);
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