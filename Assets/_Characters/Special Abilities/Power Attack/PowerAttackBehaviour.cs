using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility
    {
        PowerAttack config;

        public void SetConfig(PowerAttack configToSet)
        {
            config = configToSet;
        }

        public void Use(AbilityUseParams useParams)
        {
            float totalDamage = useParams.baseDamage + config.GetExtraDamage();
            useParams.target.TakeDamage(totalDamage);
        }
    }
}
