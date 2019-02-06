using UnityEngine;
using UnityEngine.Assertions;

// TODO consider re-wiring
using RPG.CameraUI;

// TODO extract WeaponSystem
namespace RPG.Characters
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] float baseDamage = 10f;
        [SerializeField] Weapon weaponInUse = null;
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        [Range(.1f, 1f)] [SerializeField] float criticalHitChance = 0.1f;
        [SerializeField] float criticalHitMultiplier = 1.25f;
        [SerializeField] GameObject criticalHitParticlePrefab = null;

        const string ATTACK_TRIGGER = "Attack"; // TODO move to Weapon System

        Character character;
        GameObject dominantHand; // TODO move to Weapon System
        GameObject weaponInHand; // TODO move to Weapon System
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
            character = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();
            healthSystem = GetComponent<HealthSystem>();

            RegisterForMouseEvents();
        }

        void Update()
        {
            if (!healthSystem.isDead) { ScanForAbilityKeyDown(); }
        }

        void RegisterForMouseEvents()
        {
            CameraRaycaster cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.onMouseOverWalkable += OnMouseOverWalkable;
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        // TODO move to Weapon System
        void PutWeaponInHand()
        {
            dominantHand = RequestDominantHand();
            weaponInHand = Instantiate(weaponInUse.GetWeapon(), dominantHand.transform);
            weaponInHand.transform.localPosition = weaponInUse.gripTransform.localPosition;
            weaponInHand.transform.localRotation = weaponInUse.gripTransform.localRotation;
        }

        // TODO move to Weapon System
        void SetAttackAnimation()
        {
            animatorOverrideController["DEFAULT_ATTACK"] = weaponInUse.GetAnimClip(); // remove const 
        }

        // TODO move to Weapon System
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

        void OnMouseOverWalkable(Vector3 destination)
        {
            if(Input.GetMouseButton(0)) { character.SetDestination(destination); }
        }

        // TODO move attack to Weapon System
        void OnMouseOverEnemy(Enemy enemy)
        {
            if (enemy.GetComponent<HealthSystem>().isDead) { return; }
            currentEnemy = enemy;
            character.SetDestination(enemy.transform.position);
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

        // TODO move to Weapon System
        bool IsTargetInRange()
        {
            float distanceToTarget = Vector3.Distance(currentEnemy.transform.position, transform.position);
            return distanceToTarget <= weaponInUse.GetMaxAttackRange();
        }

        // TODO move to Weapon System
        bool CanAttack()
        {
            if (healthSystem.isDead) { return false; }
            float timeSinceLastAttack = Time.time - lastHitTime;
            return timeSinceLastAttack > weaponInUse.GetMinTimeBetweenHits();
        }

        // TODO move to Weapon System
        void Attack()
        {
            TriggerAttackAnimation();
            float totalDamage = CalculateDamage();
            DealDamage(totalDamage);
        }

        // TODO move to Weapon System
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

        // TODO move to Weapon System
        void PlayParticleEffect()
        {
            GameObject newParticleObject = Instantiate(criticalHitParticlePrefab, dominantHand.transform);
            ParticleSystem criticalHitParticles = newParticleObject.GetComponent<ParticleSystem>();
            criticalHitParticles.Play();
            Destroy(newParticleObject, criticalHitParticles.main.duration);
        }

        // TODO move to Weapon System
        void TriggerAttackAnimation()
        {
            animator = GetComponent<Animator>();
            animator.SetTrigger(ATTACK_TRIGGER);
        }

        // TODO move to Weapon System
        void DealDamage(float damage) { currentEnemy.GetComponent<HealthSystem>().TakeDamage(damage); }

        // TODO move to Weapon System
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