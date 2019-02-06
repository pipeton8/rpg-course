using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RPG.Characters
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] Image healthBar = null;
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        [SerializeField] AudioClip[] damageSounds = null;
        [SerializeField] AudioClip[] deathSounds = null;
        [SerializeField] AnimationClip[] deathAnimations = null;
        // TODO maybe a parameter for character vanishing

        const string DEATH_TRIGGER = "Death"; 

        float currentHealthPoints;
        AudioSource audioSource;
        CharacterMovement characterMovement;
        AudioClip deathSound;
        AnimationClip deathAnimation;

        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        public bool IsDead() { return currentHealthPoints <= 0; }

        public void Heal(float amount)
        {
            float newHealthPoints = currentHealthPoints + amount;
            currentHealthPoints = Mathf.Clamp(newHealthPoints, 0f, maxHealthPoints);
        }

        public void TakeDamage(float damage)
        {
            if (IsDead()) { return; }
            PlayDamageSound();
            float newHealthPoints = currentHealthPoints - damage;
            currentHealthPoints = Mathf.Clamp(newHealthPoints, 0f, maxHealthPoints);
            if (IsDead()) { StartCoroutine(KillCharacter()); }
        }

        void Start()
        {
            characterMovement = GetComponent<CharacterMovement>();

            SetAudioSource();
            SetCurrentMaxHealth();
            SetDeathAnimation();
        }

        private void SetAudioSource()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = false;
        }

        void Update()
        {
            UpdateHealthBar();
        }

        void SetCurrentMaxHealth() { currentHealthPoints = maxHealthPoints; }

        void SetDeathAnimation()
        {
            deathAnimation = deathAnimations[Random.Range(0, deathAnimations.Length)];
            animatorOverrideController["DEFAULT_DEATH"] = deathAnimation; // remove const 
        }

        void UpdateHealthBar()
        {
            if (healthBar == null) { return; } // some enemies may not have a healthbar
            healthBar.fillAmount = healthAsPercentage;
        }

        void PlayDamageSound()
        {
            if (audioSource.isPlaying || damageSounds.Length == 0) { return; }
            int index = Random.Range(0, damageSounds.Length);
            audioSource.PlayOneShot(damageSounds[index]);
        }

        IEnumerator KillCharacter()
        {
            TriggerDeathAnimation();
            PlayDeathSound();

            float lengthToWait = Mathf.Max(deathSound.length, deathAnimation.length) + 0.1f;
            yield return new WaitForSeconds(lengthToWait);

            Player playerComponent = GetComponent<Player>();
            if (playerComponent && playerComponent.isActiveAndEnabled)
            {
                ReloadScene();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void ReloadScene()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.buildIndex);
        }

        void TriggerDeathAnimation()
        {
            Animator animator = GetComponent<Animator>();
            animator.SetTrigger(DEATH_TRIGGER);
        }

        void PlayDeathSound()
        {
            audioSource.Stop();
            GetDeathSound();
            audioSource.clip = deathSound;
            audioSource.Play();
        }

        void GetDeathSound() { deathSound = deathSounds[Random.Range(0, deathSounds.Length)]; }
    }
}