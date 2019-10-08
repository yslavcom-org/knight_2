using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTankGame
{
    public class TankLaunchHomingMissile :MonoBehaviour
    {
        HomingMissilePoolDispatch homingMissilePoolDispatch = null;

        public bool Launch( Radar radar)
        {
            if( null != radar)
            {
                if (radar.GetClosestLockedObject(out Vector3 targetPosition))
                {
                    if (!homingMissilePoolDispatch)
                    {
                        homingMissilePoolDispatch = GetComponent<HomingMissilePoolDispatch>();
                    }
                    return (null != homingMissilePoolDispatch) 
                        ? homingMissilePoolDispatch.BoLaunchMissile(targetPosition) : false;
                }
            }

            return false;
        }
    }
}
