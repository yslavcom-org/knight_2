using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTankGame
{
    public class TankLaunchHomingMissile : MonoBehaviour
    {
        private MyTankGame.HomingMissileController homingMissileController;

        private void Start()
        {
            // this is for test only and must be change to work on the pool of the missile
            homingMissileController = FindObjectOfType<MyTankGame.HomingMissileController>();
        }

        public void Launch(Radar radar)
        {
            if(null != homingMissileController
                && null != radar)
            {
                Vector3 position;
                if (radar.GetClosestLockedObject(out position))
                {
                    homingMissileController.Launch(position);
                }
            }
        }
    }
}
