﻿using System.Collections;
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
        private MyTankGame.HomingMissilePool[] homingMissilePoolArray;
        private int idx = -1;

        #region Built-in methods
        // Start is called before the first frame update
        void Start()
        {
            homingMissilePoolArray = FindObjectsOfType<MyTankGame.HomingMissilePool>();
            if(0 < homingMissilePoolArray.Length)
            {
                idx = 0;
            }
        }
        #endregion

        #region Custom methods
        public bool BoLaunchMissile(Vector3 targetPosition)
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
                    return homingMissilePoolArray[idx].BoLaunchMissile(targetPosition);
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