using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTankGame
{
    public class TankLaunchHomingMissile : MonoBehaviour
    {
        private MyTankGame.HomingMissilePool homingMissilePool;

        private void Start()
        {
            // this is for test only and must be change to work on the pool of the missile
            homingMissilePool = FindObjectOfType<MyTankGame.HomingMissilePool>();
        }

        public bool Launch(Radar radar)
        {
            if(null != homingMissilePool
                && null != radar)
            {
                if (radar.GetClosestLockedObject(out Vector3 position))
                {
                    return homingMissilePool.BoLaunchMissile(position);
                }
            }

            return false;
        }
    }
}
