using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System;

namespace RPG.Characters
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] float baseDamage = 10f;
        [SerializeField] Weapon weaponInUse = null;
        [Range(0f, 1f)] [SerializeField] float criticalHitChance = 0.1f;
        [SerializeField] float criticalHitMultiplier = 1.25f;
        [SerializeField] GameObject criticalHitParticlePrefab = null;

        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";

        GameObject dominantHand;
        GameObject weaponInHand;
        GameObject currentTarget;
        bool powerAttack;
        Animator animator;

        public float attackRadius { get { return weaponInUse.GetMaxAttackRange(); } }

        public void RequestPowerAttack() { powerAttack = true; }

        public void StopAttacking() { StopAllCoroutines(); }

        public void SetTarget(GameObject newTarget) { currentTarget = newTarget; }

        public void ChangeWeapon(Weapon newWeapon)
        {
            Destroy(weaponInHand);
            weaponInUse = newWeapon;
            PutWeaponInHand();
            SetAttackAnimation();
        }

        void Start()
        {
            animator = GetComponent<Animator>();

            PutWeaponInHand();
            SetAttackAnimation();
            StartCoroutine(AttackTarget());
        }

        void PutWeaponInHand()
        {
            dominantHand = RequestDominantHand();
            weaponInHand = Instantiate(weaponInUse.GetWeapon(), dominantHand.transform);
            weaponInHand.transform.localPosition = weaponInUse.gripTransform.localPosition;
            weaponInHand.transform.localRotation = weaponInUse.gripTransform.localRotation;
        }

        void SetAttackAnimation()
        {
            AnimationClip attackClip = weaponInUse.GetAnimClip();
            AnimatorOverrideController animatorOverrideController = GetComponent<Character>().runtimeAnimatorController;
            animatorOverrideController[DEFAULT_ATTACK] = attackClip;
        }

        GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int totalDominantHands = dominantHands.Length;

            Assert.AreNotEqual(totalDominantHands, 0, "No DominantHand found on Player, please add one");
            Assert.IsFalse(totalDominantHands > 1, "Multiple Dominant Hand scripts on Player, please remove one");

            return dominantHands[0].gameObject;
        }
        
        IEnumerator AttackTarget()
        {
            while (true)
            {
                if (IsTargetInRange() && IsTargetAlive())
                {
                    TriggerAttackAnimation();
                    StartCoroutine(DelayToDealDamage(weaponInUse.timeOfHit));
                    if (powerAttack)
                    {
                        GetComponent<SpecialAbilities>().RequestUse(0, currentTarget);
                        powerAttack = false;
                    }
                    else
                    {
                        float totalDamage = CalculateDamage();
                        DealDamage(totalDamage, currentTarget);
                    }
                    float weaponHitPeriod = weaponInUse.GetMinTimeBetweenHits();
                    float animSpeedMultiplier = GetComponent<Character>().animSpeed;
                    yield return new WaitForSeconds(weaponHitPeriod * animSpeedMultiplier);
                }
                yield return null;
            }
        }

        IEnumerator DelayToDealDamage(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
        }

        bool IsTargetInRange()
        {
            if (currentTarget == null) { return false; }
            float distanceToTarget = Vector3.Distance(currentTarget.transform.position, transform.position);
            return distanceToTarget <= weaponInUse.GetMaxAttackRange();
        }

        bool IsTargetAlive()
        {
            HealthSystem targetHealthSystem = currentTarget.GetComponent<HealthSystem>();
            if (targetHealthSystem == null || targetHealthSystem.isDead) { return false; }
            return true;
        }

        void TriggerAttackAnimation()
        {
            transform.LookAt(currentTarget.transform);
            animator.SetTrigger(ATTACK_TRIGGER);
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

        void PlayParticleEffect()
        {
            GameObject newParticleObject = Instantiate(criticalHitParticlePrefab, dominantHand.transform);
            ParticleSystem criticalHitParticles = newParticleObject.GetComponent<ParticleSystem>();
            criticalHitParticles.Play();
            Destroy(newParticleObject, criticalHitParticles.main.duration);
        }

        void DealDamage(float damage, GameObject target) { target.GetComponent<HealthSystem>().TakeDamage(damage); }

        void OnDrawGizmos()
        {
            // Draw attack sphere 
            Gizmos.color = new Color(255f, 0, 0, .5f);
            Gizmos.DrawWireSphere(transform.position, weaponInUse.GetMaxAttackRange());
        }
    }
}