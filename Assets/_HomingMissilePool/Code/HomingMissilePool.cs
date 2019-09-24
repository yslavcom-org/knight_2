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

        // Start is called before the first frame update
        void Start()
        {
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

        public bool BoLaunchMissile(Vector3 targetPosition)
        {
            int idx = GetNextIdleObjectIdx();
            if (0 > idx) return false;
            else
            {
                var handle = homingMissilePool[idx].GetComponent<MyTankGame.HomingMissileController>();
                Func<Vector3, MyTankGame.HomingMissileController, bool> launch_lambda_foo = (position, hndl) =>
                {
                    hndl.Launch(position);
                    return true;
                };
                return null == handle 
                    ? false : launch_lambda_foo(targetPosition, handle);
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
