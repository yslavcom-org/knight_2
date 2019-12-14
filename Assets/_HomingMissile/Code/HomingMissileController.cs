using UnityEngine;
using System;

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
        public float minDistanceToTarget = 0f; // this is the minimum distance required to fire a missle
        public const float maxDistanceToTarget = float.MaxValue;
        public const float upright_threshold = 0.8f;
        public const float look_at_target_angle = 5f;
        RaycastHit hit;

        public float _missileSpeed = 10f;

        private IndiePixel.Cameras.IP_Minimap_Camera homingMissileTrackingCamera;
        private Transform targetTransform;
        private Rigidbody homingMissile;

        [SerializeField]
        private HomingMissile homingMissileSm;

        private MyTankGame.IObjectId launcherObjId;
        #endregion

        #region Built in Methods
        // Start is called before the first frame update
        void Start()
        {
            targetTransform = null;

            homingMissileSm = HomingMissile.Idle;

            homingMissile = GetComponent<Rigidbody>();

            DeactivateMissile();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            Navigate();
        }

        private void OnTriggerEnter(Collider other) // it collided with an object
        {
            if (other.gameObject.transform.tag == HardcodedValues.StrActiveDefence)
            {//hit active defense object
                SelfDestruct(out homingMissileSm);
            }
        }
        #endregion

        #region Custom Methods

        private float DistanceToTarget(Vector3 targetPosition)
        {
            return Vector3.Distance(targetPosition, transform.position); 
        }

        private bool BoValidDistanceToTarget(Transform targetTransform)
        {
            var distance = DistanceToTarget(targetTransform.position);
            if (distance >= minDistanceToTarget
                && distance <= maxDistanceToTarget)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Launch(MyTankGame.IObjectId launcherId, Vector3 startPosition, Transform targetTransform, IndiePixel.Cameras.IP_Minimap_Camera homingMissileTrackingCamera)
        {
            if (BoValidDistanceToTarget(targetTransform))
            {
                if (null != startPosition)
                {
                    gameObject.transform.SetPositionAndRotation(startPosition, new Quaternion());
                }
                launcherObjId = launcherId;

                ActivateMissile();

                this.targetTransform = targetTransform;
                homingMissileSm = HomingMissile.Idle;

                this.homingMissileTrackingCamera = homingMissileTrackingCamera;
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
                && targetTransform == null)
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
                        if (targetTransform != null)
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
                            launchPointCoord = homingMissile.transform.position; // get the coordinate of the launching point
                        }
                        SetCameraPosition();
                    }
                    break;

                case HomingMissile.GainHeight:
                    {
                        var cur_position = homingMissile.position;
                        homingMissile.position = new Vector3(cur_position.x, cur_position.y + _missileSpeed * Time.deltaTime, cur_position.z);

                        var curCoord = transform.position;
                        if ((launchPointCoord.y + gainHeight) <= curCoord.y)
                        {
                            homingMissileSm = HomingMissile.AlignToTarget;
                            EventMissileLaunched();
                        }

                        IsAboutToCollidWithSomething();
                        SetCameraPosition();
                    }
                    break;

                case HomingMissile.AlignToTarget:
                    {
                        homingMissile.transform.LookAt(targetTransform.position);

                        var angle = Vector3.Angle(homingMissile.transform.forward, targetTransform.position - homingMissile.transform.position);
                        if (angle <= look_at_target_angle)
                        {
                            homingMissileSm = HomingMissile.HeadToTarget;
                        }

                        IsAboutToCollidWithSomething();
                        SetCameraPosition();
                    }
                    break;

                case HomingMissile.HeadToTarget:
                    {
                        homingMissile.transform.LookAt(targetTransform.position);
                        homingMissile.transform.Translate(targetTransform.position * Time.deltaTime);

                        IsAboutToCollidWithSomething();
                        SetCameraPosition();
                    }
                    break;

                case HomingMissile.HitTarget:
                    Hit(out homingMissileSm);
                    break;

                case HomingMissile.DestructSelf:
                    SelfDestruct(out homingMissileSm);
                    break;

                case HomingMissile.Explosion:
#if false
                    StartCoroutine("ExplodeMissile");
#else
                    EventMissileHitsAndBlowsTarget();
                    EventMissileTerminated();
                    DeactivateMissile();
                    homingMissileSm = HomingMissile.Destroyed;
#endif
                    break;

                case HomingMissile.Destroyed:
                    //do nothing here
                    break;

                default:
                    break;
            }
        }

#if false
        IEnumerator ExplodeMissile()
        {
            EventMissileHitsAndBlowsTarget();
            yield return new WaitForSeconds(0.2f);
            EventMissileTerminated();
            yield return new WaitForSeconds(0.2f);
            DeactivateMissile();
            yield return new WaitForSeconds(0.2f);
            homingMissileSm = HomingMissile.Destroyed;
        }
#endif
        void SetCameraPosition()
        {
            if (null != homingMissileTrackingCamera)
            {
                homingMissileTrackingCamera.SetCameraPosition(transform.position);
            }
        }


        bool IsAboutToCollidWithSomething()
        {
            const float watchoutDistance = 10f;
            const float hitDistance = 0.1f;// 2f; // distance to target triggering the explosion

            bool boCheckLinecast = Physics.Linecast(homingMissile.transform.position, targetTransform.position, out hit);
            if (boCheckLinecast)
            {
                if (hit.distance <= watchoutDistance
                    && DistanceToTarget(targetTransform.position) > (watchoutDistance * 2f))
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
                    if (hitIds == null)
                    {
                        //ignore
                    }
                    else if (hitIds[0] == null)
                    {
                        //ignore
                    }
                    else if (launcherObjId == null)
                    {
                        //ignore
                    }
                    else if (hitIds[0].GetId() == launcherObjId.GetId())
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


        private void EventMissileLaunched()
        {
            EventManager.TriggerEvent(HardcodedValues.evntName__missileLaunched, targetTransform);
            
        }

        private void EventMissileHitsAndBlowsTarget()
        {
            EventManager.TriggerEvent(HardcodedValues.evntName__missileBlowsUp, gameObject.transform);
        }

        private void EventMissileTerminated()
        {
            EventManager.TriggerEvent(HardcodedValues.evntName__missileDestroyed, gameObject.transform);
        }

#endregion
    }
}
