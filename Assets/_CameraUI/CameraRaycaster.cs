using UnityEngine;
using UnityEngine.EventSystems;

using RPG.Characters; // So we can detect by type

namespace RPG.CameraUI
{
    public class CameraRaycaster : MonoBehaviour 
    {
        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D enemyCursor = null;
        [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

        const int WALKABLE_LAYER = 9;
        float maxRaycastDepth = 5000f; // Hard coded value

        public delegate void OnMouseOverEnemy(EnemyAI enemy);
        public event OnMouseOverEnemy onMouseOverEnemy;

        public delegate void OnMouseOverWalkable(Vector3 destination);
        public event OnMouseOverWalkable onMouseOverWalkable;

        void Update()
        {
            // Check if pointer is over an interactable UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Implement UI Interaction
            }
            else
            {
                PerformRaycasts();
            }
        }

        void PerformRaycasts()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (RaycastForEnemy(ray)) { return; }
            if (RaycastForWalkable(ray)) { return; }
        }

        bool RaycastForEnemy(Ray ray)
        {
            EnemyAI enemyHit = RetrieveEnemyHit(ray);
            if (enemyHit != null)
            {
                Cursor.SetCursor(enemyCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverEnemy(enemyHit);
                return true;
            }
            return false;
        }

        EnemyAI RetrieveEnemyHit(Ray ray)
        {
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, maxRaycastDepth);
            if (hitInfo.collider == null) { return null; }
            var gameObjectHit = hitInfo.collider.gameObject;
            return gameObjectHit.GetComponent<EnemyAI>();
        }

        bool RaycastForWalkable(Ray ray)
        {
            RaycastHit hitInfo;
            LayerMask walkableLayerMask = 1 << WALKABLE_LAYER;
            bool walkableHit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, walkableLayerMask);

            if (walkableHit)
            {
                Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverWalkable(hitInfo.point);
                return true;
            }
            return false;
        }
    }
}