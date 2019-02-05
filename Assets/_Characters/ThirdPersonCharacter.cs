using UnityEngine;

// TODO change to RPG.Characters namespace
namespace RPG.Characters
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class ThirdPersonCharacter : MonoBehaviour
	{
		[SerializeField] float movingTurnSpeed = 360;
		[SerializeField] float stationaryTurnSpeed = 180;
		[SerializeField] float animSpeedMultiplier = 1f;

		Animator animator;
		float turnAmount;
		float forwardAmount;

		void Start()
        {
            SetupAnimator();
            SetupRigidbody();
        }

        void SetupAnimator()
        {
            animator = GetComponent<Animator>();
            animator.speed = animSpeedMultiplier;
        }

        void SetupRigidbody()
        {
            Rigidbody characterRigidbody = GetComponent<Rigidbody>();
            characterRigidbody.constraints =    RigidbodyConstraints.FreezeRotationX | 
                                                RigidbodyConstraints.FreezeRotationY | 
                                                RigidbodyConstraints.FreezeRotationZ;
        }

        public void Move(Vector3 move)
		{
			move.Normalize();
			move = transform.InverseTransformDirection(move);
			turnAmount = Mathf.Atan2(move.x, move.z);
			forwardAmount = move.z;
			ApplyExtraTurnRotation();
			UpdateAnimator(move);
		}

		void UpdateAnimator(Vector3 move)
		{
			animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
			animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
		}

		void ApplyExtraTurnRotation()
		{
			// help the character turn faster
			float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
			transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
		}
	}
}