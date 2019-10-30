using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
The tank may have several missile cassettes.
It will be using them one after another.
HomingMissilePoolDispatch will help us
*/

namespace MyTankGame
{
    public class HomingMissilePoolDispatch : MonoBehaviour
    {
        IndiePixel.Cameras.IP_Minimap_Camera homingMissileTrackingCamera;
        private MyTankGame.HomingMissilePool[] homingMissilePoolArray;
        private int idx = -1;

        #region Built-in methods
        // Start is called before the first frame update
        void Start()
        {
        }

        void Awake()
        {
        }

        #endregion

        #region Custom methods

        public void Init(MyTankGame.HomingMissilePool[] missilePool, IndiePixel.Cameras.IP_Minimap_Camera homingMissileTrackingCamera)
        {
            this.homingMissileTrackingCamera = homingMissileTrackingCamera;
            homingMissilePoolArray = missilePool;
            if (0 < homingMissilePoolArray.Length)
            {
                idx = 0;
            }
        }

        public bool BoLaunchMissile( Transform targetTransform)
        {
            if (idx < 0)
            {
                return false;
            }
            else
            {
                if (idx >= homingMissilePoolArray.Length)
                {
                    idx = 0;
                }

                if (null != homingMissilePoolArray[idx])
                {
                    Vector3 startPosition = homingMissilePoolArray[idx].transform.position; // this object must be a tank's missile cassette
                    return homingMissilePoolArray[idx].BoLaunchMissile(startPosition, targetTransform, homingMissileTrackingCamera);
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion
    }
}