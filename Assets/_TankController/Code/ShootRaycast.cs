using UnityEngine;

namespace MyTankGame
{
    public class ShootRaycast : MonoBehaviour
    {
        static int layerMaskToFilterOut = 0;

        void Awake()
        {
            layerMaskToFilterOut = LayerMask.GetMask(HardcodedValues.Layer__ActiveDefence);

            // This would cast rays only against colliders in layer 'Layer__ActiveDefence'.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMaskToFilterOut = ~layerMaskToFilterOut;
        }

        public static bool BoRaycastHit(Vector3 rayOrigin, Vector3 direction, float weaponRange, out Vector3 hitPosition, out Vector3 hitNormal, out Collider hitCollider)
        {
            // Create a vector at the center of our camera's viewport
            

            bool boHit = false;

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, direction, out hit, weaponRange, layerMaskToFilterOut))
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