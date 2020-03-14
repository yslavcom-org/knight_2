using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTankGame
{
    public class PlayerTurretControl : MonoBehaviour
    {
        #region Turret Variables
        private float rotSpeed = 0.25f;
        #endregion

        #region Barrel Variables
        private float rotSpeedBarrel = 0.25f;

        float maxBarrelUp = 20f;
        float maxBarrelDown = -10f;
        float counter = 0f;

        bool barrelUp = false;
        bool barrelDown = false;
        bool left = false;
        bool right = false;

        public GameObject barrel;
        #endregion

        #region CrossHair
        CrossHairControl crossHair;
        #endregion

        private void Update()
        {
            if (null != crossHair)
            {
                float navAngle;
                float navRelativeDistance;
                bool boPressed = crossHair.GetPressedDirection(out navAngle, out navRelativeDistance);

                if (boPressed)
                {
                    PrintDebugLog.PrintDebug(string.Format("navAngle = {0}, navRelativeDistance = {1}", navAngle, navRelativeDistance));
                }

                if (boPressed && navRelativeDistance >= 0.14)
                {
                    if (navAngle >= 15 && navAngle < 165)
                    {
                        right = true;
                        left = false;
                    }
                    else if (navAngle >= 165 && navAngle < 345)
                    {
                        left = true;
                        right = false;
                    }

                    if (navAngle >= 285 || navAngle < 75)
                    {
                        barrelUp = true;
                        barrelDown = false;
                    }
                    else if (navAngle >= 105 && navAngle < 255)
                    {
                        barrelUp = false;
                        barrelDown = true;
                    }

                }
                else
                {
                    left = false;
                    right = false;
                    barrelUp = false;
                    barrelDown = false;
                }
            }
        }

        private void FixedUpdate()
        {

            RotateTurret();
            BarrelUpDown();
        }

        #region Turret Methods
        void RotateTurret()
        {
            if (left)
            {
                transform.Rotate(0, -rotSpeed, 0);
            }

            if (right)
            {
                transform.Rotate(0, rotSpeed, 0);
            }
        }
        #endregion

        #region Barrel Methods
        public void BarrelUpSetter(bool val) { barrelUp = val; }

        void BarrelUpDown()
        {
            if (barrelUp && counter < maxBarrelUp)
            {
                barrel.transform.Rotate(0, 0, -rotSpeedBarrel);
                counter += rotSpeedBarrel;
            }

            if (barrelDown && counter > maxBarrelDown)
            {
                barrel.transform.Rotate(0, 0, rotSpeedBarrel);
                counter -= rotSpeedBarrel;
            }
        }
        #endregion

        public void SetCrosshair(GameObject crossHair)
        {
            if (null == crossHair) return;
            this.crossHair = crossHair.GetComponent<CrossHairControl>();
        }

    }
}
