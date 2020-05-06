using System;
using UnityEngine;

namespace MyTankGame
{
    public class TankGunShoot : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        private float _gunWeaponRange = 100f;
        [SerializeField]
        private float _shootGunHitForce = 100f;
        private MyTankGame.TankLaunchHomingMissile tankLaunchHomingMissile;
        private Radar radar;
        private InventoryItemsManager inventoryItemsManager;
        private MyTankGame.HomingMissilePool homingMissilePool;

        public static Func<string, bool> OnCheckValidGunTarget;
        public Action<bool> OnGunLockedTarget = delegate { };

        ControlMuzzle controlMuzzle;

        #endregion

        #region Built-in Methods
        private void Start()
        {
            tankLaunchHomingMissile = gameObject.GetComponent<MyTankGame.TankLaunchHomingMissile>();
            if (tankLaunchHomingMissile == null) return;

            inventoryItemsManager = gameObject.GetComponentInChildren<InventoryItemsManager>();
            if (inventoryItemsManager == null) return;

            homingMissilePool = inventoryItemsManager.GetComponent<MyTankGame.HomingMissilePool>();

            radar = GetComponentInChildren<Radar>();

            controlMuzzle = GetComponentInChildren<ControlMuzzle>();
        }
        #endregion

        #region Custom Methods

        public void SetGunParams(float gunWeaponRange, float shootGunHitForce)
        {
            _gunWeaponRange = gunWeaponRange;
            _shootGunHitForce = shootGunHitForce;
        }

        private bool GunLockTarget(Vector3 rayOrigin, Vector3 direction, Action<bool> OnGunLockedTargetCb, out Rigidbody targetRigidBody)
        {
            bool is_locked = false;

            bool boHitSomething = MyTankGame.ShootRaycast.BoRaycastHit(rayOrigin, direction, _gunWeaponRange, out Vector3 hitPosition, out Vector3 hitNormal, out Collider hitCollider);

            if (boHitSomething)
            {
                //do something
                is_locked = IsValidTarget(hitCollider, out targetRigidBody);

                OnGunLockedTargetCb?.Invoke(is_locked);
                return is_locked;
            }
            else
            {
                targetRigidBody = null;

                OnGunLockedTargetCb?.Invoke(is_locked);
                return is_locked;
            }
        }

        private void Shoot_RadarMode(ref IndiePixel.Cameras.IP_Minimap_Camera homingMissileTrackingCamera)
        {
            if (null == tankLaunchHomingMissile) return;
            if (null == radar) return;

            if (IfHasHomingMissile())
            {
                tankLaunchHomingMissile.Launch(radar, ref homingMissilePool, ref homingMissileTrackingCamera);
            }
        }

        private void tankShootsWithGun(Vector3 direction, ref Rigidbody targetRigidBody)
        {
            if (null != controlMuzzle)
            {
                controlMuzzle.PlayTrails();
            }

            if (null != targetRigidBody)
            {
                targetRigidBody.AddForce(direction * _shootGunHitForce);
                ITankGunDamageable iTankGunDamageable = targetRigidBody.GetComponent<ITankGunDamageable>();
                if (null != iTankGunDamageable)
                {
                    iTankGunDamageable.GunShootsThisObject(Vector3.zero, null);
                }
            }
        }

        //use gun
        private void tankUsesGun(Vector3 rayOrigin, Vector3 direction, bool boPulledTrigger, Action<bool> OnGunLockedTargetCb)
        {
            GunLockTarget(rayOrigin, direction, OnGunLockedTargetCb, out Rigidbody targetRigidBody);

            if (boPulledTrigger)
            {
                tankShootsWithGun(direction, ref targetRigidBody);
            }
        }

        void GunPointToTarget(ref Camera cam, GameModeEnumerator.CameraMode GameModeCameraMode, out Vector3 rayOrigin, out Vector3 direction)
        {
            if (GameModeCameraMode == GameModeEnumerator.CameraMode.SniperView
                && null != cam)
            {
                rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
                direction = cam.transform.forward;
            }
            else
            {
                rayOrigin = controlMuzzle.transform.position;
                direction = controlMuzzle.transform.forward;
            }
        }

        //any weapon which is available, such as gun or homing missile
        public void HumanTankUsesWeapons(ref Camera cam, ref IndiePixel.Cameras.IP_Minimap_Camera homingMissileTrackingCamera, GameModeEnumerator.CameraMode GameModeCameraMode, TankDemo.IP_Tank_Inputs ipTankInputs)
        {
            if (ipTankInputs.BoFireMissle)
            {
                ipTankInputs.FireMissileAck();
                Shoot_RadarMode(ref homingMissileTrackingCamera);
            }
            else if (ipTankInputs.BoFireGun)
            {
                bool pulledTrigger = false;
                if (ipTankInputs.BoFireGun)
                {
                    ipTankInputs.FireGunAck();
                    pulledTrigger = true;
                }

                GunPointToTarget(ref cam, GameModeCameraMode, out Vector3 rayOrigin, out Vector3 direction);
                tankUsesGun(rayOrigin, direction, pulledTrigger, OnGunLockedTarget);
            }

            if (GameModeCameraMode == GameModeEnumerator.CameraMode.RadarView)
            { // launch missile using radar
                OnGunLockedTarget(false);
            }
            else if (GameModeCameraMode == GameModeEnumerator.CameraMode.SniperView)
            { // shoot with gun
              //cam is only active in the sniper mode
                GunPointToTarget(ref cam, GameModeCameraMode, out Vector3 rayOrigin, out Vector3 direction);
                GunLockTarget(rayOrigin, direction, OnGunLockedTarget, out Rigidbody targetRigidBody);
            }
            else
            {
                OnGunLockedTarget(false);
            }
        }

        public void NonHumanTankUsesWeapons()
        {
            Vector3 rayOrigin = controlMuzzle.transform.position;
            Vector3 direction = controlMuzzle.transform.forward;

            bool pulledTrigger = TestPullTrigger();
            if (pulledTrigger)
            {
                tankUsesGun(rayOrigin, direction, pulledTrigger, null);
            }
        }

        //test
        float trigger_time_in_seconds = 0;
        const float interval = 5;
        bool TestPullTrigger()
        {
            bool result = false;

            //timer
            float time_in_seconds = GetTime.TimeSinceStartFloat();

            if (time_in_seconds >= (trigger_time_in_seconds + interval))
            {
                result = true;
                trigger_time_in_seconds = time_in_seconds;
            }

            return result;
        }


        bool IsValidTarget(Collider hitCollider, out Rigidbody targetRigidBody)
        {
            if (null != hitCollider.attachedRigidbody
                && null != OnCheckValidGunTarget)
            {
                bool result = OnCheckValidGunTarget(hitCollider.attachedRigidbody.tag);
                if(result)
                { 
                    targetRigidBody = hitCollider.attachedRigidbody;
                    return true;
                }
            }
       
            targetRigidBody = null;
            return false;
        }

        bool IfHasHomingMissile()
        {
            if (homingMissilePool == null) return false;

            return homingMissilePool.Enabled;
        }


#endregion
    }
}
