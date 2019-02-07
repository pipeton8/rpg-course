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

        const string DEATH_TRIGGER = "Death";
        const string DEFAULT_DEATH = "DEFAULT_DEATH";

        AudioSource audioSource;
        float currentHealthPoints;
        Animator animator;
        AudioClip deathSound;
        AnimationClip deathAnimation;

        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        public bool isDead { get { return currentHealthPoints <= 0; } }

        public void Heal(float amount)
        {
            float newHealthPoints = currentHealthPoints + amount;
            currentHealthPoints = Mathf.Clamp(newHealthPoints, 0f, maxHealthPoints);
        }

        public void TakeDamage(float damage)
        {
            if (isDead) { return; }
            PlayDamageSound();
            float newHealthPoints = currentHealthPoints - damage;
            currentHealthPoints = Mathf.Clamp(newHealthPoints, 0f, maxHealthPoints);
            if (isDead) { StartCoroutine(KillCharacter()); }
        }

        void Start()
        {
            audioSource = GetComponent<AudioSource>();

            SetCurrentMaxHealth();
            SetupAnimationsForDeathAndDamage();
        }

        void Update()
        {
            UpdateHealthBar();
        }

        void SetCurrentMaxHealth() { currentHealthPoints = maxHealthPoints; }

        void SetupAnimationsForDeathAndDamage()
        {
            animator = GetComponent<Animator>();
            deathAnimation = deathAnimations[Random.Range(0, deathAnimations.Length)];
            animatorOverrideController = GetComponent<Character>().runtimeAnimatorController;
            animatorOverrideController[DEFAULT_DEATH] = deathAnimation;
        }

        void UpdateHealthBar()
        {
            if (healthBar == null) { return; } // some characters may not have a healthbar
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
            GetComponent<WeaponSystem>().StopAttacking();
            TriggerDeathAnimation();
            PlayDeathSound();

            float lengthToWait = Mathf.Max(deathSound.length, deathAnimation.length) + 0.1f;
            yield return new WaitForSeconds(lengthToWait);

            PlayerControl playerComponent = GetComponent<PlayerControl>();
            if (playerComponent && playerComponent.isActiveAndEnabled)
            {
                ReloadScene();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void TriggerDeathAnimation() { animator.SetTrigger(DEATH_TRIGGER); }

        void PlayDeathSound()
        {
            audioSource.Stop();
            GetDeathSound();
            audioSource.clip = deathSound;
            audioSource.Play();
        }

        void GetDeathSound() { deathSound = deathSounds[Random.Range(0, deathSounds.Length)]; }

        void ReloadScene()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.buildIndex);
        }
    }
}