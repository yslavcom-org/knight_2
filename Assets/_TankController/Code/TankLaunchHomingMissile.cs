using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTankGame
{
    public class TankLaunchHomingMissile :MonoBehaviour
    {
        private MyTankGame.HomingMissilePoolDispatch homingMissilePoolDispatch;

        private void Start()
        {
            // this is for test only and must be change to work on the pool of the missile
            homingMissilePoolDispatch = FindObjectOfType<MyTankGame.HomingMissilePoolDispatch>();
        }

        public bool Launch(Radar radar)
        {
            if(null != homingMissilePoolDispatch
                && null != radar)
            {
                if (radar.GetClosestLockedObject(out Vector3 targetPosition))
                {
                    return homingMissilePoolDispatch.BoLaunchMissile(targetPosition);
                }
            }

            return false;
        }
    }
}
