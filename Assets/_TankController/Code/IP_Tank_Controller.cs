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

        enum ToroidMoveDir{
            Stop,
            Ahead,
            Back
        };
        enum ToroidRotate
        {
            Stop,
            Right,
            Left
        };

        ToroidMoveDir tor_move_dir__collided = ToroidMoveDir.Stop;
        int keyb_move_dir__collided = 0;
        ToroidMoveDir tor_move_dir = ToroidMoveDir.Stop;


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

        bool move_stop_collision_tag(Collider otherCollider)
        {
            return (HardcodedValues.StrTag__Building == otherCollider.transform.tag
                || HardcodedValues.StrTag__StreetPole == otherCollider.transform.tag
                || HardcodedValues.StrTag__Tree == otherCollider.transform.tag
                || HardcodedValues.StrTag__LevelBoundaryPlain == otherCollider.transform.tag)
                ? true : false;
        }

        private void stop_before_collision(Collider otherCollider)
        {
            if (move_stop_collision_tag(otherCollider))
            {
#if false
                if (this.tag == "Player") // debug only
                {
                    PrintDebugLog.PrintDebug("collided with building");
                    // Debug-draw all contact points and normals
                    //Debug.DrawRay(contact.point, contact.normal * 10, Color.white);
                }
#endif
                if (tor_move_dir != ToroidMoveDir.Stop)
                {
                    tor_move_dir__collided = tor_move_dir;
                }

                if (0 != ipTankInputs.ForwardInput)
                {
                    keyb_move_dir__collided = ipTankInputs.ForwardInput;
                }
            }
        }

        private void OnTriggerStay(Collider otherCollider)
        {
            stop_before_collision(otherCollider);
        }

        private void OnTriggerEnter(Collider otherCollider)
        {
            stop_before_collision(otherCollider);
        }

        private void OnTriggerExit(Collider otherCollider)
        {
            if (move_stop_collision_tag(otherCollider))
            {
#if false
                if (this.tag == "Player") // debug only
                {
                    PrintDebugLog.PrintDebug("exit from collision with building");
                }
#endif
                tor_move_dir__collided = ToroidMoveDir.Stop;
                keyb_move_dir__collided = 0;

            }
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

        ToroidRotate TopViewMode__GetForwardAndRotate(int ctrl_world_angle, int tank_world_angle)
        {
            ToroidRotate rotate = ToroidRotate.Stop;

            int diffr_angle = ctrl_world_angle - tank_world_angle;
            int quarter_ctrl = ctrl_world_angle / 90;
            int quarter_tank = tank_world_angle / 90;

            if (EnNavStat.Idle == enNavStat)
            {
                enNavStat = EnNavStat.Active;
                if (ctrl_world_angle == tank_world_angle)
                {
                    tor_move_dir = ToroidMoveDir.Ahead;
                }
                else
                {
                    if (Mathf.Abs(ctrl_world_angle - tank_world_angle) <= 90)
                    {
                        tor_move_dir = ToroidMoveDir.Ahead;
                    }
                    else if ((quarter_ctrl == 3 && quarter_tank == 0)
                        || (quarter_ctrl == 0 && quarter_tank == 3))
                    {
                        if (Mathf.Abs(diffr_angle) >= 270)
                        {
                            tor_move_dir = ToroidMoveDir.Ahead;
                        }
                        else
                        {
                            tor_move_dir = ToroidMoveDir.Back;
                        }
                    }
                    else
                    {
                        tor_move_dir = ToroidMoveDir.Back;
                    }
                }
            }

            if (ctrl_world_angle == tank_world_angle)
            {
                rotate = ToroidRotate.Stop;
            }
            else if (tor_move_dir == ToroidMoveDir.Ahead)
            {
                if (Mathf.Abs(ctrl_world_angle - tank_world_angle) <= 90)
                {
                    rotate = (ctrl_world_angle > tank_world_angle) ? ToroidRotate.Right : ToroidRotate .Left;
                }
                else if ((quarter_ctrl == 3 && quarter_tank == 0)
                    || (quarter_ctrl == 0 && quarter_tank == 3))
                {
                    if (Mathf.Abs(diffr_angle) >= 270)
                    {
                        rotate = (ctrl_world_angle > tank_world_angle) ? ToroidRotate.Left : ToroidRotate.Right;
                    }
                    else
                    {
                        if (Mathf.Abs(diffr_angle) < 180) rotate = (diffr_angle < 0) ? ToroidRotate.Left : ToroidRotate.Right;
                        else rotate = (diffr_angle < 0) ? ToroidRotate.Right : ToroidRotate.Left;
                    }
                }
                else
                {
                    if (Mathf.Abs(diffr_angle) < 180) rotate = (diffr_angle < 0) ? ToroidRotate.Left : ToroidRotate.Right;
                    else rotate = (diffr_angle < 0) ? ToroidRotate.Right : ToroidRotate.Left;
                }
            }
            else if (tor_move_dir == ToroidMoveDir.Back)
            {
                if (Mathf.Abs(diffr_angle) < 180) rotate = (diffr_angle < 0) ? ToroidRotate.Right : ToroidRotate.Left;
                else rotate = (diffr_angle < 0) ? ToroidRotate.Left : ToroidRotate.Right;
            }

            //Debug.Log(string.Format("tank_angle = {0}, control_angle = {1}, diffr_angle = {2}, rotate = {3}", tank_world_angle, ctrl_world_angle, diffr_angle, rotate));
            //Debug.Log(string.Format("ctrl_world_angle = {0}, rotate = {1}", ctrl_world_angle, rotate));

            return rotate;
        }

        ToroidRotate FrontViewMode__GetForwardAndRotate(int ctrl_world_angle)
        {
            ToroidRotate rotate = ToroidRotate.Stop;

            int quarter_ctrl = ctrl_world_angle / 90;

            //if (EnNavStat.Idle == enNavStat)
            {
                enNavStat = EnNavStat.Active;

                if (ctrl_world_angle > 330
                    || ctrl_world_angle < 30)
                {
                    tor_move_dir = ToroidMoveDir.Ahead;
                    rotate = ToroidRotate.Stop;
                }
                else if (ctrl_world_angle >= 30
                    && ctrl_world_angle < 60)
                {
                    tor_move_dir = ToroidMoveDir.Ahead;
                    rotate = ToroidRotate.Right;
                }
                else if (ctrl_world_angle >= 60
                    && ctrl_world_angle < 120)
                {
                    tor_move_dir = ToroidMoveDir.Stop;
                    rotate = ToroidRotate.Right;
                }
                else if (ctrl_world_angle >= 120
                    && ctrl_world_angle < 150)
                {
                    tor_move_dir = ToroidMoveDir.Back;
                    rotate = ToroidRotate.Right;
                }
                else if (ctrl_world_angle >= 150
                    && ctrl_world_angle < 210)
                {
                    tor_move_dir = ToroidMoveDir.Back;
                    rotate = ToroidRotate.Stop;
                }
                else if (ctrl_world_angle >= 210
                    && ctrl_world_angle < 240)
                {
                    tor_move_dir = ToroidMoveDir.Back;
                    rotate = ToroidRotate.Left;
                }
                else if (ctrl_world_angle >= 240
                    && ctrl_world_angle < 330)
                {
                    tor_move_dir = ToroidMoveDir.Ahead;
                    rotate = ToroidRotate.Left;
                }
            }

            //Debug.Log(string.Format("forward = {0}, rotate = {1}", forward, rotate));

            return rotate;
        }


        void HandleTouchNavigation(GameModeEnumerator.CameraMode GameModeCameraMode)
        {
            //angle in the control element
            int ctrl_world_angle = (int)ipTankInputs.NavigationToroidalAngle;

            //get the tank angle
            int tank_world_angle = (int)GetTankHorizontalAngleInWorld();

            ToroidRotate rotate = (GameModeEnumerator.CameraMode.RadarView == GameModeCameraMode)
                ? TopViewMode__GetForwardAndRotate(ctrl_world_angle, tank_world_angle)
                : FrontViewMode__GetForwardAndRotate(ctrl_world_angle);


            int rotation = (ToroidRotate.Left == rotate)
                ? -1 : (ToroidRotate.Right == rotate)
                ? 1 : 0;
            Quaternion wantedRotation = _transform.rotation * Quaternion.Euler(Vector3.up * tankRotationSpeed * rotation * Time.deltaTime);
            _rigidBody.MoveRotation(wantedRotation);

            //move tank
            if (tor_move_dir__collided == tor_move_dir
                && ToroidMoveDir.Stop != tor_move_dir)
            {
                tor_move_dir = ToroidMoveDir.Stop;
            }

            int moving = (ToroidMoveDir.Ahead == tor_move_dir)
                ? 1 : (ToroidMoveDir.Back == tor_move_dir)
                ? -1 : 0;
            Vector3 wantedPosition = _transform.position + (_transform.forward * moving * get_tank_speed(ipTankInputs.NavigationToroidalGearNum) * Time.deltaTime);
            _rigidBody.MovePosition(wantedPosition);

            moveUnderCondition = EnMoveUnderCondition.KeyPressed;
        }

        void HandleKeyPressNavigation()
        {
            //move tank forwards or backwards

            int moving = ipTankInputs.ForwardInput;
            if (ipTankInputs.ForwardInput == keyb_move_dir__collided
                && ipTankInputs.ForwardInput != 0)
            {
                moving = 0;
            }

            float gear =    1.0f;
            Vector3 wantedPosition = _transform.position + (_transform.forward * moving * get_tank_speed(gear) * Time.deltaTime); // inasmuchas we're calling it from FixedUpdate, we do not need deltaTime
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
