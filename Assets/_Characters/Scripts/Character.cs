using UnityEngine;
using UnityEngine.AI;

using RPG.CameraUI;  // TODO consider re-wiring
using System;

namespace RPG.Characters
{
    [SelectionBase]
    public class Character : MonoBehaviour
    {
        [Header("Rigidbody Settings")]
        [SerializeField] CollisionDetectionMode collisionDetectionMode;

        [Header("Capsule Collider Settings")]
        [SerializeField] Vector3 colliderCenter = new Vector3(0, 0.9f, 0);
        [SerializeField] float colliderRadius = 0.2f;
        [SerializeField] float colliderHeight = 1.85f;

        [Header("Animator Settings")]
        [SerializeField] RuntimeAnimatorController animatorController = null;
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        [SerializeField] Avatar characterAvatar = null;

        [Header("Movement Properties")]
        [SerializeField] float movingTurnSpeed = 360f;
        [SerializeField] float stationaryTurnSpeed = 180f;
        [SerializeField] float moveSpeedMultiplier = 1.25f;
        [SerializeField] float animationSpeed = 1.25f;
        [SerializeField] float stoppingDistance = 1.3f;
        [SerializeField] float navMeshAgentRadius = 0.35f;

        HealthSystem healthSystem;
        Animator animator;
        NavMeshAgent agent;
        Rigidbody rigidBody;
        float turnAmount;
        float forwardAmount;
        Vector3 clickPoint;

        void Awake()
        {
            AddRequiredComponents();
        }

        void AddRequiredComponents()
        {
            SetupRigidbody();
            SetupCollider();
            SetupAnimator();
            SetupNavMeshAgent();
            SetupAudioSource();
        }

        void SetupRigidbody()
        {
            rigidBody = gameObject.AddComponent<Rigidbody>();
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            rigidBody.collisionDetectionMode = collisionDetectionMode;
        }

        void SetupCollider()
        {
            CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.center = colliderCenter;
            capsuleCollider.radius = colliderRadius;
            capsuleCollider.height = colliderHeight;
        }

        void SetupAnimator()
        {
            animator = gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            animator.avatar = characterAvatar;
            animator.speed = animationSpeed;
        }

        void SetupNavMeshAgent()
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updatePosition = true;
            agent.stoppingDistance = stoppingDistance;
            agent.radius = navMeshAgentRadius;
        }

        void SetupAudioSource()
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }


        void Start()
        {
            healthSystem = GetComponent<HealthSystem>();

            RegisterToCursor();
        }

        void Update()
        {
            HandleMovement();
        }

        void RegisterToCursor()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.onMouseOverWalkable += OnWalkableLayer;
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        void OnWalkableLayer(Vector3 destination)
        {
            if (healthSystem.IsDead()) { return; }
            if (Input.GetMouseButton(0)) { agent.SetDestination(destination); }
        }

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (healthSystem.IsDead()) { return; }

            if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(1))
            {
                agent.SetDestination(enemy.transform.position);
            }
        }

        void HandleMovement()
        {
            if (healthSystem.IsDead()) { agent.isStopped = true; }
            else if (agent.remainingDistance > agent.stoppingDistance) { Move(agent.desiredVelocity); }
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