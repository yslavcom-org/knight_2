using System.Collections;
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
        private IP_Tank_Inputs ipTankInputs;
        private MyTankGame.Tank_Navigation _tankNavigation;
        private MyTankGame.TankGunShoot _tankGunShoot;

        private EnMoveUnderCondition moveUnderCondition = EnMoveUnderCondition.Idle;
        private Vector3 mouseClickTargetPosition = new Vector3(0,0,0);

        private GameModeEnumerator.CameraMode GameModeCameraMode = GameModeEnumerator.CameraMode.RadarView;

        #endregion

        #region Builtin Methods

        // Update is called once per frame
        void FixedUpdate()
        {
            if (ipTankInputs == null) return;

            HandleMovement();
        }

        void Update()
        {
            if (ipTankInputs == null) return;

            _tankGunShoot.TankUsesWeapons(ref gunCamera, ref homingMissileTrackingCamera, this.GameModeCameraMode, ipTankInputs);
        }
        #endregion

        #region Custom Methods
        public void SetParams(Transform tr, Rigidbody rb, IP_Tank_Inputs tankInp, MyTankGame.Tank_Navigation navi, MyTankGame.TankGunShoot gunShoot,
            float tankSpeed, float maxSpeed, float speedInSteps,  float rotationSpeed, Health health)
        {
            _transform = tr;
            _rigidBody = rb;
            ipTankInputs = tankInp;
            _tankNavigation = navi;
            _tankGunShoot = gunShoot;

            defTankSpeed = tankSpeed;
            maxTankSpeed = maxSpeed;
            speedStep = speedInSteps;
            tankRotationSpeed = rotationSpeed;

            actualTankSpeed = defTankSpeed;

            this.health = health;

            GameModeCameraMode = GameModeEnumerator.CameraMode.RadarView; 
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

        TorNavCalc.EnNavStat enNavStat = TorNavCalc.EnNavStat.Idle;
        int forward = 0;
        void HandleToroidTouchNavigation()
        {
            TorNavCalc.HandleToroidTouchNavigation__KeepDirection(ref _transform,
                ref enNavStat,
                ipTankInputs.NavigationToroidalAngle,
                forward,
                out forward,
                out int rotate
            );
            Quaternion wantedRotation = _transform.rotation * Quaternion.Euler(Vector3.up * tankRotationSpeed * rotate * Time.deltaTime);
            _rigidBody.MoveRotation(wantedRotation);

            //move tank
            Vector3 wantedPosition = _transform.position + (_transform.forward * forward * get_tank_speed(ipTankInputs.NavigationToroidalGearNum) * Time.deltaTime);
            _rigidBody.MovePosition(wantedPosition);

            moveUnderCondition = EnMoveUnderCondition.KeyPressed;
        }

        void HandleKeyPressNavigation()
        {
            //move tank forwards
            float gear =    1.0f;
            Vector3 wantedPosition = _transform.position + (_transform.forward * ipTankInputs.ForwardInput * get_tank_speed(gear) * Time.deltaTime);
            _rigidBody.MovePosition(wantedPosition);

            //rotate tank
            Quaternion wantedRotation = _transform.rotation * Quaternion.Euler(Vector3.up * tankRotationSpeed * ipTankInputs.RotationInput * Time.deltaTime);
            _rigidBody.MoveRotation(wantedRotation);

            moveUnderCondition = EnMoveUnderCondition.KeyPressed;
        }

        float get_tank_speed(float gear )
        {
            return actualTankSpeed * gear;
        }

        protected virtual void HandleMovement()
        {
            if (_rigidBody)
            {
                if(!ipTankInputs.NavigationToroidalControlActive)
                {
                    TorNavCalc.ResetStateMachine(ref enNavStat);
                }

                if (ipTankInputs.NavigationToroidalControlActive)
                {
                    HandleToroidTouchNavigation();
                }
                else if (ipTankInputs.NavigationKeyPressed)
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
