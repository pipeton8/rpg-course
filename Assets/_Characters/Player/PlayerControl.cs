using UnityEngine;
using UnityEngine.Assertions;

// TODO consider re-wiring
using RPG.CameraUI;

// TODO extract WeaponSystem
namespace RPG.Characters
{
    public class PlayerControl : MonoBehaviour
    {
        Character character;
        Animator animator;
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

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (healthSystem.isDead) { return; }
            if (enemy.GetComponent<HealthSystem>().isDead) { return; }

            currentEnemy = enemy.gameObject;

            if (Input.GetMouseButton(0)) 
            { 
                weaponSystem.SetTarget(currentEnemy);
                character.SetDestination(enemy.transform.position);
            }
            else if (Input.GetMouseButtonDown(1)) 
            {
                weaponSystem.SetTarget(currentEnemy);
                weaponSystem.RequestPowerAttack();
                character.SetDestination(enemy.transform.position);
            }
        }
    }
}