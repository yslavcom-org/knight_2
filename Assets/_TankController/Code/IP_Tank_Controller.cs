﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankDemo
{
   [RequireComponent(typeof(Rigidbody))]
   [RequireComponent(typeof(IP_Tank_Inputs))]
   [RequireComponent(typeof(MyTankGame.Tank_Navigation))]
   [RequireComponent(typeof(MyTankGame.ShootRaycast))]
   [RequireComponent(typeof(IndiePixel.Cameras.IP_Minimap_Camera))]
    public class IP_Tank_Controller : MonoBehaviour, IHomingMissileDamageable, ITankGunDamageable
    {
        #region Custom Enumerators
        enum EnMoveUnderCondition
        {
            Idle = 0,
            MouseClicked,
            KeyPressed,
        };
        #endregion

        #region Variables
        private float defTankSpeed = 5f;
        private float maxTankSpeed = 7f;
        private float speedStep = 0.2f;
        private float tankRotationSpeed = 50f;
        private float actualTankSpeed = 0f;
        const float minTankSpeed = 0f;
        private Camera gunCamera;
        private IndiePixel.Cameras.IP_Minimap_Camera homingMissileTrackingCamera;
        Health health;

        private Transform _transform;
        private Rigidbody _rigidBody;
        private IP_Tank_Inputs _ipTankInputs;
        private MyTankGame.Tank_Navigation _tankNavigation;
        private MyTankGame.TankGunShoot _tankGunShoot;

        private EnMoveUnderCondition moveUnderCondition = EnMoveUnderCondition.Idle;
        private Vector3 mouseClickTargetPosition = new Vector3(0,0,0);

        private GameModeEnumerator.CameraMode GameModeCameraMode = GameModeEnumerator.CameraMode.TopView;

        #endregion

        #region Builtin Methods

        // Update is called once per frame
        void FixedUpdate()
        {
            if (_ipTankInputs == null) return;

            HandleMovement();
        }

        void Update()
        {
            if (_ipTankInputs == null) return;

            _tankGunShoot.TankUsesWeapons(ref gunCamera, ref homingMissileTrackingCamera, this.GameModeCameraMode, _ipTankInputs);
        }
        #endregion

        #region Custom Methods
        public void SetParams(Transform tr, Rigidbody rb, IP_Tank_Inputs tankInp, MyTankGame.Tank_Navigation navi, MyTankGame.TankGunShoot gunShoot,
            float tankSpeed, float maxSpeed, float speedInSteps,  float rotationSpeed, Health health)
        {
            _transform = tr;
            _rigidBody = rb;
            _ipTankInputs = tankInp;
            _tankNavigation = navi;
            _tankGunShoot = gunShoot;

            defTankSpeed = tankSpeed;
            maxTankSpeed = maxSpeed;
            speedStep = speedInSteps;
            tankRotationSpeed = rotationSpeed;

            actualTankSpeed = defTankSpeed;

            this.health = health;

            GameModeCameraMode = GameModeEnumerator.CameraMode.TopView; 
        }

        public void SetWeaponCameras(Camera cam, IndiePixel.Cameras.IP_Minimap_Camera homingMissileTrackingCamera)
        {
            gunCamera = cam;
            this.homingMissileTrackingCamera = homingMissileTrackingCamera;
        }

        public void SetGameModeCameraMode(GameModeEnumerator.CameraMode GameModeCameraMode )
        {
            this.GameModeCameraMode = GameModeCameraMode;
        }

        private float PosNegAngle(Vector3 a1, Vector3 a2, Vector3 normal)
        {
            //Where "normal" is the reference Vector you are determining the clockwise / counter-clockwise rotation around.
            float angle = Vector3.Angle(a1, a2);
            float sign = Mathf.Sign(Vector3.Dot(normal, Vector3.Cross(a1, a2)));
            return angle * sign;
        }

        float GetTankHorizontalAngleInWorld()
        {
            float tank_angle = PosNegAngle(_transform.forward, Vector3.forward, Vector3.up);

            //get the tank_angle to the same basis as the ctrl_angle
            if (tank_angle <= 0 && tank_angle >= -180)
            {
                tank_angle = -1 * tank_angle;
            }
            else if (tank_angle > 0 && tank_angle <= 180)
            {
                tank_angle = 360 - tank_angle;
            }

            return tank_angle;
        }

        int GetReverseRotation(int ctrl_angle, int tank_angle)
        {
            int rotate = 0;

            int tank_opposite_angle_rem = Mathf.Abs(180 - tank_angle) % 90;
            int ctrl_angle_rem = ctrl_angle % 90;
            if (tank_opposite_angle_rem > ctrl_angle_rem)
            {
                rotate = -1;
            }
            else if (tank_opposite_angle_rem < ctrl_angle_rem)
            {
                rotate = 1;
            }

            return rotate;
        }

        void HandleTouchNavigation()
        {
            int forward = 0;
            int rotate = 0;

            //angle in the control element
            int ctrl_world_angle = (int)_ipTankInputs.NavigationToroidalAngle;

            //get the tank angle
            int tank_world_angle = (int)GetTankHorizontalAngleInWorld();

            if (ctrl_world_angle == tank_world_angle)
            {
                forward = 1;
                rotate = 0;
            }
            else
            {
                int diffr_angle = ctrl_world_angle - tank_world_angle;
                int quarter_ctrl = ctrl_world_angle / 90;
                int quarter_tank = tank_world_angle / 90;

                if (Mathf.Abs(ctrl_world_angle - tank_world_angle) <= 90)
                {
                    forward = 1;
                    rotate = (ctrl_world_angle > tank_world_angle) ? 1 : -1;
                }
                else if((quarter_ctrl == 3 && quarter_tank == 0)
                    || (quarter_ctrl == 0 && quarter_tank == 3))
                {
                    if (Mathf.Abs(diffr_angle) >= 270)
                    {
                        forward = 1;
                        rotate = (ctrl_world_angle > tank_world_angle) ? -1 : 1;
                    }
                    else
                    {
                        forward = -1;
                    }
                }
                else
                {
                    forward = -1;
                }

                if(forward < 0)
                {
                    if (Mathf.Abs(diffr_angle) < 180) rotate = (diffr_angle < 0) ? 1 : -1;
                    else rotate = (diffr_angle < 0) ? -1 : 1;
                }

                Debug.Log(string.Format("tank_angle = {0}, control_angle = {1}, diffr_angle = {2}, rotate = {3}", tank_world_angle, ctrl_world_angle, diffr_angle, rotate));
            }

            Quaternion wantedRotation = _transform.rotation * Quaternion.Euler(Vector3.up * tankRotationSpeed * rotate * Time.deltaTime);
            _rigidBody.MoveRotation(wantedRotation);

            //move tank
            Vector3 wantedPosition = _transform.position + (_transform.forward * forward * actualTankSpeed * Time.deltaTime);
            _rigidBody.MovePosition(wantedPosition);

            moveUnderCondition = EnMoveUnderCondition.KeyPressed;
        }

        void HandleKeyPressNavigation()
        {
            //move tank forwards
            Vector3 wantedPosition = _transform.position + (_transform.forward * _ipTankInputs.ForwardInput * actualTankSpeed * Time.deltaTime);
            _rigidBody.MovePosition(wantedPosition);

            //rotate tank
            Quaternion wantedRotation = _transform.rotation * Quaternion.Euler(Vector3.up * tankRotationSpeed * _ipTankInputs.RotationInput * Time.deltaTime);
            _rigidBody.MoveRotation(wantedRotation);

            moveUnderCondition = EnMoveUnderCondition.KeyPressed;
        }

        protected virtual void HandleMovement()
        {
            if (_rigidBody)
            {
                if(_ipTankInputs.NavigationToroidalControlActive)
                {
                    HandleTouchNavigation();
                }
                else if (_ipTankInputs.NavigationKeyPressed)
                {
                    HandleKeyPressNavigation();
                }
            }
        }
#endregion

#region IHomingMissileDamage
        public bool IsHomingMissileDamageable()
        {
            return true;
        }
        public bool HomingMissileBlowUp()
        {
            return true;
        }
#endregion

#region ITankGunDamageable
        public bool GunPointsThisObject(Vector3 distance, object obj)
        {
            //highlight the object icon
            Debug.Log("GunPointsThisObject");
            return true;
        }

        public bool GunShootsThisObject(Vector3 distance, object obj)
        {
            //highlight the object icon

            health.ModifyStamina(-10);

            return true;
        }
#endregion
    }
}
