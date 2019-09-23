using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTankGame
{
    public class PlayerTurretControl : MonoBehaviour
    {
        #region Turret Variables
        public float rotSpeed = 0.5f;
        #endregion

        #region Barrel Variables
        public float rotSpeedBarrel = 0.25f;

        float maxBarrelUp = 20f;
        float maxBarrelDown = -10f;
        float counter = 0f;

        bool barrelUp = false;
        bool barrelDown = false;

        public GameObject barrel;
        #endregion


        private void FixedUpdate()
        {
            RotateTurret();
            BarrelUpDown();
        }

        #region Turret Methods
        bool left = false;
        bool right = false;

        public void RotateLeft()
        {
            left = true;
        }

        public void RotateRight()
        {
            right = true;
        }

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
        public void BarrelUp()
        {
            barrelUp = true;
        }

        public void BarrelDown()
        {
            barrelDown = true;
        }

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

        public void RotateStop()
        {
            left = false;
            right = false;

            barrelUp = false;
            barrelDown = false;
        }

    }
}
