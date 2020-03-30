using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankDemo
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(IP_Tank_Inputs))]
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
        private MyTankGame.TankGunShoot _tankGunShoot;

        private EnMoveUnderCondition moveUnderCondition = EnMoveUnderCondition.Idle;
        private Vector3 mouseClickTargetPosition = new Vector3(0, 0, 0);

        private GameModeEnumerator.CameraMode GameModeCameraMode = GameModeEnumerator.CameraMode.RadarView;

        #endregion

        #region Builtin Methods

        // Update is called once per frame
        void FixedUpdate()
        {
            if (ipTankInputs == null) return;

            HandleMovement(this.GameModeCameraMode);
        }

        void Update()
        {
            if (ipTankInputs == null) return;

            _tankGunShoot.TankUsesWeapons(ref gunCamera, ref homingMissileTrackingCamera, this.GameModeCameraMode, ipTankInputs);
        }
        #endregion

        #region Custom Methods
        public void SetParams(Transform tr, Rigidbody rb, IP_Tank_Inputs tankInp, MyTankGame.TankGunShoot gunShoot,
            float tankSpeed, float maxSpeed, float speedInSteps, float rotationSpeed, Health health)
        {
            _transform = tr;
            _rigidBody = rb;
            ipTankInputs = tankInp;
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

        public void SetGameModeCameraMode(GameModeEnumerator.CameraMode GameModeCameraMode)
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


        enum EnNavStat
        {
            Idle,
            Active
        };
        EnNavStat enNavStat = EnNavStat.Idle;
        int forward = 0;

        int TopViewMode__GetForwardAndRotate(int ctrl_world_angle, int tank_world_angle)
        {
            int rotate = 0;

            int diffr_angle = ctrl_world_angle - tank_world_angle;
            int quarter_ctrl = ctrl_world_angle / 90;
            int quarter_tank = tank_world_angle / 90;

            if (EnNavStat.Idle == enNavStat)
            {
                enNavStat = EnNavStat.Active;
                if (ctrl_world_angle == tank_world_angle)
                {
                    forward = 1;
                }
                else
                {
                    if (Mathf.Abs(ctrl_world_angle - tank_world_angle) <= 90)
                    {
                        forward = 1;
                    }
                    else if ((quarter_ctrl == 3 && quarter_tank == 0)
                        || (quarter_ctrl == 0 && quarter_tank == 3))
                    {
                        if (Mathf.Abs(diffr_angle) >= 270)
                        {
                            forward = 1;
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
                }
            }

            if (ctrl_world_angle == tank_world_angle)
            {
                rotate = 0;
            }
            else if (forward > 0)
            {
                if (Mathf.Abs(ctrl_world_angle - tank_world_angle) <= 90)
                {
                    rotate = (ctrl_world_angle > tank_world_angle) ? 1 : -1;
                }
                else if ((quarter_ctrl == 3 && quarter_tank == 0)
                    || (quarter_ctrl == 0 && quarter_tank == 3))
                {
                    if (Mathf.Abs(diffr_angle) >= 270)
                    {
                        rotate = (ctrl_world_angle > tank_world_angle) ? -1 : 1;
                    }
                    else
                    {
                        if (Mathf.Abs(diffr_angle) < 180) rotate = (diffr_angle < 0) ? -1 : 1;
                        else rotate = (diffr_angle < 0) ? 1 : -1;
                    }
                }
                else
                {
                    if (Mathf.Abs(diffr_angle) < 180) rotate = (diffr_angle < 0) ? -1 : 1;
                    else rotate = (diffr_angle < 0) ? 1 : -1;
                }
            }
            else if (forward < 0)
            {
                if (Mathf.Abs(diffr_angle) < 180) rotate = (diffr_angle < 0) ? 1 : -1;
                else rotate = (diffr_angle < 0) ? -1 : 1;
            }

            //Debug.Log(string.Format("tank_angle = {0}, control_angle = {1}, diffr_angle = {2}, rotate = {3}", tank_world_angle, ctrl_world_angle, diffr_angle, rotate));
            //Debug.Log(string.Format("ctrl_world_angle = {0}, rotate = {1}", ctrl_world_angle, rotate));

            return rotate;
        }

        const int move_ahead = 1;
        const int move_back = -1;
        const int stay_no_move = 0;
        const int rotate_right = 1;
        const int rotate_left = -1;

        int FrontViewMode__GetForwardAndRotate(int ctrl_world_angle)
        {
            int rotate = 0;

            int quarter_ctrl = ctrl_world_angle / 90;

            //if (EnNavStat.Idle == enNavStat)
            {
                enNavStat = EnNavStat.Active;

                if (ctrl_world_angle > 330
                    || ctrl_world_angle < 30)
                {
                    forward = move_ahead;
                    rotate = stay_no_move;
                }
                else if (ctrl_world_angle >= 30
                    && ctrl_world_angle < 60)
                {
                    forward = move_ahead;
                    rotate = rotate_right;
                }
                else if (ctrl_world_angle >= 60
                    && ctrl_world_angle < 120)
                {
                    forward = stay_no_move;
                    rotate = rotate_right;
                }
                else if (ctrl_world_angle >= 120
                    && ctrl_world_angle < 150)
                {
                    forward = move_back;
                    rotate = rotate_right;
                }
                else if (ctrl_world_angle >= 150
                    && ctrl_world_angle < 210)
                {
                    forward = move_back;
                    rotate = stay_no_move;
                }
                else if (ctrl_world_angle >= 210
                    && ctrl_world_angle < 240)
                {
                    forward = move_back;
                    rotate = rotate_left;
                }
                else if (ctrl_world_angle >= 240
                    && ctrl_world_angle < 330)
                {
                    forward = move_ahead;
                    rotate = rotate_left;
                }
            }

            //Debug.Log(string.Format("forward = {0}, rotate = {1}", forward, rotate));

            return rotate;
        }

        private void OnCollisionStay(Collision collisionInfo)
        {
            // Debug-draw all contact points and normals
            foreach (ContactPoint contact in collisionInfo.contacts)
            {
                if (HardcodedValues.StrTag__Building == contact.otherCollider.transform.tag)
                {
                    PrintDebugLog.PrintDebug("collided with building");
                    Debug.DrawRay(contact.point, contact.normal * 10, Color.white);

                    if(move_ahead == forward
                        || move_back == forward)
                    {
                        forward = stay_no_move;
                    }
                }
                
            }
        }

        void HandleTouchNavigation(GameModeEnumerator.CameraMode GameModeCameraMode)
        {
            //angle in the control element
            int ctrl_world_angle = (int)ipTankInputs.NavigationToroidalAngle;

            //get the tank angle
            int tank_world_angle = (int)GetTankHorizontalAngleInWorld();

            int rotate = (GameModeEnumerator.CameraMode.RadarView == GameModeCameraMode) 
                ? TopViewMode__GetForwardAndRotate( ctrl_world_angle,  tank_world_angle)
                : FrontViewMode__GetForwardAndRotate(ctrl_world_angle);

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
            Vector3 wantedPosition = _transform.position + (_transform.forward * ipTankInputs.ForwardInput * get_tank_speed(gear) * Time.deltaTime); // inasmuchas we're calling it from FixedUpdate, we do not need deltaTime
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

        protected virtual void HandleMovement(GameModeEnumerator.CameraMode GameModeCameraMode)
        {
            if (_rigidBody)
            {
                if(!ipTankInputs.NavigationToroidalControlActive)
                {
                    enNavStat = EnNavStat.Idle;
                }

                if (ipTankInputs.NavigationToroidalControlActive)
                {
                    HandleTouchNavigation(GameModeCameraMode);
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
            PrintDebugLog.PrintDebug("GunPointsThisObject");
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
