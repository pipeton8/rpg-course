using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        [SerializeField] Image energyBar = null;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float regenPointsPerSecond = 10f;

        float currentEnergyPoints;

        public bool IsEnergyAvailable(float amount) { return amount <= currentEnergyPoints; }

        public void ConsumeEnergy(float amount)
        {
            currentEnergyPoints = Mathf.Clamp(currentEnergyPoints - amount, 0f, maxEnergyPoints);
            UpdateEnergyBar();
        }

        // Start is called before the first frame update
        void Start()
        {
            SetMaxEnergyPoints();
        }

        void Update()
        {
            RegenerateEnergy();
        }

        void RegenerateEnergy()
        {
            float amountToRegerate = regenPointsPerSecond * Time.deltaTime;
            ConsumeEnergy(-amountToRegerate);
        }

        void SetMaxEnergyPoints() { currentEnergyPoints = maxEnergyPoints; }

        void UpdateEnergyBar() { energyBar.fillAmount = energyAsPercentage; }

        float energyAsPercentage { get { return currentEnergyPoints / maxEnergyPoints; } }

    }
}
