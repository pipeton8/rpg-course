using UnityEngine;
using UnityEngine.AI;

namespace RPG.Characters
{
    [SelectionBase]
    public class Character : MonoBehaviour
    {
        [Header("Animator")]
        [SerializeField] RuntimeAnimatorController animatorController = null;
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        [SerializeField] Avatar characterAvatar = null;

        [Header("Audio")]
        [Range(0f, 1f)] [SerializeField] float spatialBlend = 0;

        [Header("Capsule Collider")]
        [SerializeField] Vector3 colliderCenter = new Vector3(0, 0.9f, 0);
        [SerializeField] float colliderRadius = 0.2f;
        [SerializeField] float colliderHeight = 1.85f;

        [Header("Movement")]
        [SerializeField] float movingTurnSpeed = 720f;
        [SerializeField] float stationaryTurnSpeed = 360f;
        [SerializeField] float moveSpeedMultiplier = 1.25f;
        [SerializeField] float animationSpeed = 1.25f;

        [Header("Nav Mesh Agent")]
        [SerializeField] float navMeshAgentSteerinSpeed = 1.0f;
        [SerializeField] float navMeshAgentStoppingDistance = 1.3f;
        [SerializeField] float navMeshAgentRadius = 0.35f;

        [Header("Rigidbody")]
        [SerializeField] CollisionDetectionMode collisionDetectionMode = CollisionDetectionMode.Continuous;

        bool isDead;
        Animator animator;
        NavMeshAgent navMeshAgent;
        Rigidbody rigidBody;
        float turnAmount;
        float forwardAmount;

        public AnimatorOverrideController runtimeAnimatorController { get { return animatorOverrideController; } }
        public void Kill() { isDead = true; }
        public void SetDestination(Vector3 destination) { navMeshAgent.destination = destination; }
        public float stoppingDistance { get { return navMeshAgentStoppingDistance;} }
        public float animSpeed { get { return animationSpeed; } }

        void Awake()
        {
            AddRequiredComponents();
        }

        void Update()
        {
            HandleMovement();
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
            animatorOverrideController.runtimeAnimatorController = animatorController;
            animator.runtimeAnimatorController = animatorOverrideController;
            animator.avatar = characterAvatar;
            animator.speed = animationSpeed;
        }

        void SetupNavMeshAgent()
        {
            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent.speed = navMeshAgentSteerinSpeed;
            navMeshAgent.stoppingDistance = navMeshAgentStoppingDistance;
            navMeshAgent.radius = navMeshAgentRadius;
            navMeshAgent.updateRotation = false;
            navMeshAgent.updatePosition = true;
            navMeshAgent.autoBraking = false;
        }

        void SetupAudioSource()
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = spatialBlend;
        }

        void HandleMovement()
        {
            if (isDead) { navMeshAgent.isStopped = true; }
            else if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance) { Move(navMeshAgent.desiredVelocity); }
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