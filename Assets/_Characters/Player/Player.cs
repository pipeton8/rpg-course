using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

// TODO consider re-wiring
using RPG.CameraUI;
using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters
{
    public class Player : MonoBehaviour, IDamageable
    {
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float baseDamage = 10f;
        [SerializeField] Weapon weaponInUse = null;
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        [SerializeField] AudioClip[] damageSounds = null;
        [SerializeField] AudioClip[] deathSounds = null;
        [SerializeField] AnimationClip[] deathAnimations = null;

        // Temporarily serialized for dubbing
        [SerializeField] SpecialAbility[] abilities = null;

        const string ATTACK_TRIGGER = "Attack";
        const string DEATH_TRIGGER = "Death";

        AudioSource audioSource;
        CameraRaycaster cameraRaycaster;
        Animator animator;
        Energy energy;
        float currentHealthPoints;
        float lastHitTime;
        bool dead;
        AudioClip deathSound;
        AnimationClip deathAnimation;

        public bool IsDead() { return currentHealthPoints <= 0; }

        public void TakeDamage(float damage)
        {
            if (IsDead()) { return; }
            bool playerDies = currentHealthPoints <= damage;
            ReduceHealth(damage);
            PlayDamageSound();
            if (playerDies) { StartCoroutine(KillPlayer()); }
        }

        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        void Start()
        {
            RegisterForEnemyCursor();
            SetCurrentMaxHealth();
            PutWeaponInHand();
            SetupRuntimeAnimator();
            SetAudioSource();
            abilities[0].AttachComponentTo(gameObject);
            energy = GetComponent<Energy>();
        }

        void RegisterForEnemyCursor()
        {
            cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        void SetCurrentMaxHealth() { currentHealthPoints = maxHealthPoints; }

        void PutWeaponInHand()
        {
            GameObject dominantHand = RequestDominantHand();
            GameObject weaponInHand = Instantiate(weaponInUse.GetWeapon(), dominantHand.transform);
            weaponInHand.transform.localPosition = weaponInUse.gripTransform.localPosition;
            weaponInHand.transform.localRotation = weaponInUse.gripTransform.localRotation;
        }

        void SetupRuntimeAnimator()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            SetAttackAnimation();
            SetDeathAnimation();
        }

        void SetAttackAnimation()
        {
            animatorOverrideController["DEFAULT_ATTACK"] = weaponInUse.GetAnimClip(); // remove const 
        }

        void SetDeathAnimation()
        {
            GetDeathAnimation();
            animatorOverrideController["DEFAULT_DEATH"] = deathAnimation; // remove const 
        }

        void GetDeathAnimation() 
        {
            deathAnimation = deathAnimations[Random.Range(0, deathAnimations.Length)];
        }

        void SetAudioSource()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = false;
        }

        void GetDeathSound() { deathSound = deathSounds[Random.Range(0, deathSounds.Length)]; }

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (enemy.IsDead()) { return; }

            if (IsTargetInRange(enemy) && CanAttack())
            {
                if (Input.GetMouseButton(0)) 
                {
                    Attack(enemy);
                }
                else if (Input.GetMouseButtonDown(1) && energy.IsEnergyAvailable(abilities[0].GetEnergyCost()))
                {
                    print("Special attacking " + enemy.gameObject.name);
                    SpecialAttack(0, enemy);
                }
            }
        }

        void SpecialAttack(int abilityIndex, Enemy enemy)
        {
            energy.ConsumeEnergy(abilities[abilityIndex].GetEnergyCost());
            AbilityUseParams abilityParams = new AbilityUseParams(gameObject, enemy, baseDamage);

            abilities[abilityIndex].Use(abilityParams);
            lastHitTime = Time.time;
        }

        bool IsTargetInRange(Enemy enemy)
        {
            float distanceToTarget = Vector3.Distance(enemy.transform.position, transform.position);
            return distanceToTarget <= weaponInUse.GetMaxAttackRange();
        }

        bool CanAttack()
        {
            float timeSinceLastAttack = Time.time - lastHitTime;
            return timeSinceLastAttack > weaponInUse.GetMinTimeBetweenHits();
        }

        void Attack(Enemy enemy)
        {
            TriggerAttackAnimation();
            DealDamage(baseDamage, enemy);
            lastHitTime = Time.time;
        }

        void TriggerAttackAnimation()
        {
            animator = GetComponent<Animator>();
            animator.SetTrigger(ATTACK_TRIGGER);
        }

        void DealDamage(float damage, Enemy enemy) { enemy.TakeDamage(damage); }

        void ReduceHealth(float damage)
        {
            float newHealthPoints = currentHealthPoints - damage;
            currentHealthPoints = Mathf.Clamp(newHealthPoints, 0f, maxHealthPoints);
        }

        void PlayDamageSound()
        {
            if (audioSource.isPlaying) { return; }
            int index = Random.Range(0, damageSounds.Length);
            audioSource.PlayOneShot(damageSounds[index]);
        }

        GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int totalDominantHands = dominantHands.Length;

            Assert.AreNotEqual(totalDominantHands, 0, "No DominantHand found on Player, please add one");
            Assert.IsFalse(totalDominantHands > 1, "Multiple Dominant Hand scripts on Player, please remove one");

            return dominantHands[0].gameObject;
        }

        IEnumerator KillPlayer()
        {
            TriggerDeathAnimation();
            PlayDeathSound();

            float lengthToWait = Mathf.Max(deathSound.length, deathAnimation.length) + 0.1f;
            yield return new WaitForSeconds(lengthToWait);

            ReloadScene();
        }

        void ReloadScene()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.buildIndex);
        }

        void TriggerDeathAnimation()
        {
            animator = GetComponent<Animator>();
            animator.SetTrigger(DEATH_TRIGGER);
        }

        void PlayDeathSound()
        {
            audioSource.Stop();
            GetDeathSound();
            audioSource.PlayOneShot(deathSound);
        }
    }
}