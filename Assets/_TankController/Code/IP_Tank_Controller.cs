using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankDemo
{
   [RequireComponent(typeof(Rigidbody))]
   [RequireComponent(typeof(IP_Tank_Inputs))]
   [RequireComponent(typeof(MyTankGame.Tank_Navigation))]
   [RequireComponent(typeof(MyTankGame.ShootRaycast))]
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

        private Transform _transform;
        private Rigidbody _rigidBody;
        private IP_Tank_Inputs _ipTankInputs;
        private MyTankGame.Tank_Navigation _tankNavigation;
        private MyTankGame.TankGunShoot _tankGunShoot;

        private EnMoveUnderCondition moveUnderCondition = EnMoveUnderCondition.Idle;
        private Vector3 mouseClickTargetPosition = new Vector3(0,0,0);

        private bool isRadarMode = false;

        #endregion

        #region Builtin Methods

        // Update is called once per frame
        void FixedUpdate()
        {
            if (_ipTankInputs)
            {
                HandleMovement();
                _tankGunShoot.ShootGun(ref gunCamera, isRadarMode, _ipTankInputs);
            }
        }
        #endregion

        #region Custom Methods
        public void SetParams(Transform tr, Rigidbody rb, IP_Tank_Inputs tankInp, MyTankGame.Tank_Navigation navi, MyTankGame.TankGunShoot gunShoot,
            float tankSpeed, float maxSpeed, float speedInSteps,  float rotationSpeed)
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

            isRadarMode = false;
        }

        public void SetGunCamera(Camera cam)
        {
            gunCamera = cam;
        }

        public void SetRadarMode(bool isRadar)
        {
            isRadarMode = isRadar;
        }

        protected virtual void HandleMovement()
        {
            if (_rigidBody)
            {
                if (_ipTankInputs.MovementKeyDown)
                {
                    //move tank forwards
                    Vector3 wantedPosition = _transform.position + (_transform.forward * _ipTankInputs.ForwardInput * actualTankSpeed * Time.deltaTime);
                    _rigidBody.MovePosition(wantedPosition);

                    //rotate tank
                    Quaternion wantedRotation = _transform.rotation * Quaternion.Euler(Vector3.up * tankRotationSpeed * _ipTankInputs.RotationInput * Time.deltaTime);
                    _rigidBody.MoveRotation(wantedRotation);

                    moveUnderCondition = EnMoveUnderCondition.KeyPressed;
                }
                else if (_ipTankInputs.BoMouseClicked)
                {
                    mouseClickTargetPosition = _ipTankInputs.MouseTrackPosition;

                    moveUnderCondition = EnMoveUnderCondition.MouseClicked;
                    _ipTankInputs.MouseClickAck();
                }

                if (moveUnderCondition == EnMoveUnderCondition.MouseClicked)
                {
                    _tankNavigation.GraduallyMoveRigidBody(_transform, _rigidBody, mouseClickTargetPosition, actualTankSpeed, tankRotationSpeed);
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
            Debug.Log("GunShootsThisObject");
           //
           // if (healthAmountPercent >= tankGunReduceHealthAmount)
           // {
           //     healthAmountPercent -= tankGunReduceHealthAmount;
           // }
           // else
           // {//vehicle destroyed
           //     healthAmountPercent = 0f;
           // }

            return true;
        }
        #endregion
    }
}
