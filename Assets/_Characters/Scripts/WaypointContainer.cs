using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class WaypointContainer : MonoBehaviour
    {
        void OnDrawGizmos()
        {
            Vector3 firstPosition = transform.GetChild(0).position;
            Vector3 previousPosition = firstPosition; 

            foreach (Transform waypoint in transform)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(waypoint.position, 0.2f);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(waypoint.position, previousPosition);
                previousPosition = waypoint.position;
            }
            Gizmos.DrawLine(previousPosition, firstPosition);
        }
    }
}