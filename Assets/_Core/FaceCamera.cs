﻿using UnityEngine;

namespace RPG.Core
{
    public class FaceCamera : MonoBehaviour
    {
        Camera cameraToLookAt;

        // Use this for initialization 
        void Start()
        {
            cameraToLookAt = Camera.main;
        }

        // Update is called once per frame 
        void LateUpdate()
        {
            transform.LookAt(cameraToLookAt.transform);
        }
    }
}