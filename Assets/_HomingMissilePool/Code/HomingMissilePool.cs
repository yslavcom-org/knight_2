using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
HomingMissilePool which will dispatch the missile Launch.
Objects get created on start-up and reused
*/

namespace MyTankGame
{
    public class HomingMissilePool : MonoBehaviour
    {
        public const int homingMissileCount = 10;
        public GameObject homingMissilePrefab;
        GameObject[] homingMissilePool;
        private int currentIdx = -1;
        MyTankGame.IObjectId launcherObjId;
        

        // Start is called before the first frame update
        void Start()
        {
            var id = gameObject.GetComponentsInParent<MyTankGame.IObjectId>();
            if (null != id)
            {
                launcherObjId = id[0];
            }

            homingMissilePool = new GameObject[homingMissileCount];
            if (null != homingMissilePool)
            {
                for (int i = 0; i < homingMissileCount; i++)
                {
                    homingMissilePool[i] = Instantiate(homingMissilePrefab) as GameObject;
                }
                currentIdx = 0;
            }
        }

        #region Custom methods
        int GetTotalObjects()
        {
            return (null != homingMissilePool) ? homingMissilePool.Length : 0;
        }

        public bool BoLaunchMissile(Vector3 startPosition, Transform targetTransform, IndiePixel.Cameras.IP_Minimap_Camera homingMissileTrackingCamera)
        {
            int idx = GetNextIdleObjectIdx();
            if (0 > idx) return false;
            else
            {
                var handle = homingMissilePool[idx].GetComponent<MyTankGame.HomingMissileController>();
                Func<MyTankGame.IObjectId, Vector3, Transform, IndiePixel.Cameras.IP_Minimap_Camera, MyTankGame.HomingMissileController, bool> launch_lambda_foo = (id, from_position, to_transform, track_camera, hndl) =>
                {
                    hndl.Launch(id, from_position, to_transform, track_camera);
                    return true;
                };
                return null == handle 
                    ? false : launch_lambda_foo(launcherObjId, startPosition, targetTransform, homingMissileTrackingCamera, handle);
            }
        }

        int GetNextIdleObjectIdx()
        {
            int totalObj = GetTotalObjects();
            if (totalObj == 0) return -1;
            else
            {
                if(currentIdx >= totalObj)
                {
                    currentIdx = 0;
                }

                return currentIdx++;
            }
        }


        #endregion
    }
}
