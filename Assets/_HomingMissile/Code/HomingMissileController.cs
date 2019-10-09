using System.Collections;
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
            Explosion,
            Destroyed
        };
        #endregion

        #region Variables
        public const float turn = 20f;
        public float _minDistanceToTarget = 0f; // this is the minimum distance required to fire a missle
        public const float _maxDistanceToTarget = float.MaxValue;
        public const float upright_threshold = 0.8f;
        public const float look_at_target_angle = 5f;
        RaycastHit hit;

        public float _missileSpeed = 10f;

        private Vector3 _targetPosition;
        private Rigidbody _homingMissile;

        [SerializeField]
        private HomingMissile homingMissileSm;

        private MyTankGame.IObjectId launcherObjId;
        #endregion


        #region Built in Methods
        // Start is called before the first frame update
        void Start()
        {
            _targetPosition = new Vector3(0, 0, 0);

            homingMissileSm = HomingMissile.Idle;

            _homingMissile = GetComponent<Rigidbody>();

            DeactivateMissile();
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

        private float DistanceToTarget(Vector3 targetPosition)
        {
            return Vector3.Distance(targetPosition, transform.position); 
        }

        private bool BoValidDistanceToTarget(Vector3 targetPosition)
        {
            var distance = DistanceToTarget(targetPosition);
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

        public void Launch(MyTankGame.IObjectId launcherId, Vector3 startPosition, Vector3 targetPosition)
        {
            if (BoValidDistanceToTarget(targetPosition))
            {
                if (null != startPosition)
                {
                    gameObject.transform.SetPositionAndRotation(startPosition, new Quaternion());
                }
                launcherObjId = launcherId;

                ActivateMissile();

                _targetPosition = targetPosition;
                homingMissileSm = HomingMissile.Idle;
            }
        }


        private void Hit(out HomingMissile StateMachineAfterHit) // hit the object
        {
            SelfDestruct(out StateMachineAfterHit);
        }

        private void SelfDestruct(out HomingMissile StateMachineAfterHit) 
        {
            DestroyThisMissile(out StateMachineAfterHit);
        }

        private void DestroyThisMissile(out HomingMissile StateMachineAfterHit)
        {
            StateMachineAfterHit = HomingMissile.Explosion;
        }

        private bool BoMissionTerminated()
        {
            return HomingMissile.Destroyed == homingMissileSm;
        }

        private void ActivateMissile()
        {
            gameObject.SetActive(true);
        }

        private void DeactivateMissile()
        {
            gameObject.SetActive(false);
        }

        private Vector3 launchPointCoord;
        const float gainHeight = 5f;//20f // gain this height from the launching point before aligning to the target            
        private void Navigate()
        {
            if (HomingMissile.Idle != homingMissileSm
                && !BoMissionTerminated()
                && _targetPosition == (new Vector3(0, 0, 0)))
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
                        const float speed = 3.5f;
                        Quaternion q = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
                        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * speed);
                        if (IsUpright()) // check if it is pointing upwards
                        {
                            homingMissileSm = HomingMissile.GainHeight;
                            launchPointCoord = _homingMissile.transform.position; // get the coordinate of the launching point
                        }
                    }
                    break;

                case HomingMissile.GainHeight:
                    {
                        var cur_position = _homingMissile.position;
                        _homingMissile.position = new Vector3(cur_position.x, cur_position.y + _missileSpeed * Time.deltaTime, cur_position.z);

                        var curCoord = transform.position;
                        if ((launchPointCoord.y + gainHeight) <= curCoord.y)
                        {
                            homingMissileSm = HomingMissile.AlignToTarget;
                            SetCameraToThisMissileCamera();
                        }

                        IsCollidedSomething();
                    }
                    break;

                case HomingMissile.AlignToTarget:
                    {
                        _homingMissile.transform.LookAt(_targetPosition);

                        var angle = Vector3.Angle(_homingMissile.transform.forward, _targetPosition - _homingMissile.transform.position);
                        if (angle <= look_at_target_angle)
                        {
                            homingMissileSm = HomingMissile.HeadToTarget;
                        }

                        IsCollidedSomething();
                    }
                    break;

                case HomingMissile.HeadToTarget:
                    {
                        _homingMissile.transform.LookAt(_targetPosition);
                        _homingMissile.transform.Translate(_targetPosition * Time.deltaTime);

                        IsCollidedSomething();
                    }
                    break;

                case HomingMissile.HitTarget:
                    Hit(out homingMissileSm);
                    break;

                case HomingMissile.DestructSelf:
                    SelfDestruct(out homingMissileSm);
                    break;

                case HomingMissile.Explosion:
                    StartCoroutine("ExplodeMissile");
                    break;

                case HomingMissile.Destroyed:
                    //do nothing here
                    break;

                default:
                    break;
            }
        }

        IEnumerator ExplodeMissile()
        {
            MissileHitsAndBlowsTarget();
            //yield return new WaitForSeconds(0.5f);
            ReleaseThisMissileCamera();
            //yield return new WaitForSeconds(0.5f);
            homingMissileSm = HomingMissile.Destroyed;
            DeactivateMissile();

            yield return new WaitForSeconds(0.5f);
        }

        bool IsCollidedSomething()
        {
            const float watchoutDistance = 10f; 
            const float hitDistance = 2f; // distance to target triggering the explosion

            bool boCheckLinecast = Physics.Linecast(_homingMissile.transform.position, _targetPosition, out hit);
            if (boCheckLinecast)
            {
                if(hit.distance <= watchoutDistance
                    && DistanceToTarget(_targetPosition) > (watchoutDistance * 2f))
                {
#if false
                    if (GameTargetsOfPlayer.IsStaticObstacle(hit.collider.tag))
                    {//try gain more height to flight over the obstacle
                        var cur_position = _homingMissile.position;
                        _homingMissile.position = new Vector3(cur_position.x, cur_position.y + _missileSpeed * Time.deltaTime, cur_position.z);
                    }
#endif
                }
                else if (hit.distance <= hitDistance
                    && hit.collider.attachedRigidbody != null)
                {

                    var hitIds = hit.collider.gameObject.GetComponentsInParent<MyTankGame.IObjectId>();
                    if (hitIds[0]?.GetId() == launcherObjId?.GetId())
                    {
                        //ignore
                    }
                    else if (GameTargetsOfPlayer.IsValidTarget(hit.collider.attachedRigidbody?.tag))
                    {
                        homingMissileSm = HomingMissile.HitTarget;
                    }
                    else
                    {
                        homingMissileSm = HomingMissile.DestructSelf;
                    }

                    return true;
                }
            }

            return false;
        }

        private bool IsUpright()
        {
            bool isUpright = transform.up.y > upright_threshold;
            return isUpright;
        }

        public const string evntName__missileLaunched = "missileLaunch";
        public const string evntName__missileDestroyed = "missileTerminate";
        public const string evntName__missileBlowsUp = "SphereBlowsUp";
        private void SetCameraToThisMissileCamera()
        {
            EventManager.TriggerEvent(evntName__missileLaunched, gameObject.transform);
        }

        private void MissileHitsAndBlowsTarget()
        {
            EventManager.TriggerEvent(evntName__missileBlowsUp, gameObject.transform.position);
        }

        private void ReleaseThisMissileCamera()
        {
            
            EventManager.TriggerEvent(evntName__missileDestroyed, null);
        }

#endregion
    }
}
