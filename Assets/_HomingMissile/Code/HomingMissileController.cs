using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTankGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class HomingMissileController : MonoBehaviour
    {
        #region Enums
        enum HomingMissile
        {
            Idle,
            Start, 
            GainHeight,
            AlignToTarget,
            HeadToTarget,
            HitTarget,
            DestructSelf,
            Destroyed
        };
        #endregion

        #region Variables
        public const float turn = 20f;
        public float _minDistanceToTarget = 0f; // this is the minimum distance required to fire a missle
        public const float _maxDistanceToTarget = float.MaxValue;
        public const float upright_threshold = 0.8f;
        public const float look_at_target_angle = 5f;

        public float _missileSpeed = 10f;

        private Vector3 _targetPosition;
        private Rigidbody _homingMissile;
        
        [SerializeField]
        private HomingMissile homingMissileSm;
        #endregion


        #region Built in Methods
        // Start is called before the first frame update
        void Start()
        {
            _targetPosition = new Vector3(0,0,0);

            homingMissileSm = HomingMissile.Idle;

            _homingMissile = GetComponent<Rigidbody>();

            gameObject.SetActive(false); // set the missile to the inactive state
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            Navigate();
        }

        private void OnCollisionEnter(Collision collision) // it collided with an object
        {
      //      Hit(out homingMissileSm);
        }
        #endregion

        #region Custom Methods
        private bool BoValidDistanceToTarget(Vector3 targetPosition)
        {
            var distance = Vector3.Distance(targetPosition, transform.position);
            if (distance >= _minDistanceToTarget
                && distance <= _maxDistanceToTarget)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Launch(Vector3 startPosition,  Vector3 targetPosition)
        {
            if (BoValidDistanceToTarget(targetPosition))
            {
                if(null != startPosition)
                {
                    gameObject.transform.SetPositionAndRotation(startPosition, new Quaternion());
                }

                gameObject.SetActive(true); // set the missile to the active state

                SetCameraToThisMissileCamera();

                _targetPosition = targetPosition;
                homingMissileSm = HomingMissile.Idle;
            }
        }

        public void Explode() // epxlode itself and the objects around
        {

        }

        private void Hit(out HomingMissile StateMachineAfterHit) // hit the object
        {
            Explode();
            DestroyThisMissile(out StateMachineAfterHit);
        }

        private void DestroyThisMissile(out HomingMissile StateMachineAfterHit)
        {
            ReleaseThisMissileCamera();

            StateMachineAfterHit = HomingMissile.Destroyed;
            gameObject.SetActive(false); // set the object inactive again

        }

        private Vector3 launchPointCoord;
        const float gainHeight = 5f;//20f // gain this height from the launching point before aligning to the target            
        const float hitDistance = 2f; // distance to target triggering the explosion
        private void Navigate()
        {
            if (HomingMissile.Idle != homingMissileSm
                && HomingMissile.Destroyed != homingMissileSm
                && _targetPosition == (new Vector3(0,0,0)))
            {
                
                {//target was detroyed before the missile hit it. 
                    //start the missile self-destruction
                    homingMissileSm = HomingMissile.DestructSelf;
                }
            }

            switch (homingMissileSm)
            {
                case HomingMissile.Idle:
                    {
                        if (_targetPosition != (new Vector3(0, 0, 0)))
                        {
                            homingMissileSm = HomingMissile.Start;
                        }
                    }
                    break;

                case HomingMissile.Start:
                    {//point to vertical direction
#if true
                        const float speed = 3.5f;
                        Quaternion q = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
                        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * speed);
                        if (IsUpright()) // check if it is pointing upwards
                        {
                            homingMissileSm = HomingMissile.GainHeight;
                            launchPointCoord = _homingMissile.transform.position; // get the coordinate of the launching point
                        }
#else
                        _homingMissile.transform.LookAt(_homingMissile.transform.position + new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0));
                        if(IsUpright()) // check if it is pointing upwards
                        {
                            homingMissileSm = HomingMissile.GainHeight;
                            launchPointCoord = _homingMissile.transform.position; // get the coordinate of the launching point
                        }
#endif
                    }
                    break;

                case HomingMissile.GainHeight:
                    {

                        //_homingMissile.velocity = transform.forward * _missileSpeed;
                        var cur_position = _homingMissile.position;
                        _homingMissile.position = new Vector3(cur_position.x, cur_position.y + _missileSpeed * Time.deltaTime, cur_position.z);

                        var curCoord = transform.position;
                        if ((launchPointCoord.y + gainHeight )<= curCoord.y)
                        {
                            homingMissileSm = HomingMissile.AlignToTarget;
                        }
                    }
                    break;

                case HomingMissile.AlignToTarget:
                    {
                        //var targetRotation = Quaternion.LookRotation(_targetPosition - transform.position);

                        //_homingMissile.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, turn));
                        _homingMissile.transform.LookAt(_targetPosition);

                        var angle = Vector3.Angle(_homingMissile.transform.forward, _targetPosition - _homingMissile.transform.position);
                        if (angle <= look_at_target_angle)
                        {
                            homingMissileSm = HomingMissile.HeadToTarget;
                        }
                    }
                    break;

                case HomingMissile.HeadToTarget:
                    {
                        _homingMissile.transform.LookAt(_targetPosition);
                        _homingMissile.transform.Translate(_targetPosition * Time.deltaTime);

                        var distance = Vector3.Distance(_targetPosition, _homingMissile.transform.position);
                        if (distance <= hitDistance)
                        {
                            homingMissileSm = HomingMissile.HitTarget;
                        }
                    }
                    break;

                case HomingMissile.HitTarget:
                    Hit(out homingMissileSm);
                    break;

                case HomingMissile.DestructSelf:
                    Hit(out homingMissileSm);
                    break;

                case HomingMissile.Destroyed:
                    //do nothing here
                    break;

                default:
                    break;
            }

            //_homingMissile.velocity = transform.forward * _missileSpeed;
        }

        private bool IsUpright()
        {
            bool isUpright = transform.up.y > upright_threshold;
            return isUpright;
        }

        public const string evntName__missileLaunched = "missileLaunch";
        public const string evntName__missileDestroyed = "missileDestroy";
        private void SetCameraToThisMissileCamera()
        {
            EventManager.TriggerEvent(evntName__missileLaunched, gameObject.transform);
        }

        private void ReleaseThisMissileCamera()
        {
            EventManager.TriggerEvent(evntName__missileDestroyed, null);
        }

        #endregion
    }
}
