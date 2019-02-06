using UnityEngine;
using UnityEngine.Assertions;

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

        const string ATTACK_TRIGGER = "Attack";

        GameObject dominantHand;
        GameObject weaponInHand;
        Enemy currentEnemy = null;
        Animator animator;
        SpecialAbilities abilities;
        HealthSystem healthSystem;
        float lastHitTime;

        public delegate void OnAbilityUse(int abilityIndex, GameObject target = null);
        public event OnAbilityUse onAbilityUse;

        public void ChangeWeapon(Weapon newWeapon)
        {
            Destroy(weaponInHand);
            weaponInUse = newWeapon;
            PutWeaponInHand();
            SetAttackAnimation();
        }

        void Start()
        {
            abilities = GetComponent<SpecialAbilities>();
            healthSystem = GetComponent<HealthSystem>();

            RegisterForEnemyCursor();
            PutWeaponInHand();
            SetupRuntimeAnimator();
        }

        void Update()
        {
            if (!healthSystem.IsDead()) { ScanForAbilityKeyDown(); }
        }

        void RegisterForEnemyCursor()
        {
            CameraRaycaster cameraRaycaster = FindObjectOfType<CameraRaycaster>();
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
            for(int abilityIndex = 1; abilityIndex < abilities.numberOfAbilities; abilityIndex++)
            {
                if (Input.GetKeyDown(abilityIndex.ToString())) 
                {
                    if (currentEnemy == null) { onAbilityUse(abilityIndex); }
                    else { onAbilityUse(abilityIndex,currentEnemy.gameObject); }
                    return;
                }
            }
        }

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (enemy.GetComponent<HealthSystem>().IsDead()) { return; }
            currentEnemy = enemy;
            if (CanAttack() && IsTargetInRange())
            {
                if (Input.GetMouseButton(0)) { Attack(); }
                else if (Input.GetMouseButtonDown(1)) 
                {
                    onAbilityUse(0, currentEnemy.gameObject);
                }
                lastHitTime = Time.time;
            }
        }

        bool IsTargetInRange()
        {
            float distanceToTarget = Vector3.Distance(currentEnemy.transform.position, transform.position);
            return distanceToTarget <= weaponInUse.GetMaxAttackRange();
        }

        bool CanAttack()
        {
            if (healthSystem.IsDead()) { return false; }
            float timeSinceLastAttack = Time.time - lastHitTime;
            return timeSinceLastAttack > weaponInUse.GetMinTimeBetweenHits();
        }

        void Attack()
        {
            TriggerAttackAnimation();
            float totalDamage = CalculateDamage();
            DealDamage(totalDamage);
        }

        float CalculateDamage()
        {
            float damageBeforeCritical = baseDamage + weaponInUse.GetAdditionalDamage();
            bool isCriticalHit = Random.Range(0f, 1f) <= criticalHitChance;
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

        void DealDamage(float damage) { currentEnemy.GetComponent<HealthSystem>().TakeDamage(damage); }

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