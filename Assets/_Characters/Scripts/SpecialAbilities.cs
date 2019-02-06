using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class SpecialAbilities : MonoBehaviour
    {
        [SerializeField] SpecialAbility[] abilities = null;
        [SerializeField] Image energyBar = null;
        [SerializeField] AudioClip outOfEnergySound = null;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float regenPointsPerSecond = 10f;

        float currentEnergyPoints;
        AudioSource audioSource;

        public int numberOfAbilities { get { return abilities.Length; } }
        float energyAsPercentage { get { return currentEnergyPoints / maxEnergyPoints; } }

        public void RequestUse(int abilityIndex, GameObject target = null)
        {
            float energyCost = abilities[abilityIndex].GetEnergyCost();
            if (energyCost > currentEnergyPoints)
            {
                audioSource.PlayOneShot(outOfEnergySound);
                return;
            }
            ConsumeEnergy(energyCost);
            abilities[abilityIndex].Use(target);
        }

        public void ConsumeEnergy(float amount)
        {
            currentEnergyPoints = Mathf.Clamp(currentEnergyPoints - amount, 0f, maxEnergyPoints);
            UpdateEnergyBar();
        }

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            SetMaxEnergyPoints();
            AttachInitialAbilities();
        }

        void Update()
        {
            RegenerateEnergy();
        }

        void AttachInitialAbilities()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachAbilityTo(gameObject);
            }
        }

        void RegenerateEnergy()
        {
            float amountToRegerate = regenPointsPerSecond * Time.deltaTime;
            ConsumeEnergy(-amountToRegerate);
        }

        void SetMaxEnergyPoints() { currentEnergyPoints = maxEnergyPoints; }

        void UpdateEnergyBar() { energyBar.fillAmount = energyAsPercentage; }
    }
}
