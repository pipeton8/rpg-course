using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [ExecuteInEditMode]
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] Weapon weapon = null;
        [SerializeField] AudioClip pickupSound = null;

        void LateUpdate()
        {
            if (!Application.isPlaying)
            {
                DestroyChildren();
                InstantiateWeapon();
            }
        }

        void DestroyChildren()
        {
            foreach (Transform child in transform) { DestroyImmediate(child.gameObject); }
        }

        void InstantiateWeapon()
        {
            GameObject weaponObject = weapon.GetWeapon();
            Instantiate(weaponObject, gameObject.transform);
        }

        void OnTriggerEnter(Collider other)
        {
            WeaponSystem weaponSystem = other.GetComponent<WeaponSystem>();
            if (other.tag != "Player" || weaponSystem == null) { return; }
            weaponSystem.ChangeWeapon(weapon);
            PlaySoundEffect();
            SelfDestroy();
        }

        private void SelfDestroy()
        {
            foreach (Transform child in transform) { Destroy(child.gameObject); }
            GetComponent<BoxCollider>().enabled = false;
            Destroy(gameObject, pickupSound.length);
        }

        private void PlaySoundEffect() { GetComponent<AudioSource>().PlayOneShot(pickupSound); }
    }
}
