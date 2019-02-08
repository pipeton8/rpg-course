using UnityEngine;

using RPG.CameraUI;

namespace RPG.Characters
{
    [RequireComponent(typeof(WeaponSystem))]
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(HealthSystem))]
    public class PlayerControl : MonoBehaviour
    {
        Character character;
        SpecialAbilities abilities;
        WeaponSystem weaponSystem;
        HealthSystem healthSystem;
        GameObject currentEnemy;

        void Start()
        {
            character = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();
            healthSystem = GetComponent<HealthSystem>();
            weaponSystem = GetComponent<WeaponSystem>();

            RegisterForMouseEvents();
        }

        void Update()
        {
            if (!healthSystem.isDead) { ScanForAbilityKeyDown(); }
            else { character.Kill(); }
        }

        void RegisterForMouseEvents()
        {
            CameraRaycaster cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.onMouseOverWalkable += OnMouseOverWalkable;
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        void ScanForAbilityKeyDown()
        {
            for(int abilityIndex = 1; abilityIndex < abilities.numberOfAbilities; abilityIndex++)
            {
                if (Input.GetKeyDown(abilityIndex.ToString())) 
                {
                    if (currentEnemy == null) { abilities.RequestUse(abilityIndex); }
                    else { abilities.RequestUse(abilityIndex,currentEnemy.gameObject); }
                    return;
                }
            }
        }

        void OnMouseOverWalkable(Vector3 destination)
        {
            if(Input.GetMouseButton(0)) { character.SetDestination(destination); }
        }

        void OnMouseOverEnemy(EnemyAI enemy)
        {
            if (healthSystem.isDead) { return; }
            if (enemy.GetComponent<HealthSystem>().isDead) { return; }

            currentEnemy = enemy.gameObject;

            if (Input.GetMouseButton(0))
            {
                TargetAndMove(currentEnemy);
            }
            else if (Input.GetMouseButtonDown(1)) 
            {
                TargetAndMove(currentEnemy);
                weaponSystem.RequestPowerAttack();
            }
        }

        private void TargetAndMove(GameObject target)
        {
            weaponSystem.SetTarget(target);
            character.SetDestination(target.transform.position);
        }
    }
}