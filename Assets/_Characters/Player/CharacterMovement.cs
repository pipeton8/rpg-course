using System;
using UnityEngine;
using UnityEngine.AI;

using RPG.CameraUI;  // TODO consider re-wiring

namespace RPG.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] float stoppingDistance = 1f;
        [SerializeField] float moveSpeedMultiplier = 1f;
        // TODO consider animationSpeedMultiplier

        Animator animator;
        Rigidbody characterRigidbody;
        ThirdPersonCharacter character = null;   // A reference to the ThirdPersonCharacter on the object
        Vector3 clickPoint;
        GameObject walkTarget;
        NavMeshAgent agent = null;

        void Start()
        {
            character = GetComponent<ThirdPersonCharacter>();
            characterRigidbody = GetComponent<Rigidbody>();
            walkTarget = new GameObject("walkTarget");

            SetupAnimator();
            SetNavMeshAgent();
            RegisterToCursor();
        }

        void Update()
        {
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                character.Move(agent.desiredVelocity);
            }
            else
            {
                character.Move(Vector3.zero);
            }
        }

        void SetupAnimator()
        {
            animator = GetComponent<Animator>();
            //animator.speed = moveSpeedMultiplier;
        }


        void SetNavMeshAgent()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updatePosition = true;
            agent.stoppingDistance = stoppingDistance;
        }

        void RegisterToCursor()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.onMouseOverWalkable += OnWalkableLayer;
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (CanMove()) { return; }

            if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(1))
            {
                agent.SetDestination(enemy.transform.position);
            }
        }

        void OnWalkableLayer(Vector3 destination)
        {
            if (CanMove()) { return; }

            if (Input.GetMouseButton(0))
            {
                agent.SetDestination(destination);
            }
        }

        bool CanMove() { return GetComponent<Player>().IsDead(); }

        void OnAnimatorMove()
        {
            if (Time.deltaTime > 0)
            {
                Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;
                velocity.y = characterRigidbody.velocity.y;
                characterRigidbody.velocity = velocity;
            }
        }

    }
}