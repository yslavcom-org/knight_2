using UnityEngine;

namespace MyTankGame
{
    public class ShootRaycast : MonoBehaviour
    {
        public static bool BoRaycastHit(Camera camera, float weaponRange, out Vector3 hitPosition, out Vector3 hitNormal, out Collider hitCollider)
        {
            // Create a vector at the center of our camera's viewport
            Vector3 rayOrigin = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

            bool boHit = false;

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, camera.transform.forward, out hit, weaponRange))
            {
                hitPosition = hit.point;
                hitNormal = hit.normal;
                hitCollider = hit.collider;
                boHit = true;
            }
            else
            {
                hitPosition = new Vector3(0, 0, 0);
                hitNormal = new Vector3(0, 0, 0);
                hitCollider = null;
            }

            return boHit;
        }
    }
}