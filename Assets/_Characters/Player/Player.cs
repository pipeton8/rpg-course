using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

// TODO consider re-wiring
using RPG.CameraUI;
using RPG.Core;
using RPG.Weapons;
using System;

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
        [Range(.1f, 1f)] [SerializeField] float criticalHitChance = 0.1f;
        [SerializeField] float criticalHitMultiplier = 1.25f;
        [SerializeField] GameObject criticalHitParticlePrefab = null;

        // Temporarily serialized for dubbing
        [SerializeField] SpecialAbility[] abilities = null;

        const string ATTACK_TRIGGER = "Attack";
        const string DEATH_TRIGGER = "Death";

        GameObject dominantHand;
        Enemy currentEnemy = null;
        AudioSource audioSource;
        CameraRaycaster cameraRaycaster;
        Animator animator;
        Energy energy;
        float currentHealthPoints;
        float lastHitTime;
        AudioClip deathSound;
        AnimationClip deathAnimation;
        
        public bool IsDead() { return currentHealthPoints <= 0; }

        public void TakeDamage(float damage)
        {
            if (IsDead()) { return; }
            PlayDamageSound();
            float newHealthPoints = currentHealthPoints - damage;
            currentHealthPoints = Mathf.Clamp(newHealthPoints, 0f, maxHealthPoints);
            if (IsDead()) { StartCoroutine(KillPlayer()); }
        }

        public void Heal(float amount)
        {
            float newHealthPoints = currentHealthPoints + amount;
            currentHealthPoints = Mathf.Clamp(newHealthPoints, 0f, maxHealthPoints);
        }

        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        void Start()
        {
            energy = GetComponent<Energy>();

            RegisterForEnemyCursor();
            SetCurrentMaxHealth();
            PutWeaponInHand();
            SetupRuntimeAnimator();
            AttachInitialAbilities();
            SetAudioSource();
        }

        private void AttachInitialAbilities()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachAbilityTo(gameObject);
            }
        }

        void Update()
        {
            if (!IsDead()) { ScanForAbilityKeyDown(); }
        }

        void RegisterForEnemyCursor()
        {
            cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        void SetCurrentMaxHealth() { currentHealthPoints = maxHealthPoints; }

        void PutWeaponInHand()
        {
            dominantHand = RequestDominantHand();
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
            deathAnimation = deathAnimations[UnityEngine.Random.Range(0, deathAnimations.Length)];
        }

        void SetAudioSource()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = false;
        }

        void ScanForAbilityKeyDown()
        {
            for(int abilityIndex = 1; abilityIndex < abilities.Length; abilityIndex++)
            {
                if (Input.GetKeyDown(abilityIndex.ToString()) && CanCast(abilityIndex)) 
                {
                    SpecialAttack(abilityIndex);
                }
            }

        }

        void GetDeathSound() { deathSound = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)]; }

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (enemy.IsDead()) { return; }

            currentEnemy = enemy;
            if (CanAttack() && IsTargetInRange())
            {
                if (Input.GetMouseButton(0)) { Attack(); }
                else if (Input.GetMouseButtonDown(1) && CanCast(0)) { SpecialAttack(0); }
            }
        }

        bool IsTargetInRange()
        {
            float distanceToTarget = Vector3.Distance(currentEnemy.transform.position, transform.position);
            return distanceToTarget <= weaponInUse.GetMaxAttackRange();
        }

        bool CanAttack()
        {
            if (IsDead()) { return false; }
            float timeSinceLastAttack = Time.time - lastHitTime;
            return timeSinceLastAttack > weaponInUse.GetMinTimeBetweenHits();
        }

        bool CanCast(int abilityIndex)
        {
            return energy.IsEnergyAvailable(abilities[abilityIndex].GetEnergyCost());
        }

        void Attack()
        {
            TriggerAttackAnimation();
            float totalDamage = CalculateDamage();
            DealDamage(totalDamage);
            lastHitTime = Time.time;
        }

        float CalculateDamage()
        {
            float damageBeforeCritical = baseDamage + weaponInUse.GetAdditionalDamage();
            bool isCriticalHit = UnityEngine.Random.Range(0f, 1f) <= criticalHitChance;
            if (isCriticalHit) 
            {
                PlayParticleEffect(); 
                return damageBeforeCritical * criticalHitMultiplier; 
            }
            return damageBeforeCritical;
        }

        private void PlayParticleEffect()
        {
            GameObject newParticleObject = Instantiate(criticalHitParticlePrefab, dominantHand.transform);
            ParticleSystem criticalHitParticles = newParticleObject.GetComponent<ParticleSystem>();
            criticalHitParticles.Play();
            Destroy(newParticleObject, criticalHitParticles.main.duration);
        }

        void TriggerAttackAnimation()
        {
            animator = GetComponent<Animator>();
            animator.SetTrigger(ATTACK_TRIGGER);
        }

        void DealDamage(float damage) { currentEnemy.TakeDamage(damage); }

        void SpecialAttack(int abilityIndex)
        {
            energy.ConsumeEnergy(abilities[abilityIndex].GetEnergyCost());
            AbilityUseParams abilityParams = new AbilityUseParams(currentEnemy, baseDamage);
            abilities[abilityIndex].Use(abilityParams);
            lastHitTime = Time.time;
        }

        void PlayDamageSound()
        {
            if (audioSource.isPlaying) { return; }
            int index = UnityEngine.Random.Range(0, damageSounds.Length);
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