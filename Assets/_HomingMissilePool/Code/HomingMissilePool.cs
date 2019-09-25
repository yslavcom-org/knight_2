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

        public bool BoLaunchMissile(Vector3 startPosition, Vector3 targetPosition)
        {
            int idx = GetNextIdleObjectIdx();
            if (0 > idx) return false;
            else
            {
                var handle = homingMissilePool[idx].GetComponent<MyTankGame.HomingMissileController>();
                Func<Vector3, Vector3, MyTankGame.HomingMissileController, bool> launch_lambda_foo = (from_position, to_position, hndl) =>
                {
                    hndl.Launch(from_position, to_position);
                    return true;
                };
                return null == handle 
                    ? false : launch_lambda_foo(startPosition, targetPosition, handle);
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
