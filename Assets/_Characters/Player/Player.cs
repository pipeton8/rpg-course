using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

// TODO consider re-wiring
using RPG.CameraUI;

namespace RPG.Characters
{
    public class Player : MonoBehaviour
    {
        [SerializeField] float baseDamage = 10f;
        [SerializeField] Weapon weaponInUse = null;
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        [Range(.1f, 1f)] [SerializeField] float criticalHitChance = 0.1f;
        [SerializeField] float criticalHitMultiplier = 1.25f;
        [SerializeField] GameObject criticalHitParticlePrefab = null;

        // Temporarily serialized for dubbing
        [SerializeField] SpecialAbility[] abilities = null;

        const string ATTACK_TRIGGER = "Attack";

        GameObject dominantHand;
        GameObject weaponInHand;
        Enemy currentEnemy = null;
        CameraRaycaster cameraRaycaster;
        Animator animator;
        Energy energy;
        float lastHitTime;

        public void ChangeWeapon(Weapon newWeapon)
        {
            Destroy(weaponInHand);
            weaponInUse = newWeapon;
            PutWeaponInHand();
            SetAttackAnimation();
        }

        void Start()
        {
            energy = GetComponent<Energy>();

            RegisterForEnemyCursor();
            PutWeaponInHand();
            SetupRuntimeAnimator();
            AttachInitialAbilities();
        }

        void AttachInitialAbilities()
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

        void PutWeaponInHand()
        {
            dominantHand = RequestDominantHand();
            weaponInHand = Instantiate(weaponInUse.GetWeapon(), dominantHand.transform);
            weaponInHand.transform.localPosition = weaponInUse.gripTransform.localPosition;
            weaponInHand.transform.localRotation = weaponInUse.gripTransform.localRotation;
        }

        void SetupRuntimeAnimator()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            SetAttackAnimation();
        }

        void SetAttackAnimation()
        {
            animatorOverrideController["DEFAULT_ATTACK"] = weaponInUse.GetAnimClip(); // remove const 
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


        GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int totalDominantHands = dominantHands.Length;

            Assert.AreNotEqual(totalDominantHands, 0, "No DominantHand found on Player, please add one");
            Assert.IsFalse(totalDominantHands > 1, "Multiple Dominant Hand scripts on Player, please remove one");

            return dominantHands[0].gameObject;
        }
    }
}