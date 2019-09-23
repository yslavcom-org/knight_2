using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankDemo
{
   [RequireComponent(typeof(Rigidbody))]
   [RequireComponent(typeof(IP_Tank_Inputs))]
   [RequireComponent(typeof(MyTankGame.Tank_Navigation))]
   [RequireComponent(typeof(MyTankGame.ShootRaycast))]
    public class IP_Tank_Controller : MonoBehaviour
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
        public float defTankSpeed = 5f;
        public float maxTankSpeed = 7f;
        public float speedStep = 0.2f;
        const float minTankSpeed = 0f;
        private float actualTankSpeed;
        //public Transform bottomPositionSensor;
        public float tankRotationSpeed = 50f;

        private Transform _transform;
        private Rigidbody _rigidBody;
        private IP_Tank_Inputs _ipTankInputs;
        private MyTankGame.Tank_Navigation _tankNavigation;
        private MyTankGame.TankGunShoot _tankGunShoot;

        [SerializeField]
        private EnMoveUnderCondition moveUnderCondition = EnMoveUnderCondition.Idle;
        [SerializeField]
        private Vector3 mouseClickTargetPosition = new Vector3(0,0,0);
        #endregion


        #region Builtin Methods

        // Start is called before the first frame update
        void Start()
        {
            actualTankSpeed = defTankSpeed;

            _transform = GetComponent<Transform>();
            _rigidBody = GetComponent<Rigidbody>();
            _ipTankInputs = GetComponent<IP_Tank_Inputs>();
            _tankNavigation = GetComponent<MyTankGame.Tank_Navigation>();
            _tankGunShoot = GetComponent<MyTankGame.TankGunShoot>();

            #region Log Some Errors
            if (null == transform)
            {
                //Debug.LogError("transform does not exist");
                return;
            }
            if (null == _rigidBody)
            {
                //Debug.LogError("rigidBody does not exist");
                return;
            }
            if (null == _ipTankInputs)
            {
                //Debug.LogError("ipTankInputs does not exist");
                return;
            }
            if (null == _tankNavigation)
            {
                //Debug.LogError("tankNavigation does not exist");
                return;
            }
            if (null == _tankGunShoot)
            {
                //Debug.LogError("_tankGunShoot does not exist");
                return;
            }
         
            #endregion
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (_ipTankInputs)
            {
                HandleMovement();
                _tankGunShoot.ShootGun(_ipTankInputs);
            }
        }
        #endregion

        #region Custom Methods
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
    }
}
