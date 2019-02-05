using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class AbilityBehaviour : MonoBehaviour
    {
        SpecialAbility config;

        public abstract void Use(AbilityUseParams useParams);
    }
}


