using System;
using UnityEngine;
using UnityEngine.AI;

using RPG.CameraUI;  // TODO consider re-wiring

namespace RPG.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] float movingTurnSpeed = 360f;
        [SerializeField] float stationaryTurnSpeed = 180f;
        [SerializeField] float stoppingDistance = 1.3f;
        [SerializeField] float moveSpeedMultiplier = 1.25f;
        [SerializeField] float animationSpeed = 1.25f;

        Animator animator;
        NavMeshAgent agent;
        Rigidbody rigidBody;
        float turnAmount;
        float forwardAmount;
        Vector3 clickPoint;

        void Start()
        {
            SetupRigidbody();
            SetupAnimator();
            SetNavMeshAgent();
            RegisterToCursor();
        }

        void Update()
        {
            HandleMovement();
        }

        void SetupRigidbody()
        {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        void SetupAnimator()
        {
            animator = GetComponent<Animator>();
            animator.speed = animationSpeed;
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

        void OnWalkableLayer(Vector3 destination)
        {
            if (GetComponent<Player>().IsDead()) { return; }
            if (Input.GetMouseButton(0)) { agent.SetDestination(destination); }
        }

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (GetComponent<Player>().IsDead()) { return; }

            if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(1))
            {
                agent.SetDestination(enemy.transform.position);
            }
        }

        void HandleMovement()
        {
            if (agent.remainingDistance > agent.stoppingDistance) { Move(agent.desiredVelocity); }
            else { Move(Vector3.zero); }
        }

        void Move(Vector3 move)
        {
            SetForwardAndTurn(move);
            UpdateAnimator();
            ApplyExtraTurnRotation();
        }

        void SetForwardAndTurn(Vector3 move)
        {
            move.Normalize();
            move = transform.InverseTransformDirection(move);
            turnAmount = Mathf.Atan2(move.x, move.z);
            forwardAmount = move.z;
        }

        void UpdateAnimator()
        {
            animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
        }

        void ApplyExtraTurnRotation()
        {
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        void OnAnimatorMove()
        {
            if (Time.deltaTime <= 0) { return; }
            Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;
            velocity.y = rigidBody.velocity.y;
            rigidBody.velocity = velocity;
        }

    }
}