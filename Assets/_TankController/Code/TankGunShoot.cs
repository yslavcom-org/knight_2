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

        private bool Shoot_GunLockTarget(Vector3 rayOrigin, Vector3 direction, out Rigidbody targetRigidBody)
        {
            bool is_locked = false;

            bool boHitSomething = MyTankGame.ShootRaycast.BoRaycastHit(rayOrigin, direction, _gunWeaponRange, out Vector3 hitPosition, out Vector3 hitNormal, out Collider hitCollider);

            if (boHitSomething)
            {
                //do something
                is_locked = IsValidTarget(hitCollider, out targetRigidBody);
                return is_locked;
            }
            else
            {
                targetRigidBody = null;
                return is_locked;
            }
           
        }

        private void Shoot_RadarMode(TankDemo.IP_Tank_Inputs ipTankInputs, ref IndiePixel.Cameras.IP_Minimap_Camera homingMissileTrackingCamera)
        {
            if (ipTankInputs.BoFireGun)
            {
                ipTankInputs.FireGunAck();

                if (null == tankLaunchHomingMissile) return;
                if (null == radar) return;

                if (IfHasHomingMissile())
                {
                    tankLaunchHomingMissile.Launch(radar, ref homingMissilePool, ref homingMissileTrackingCamera);
                }
            }
        }

        private void Shoot_GunMode(Vector3 direction, TankDemo.IP_Tank_Inputs ipTankInputs, ref Rigidbody targetRigidBody)
        {
            if (ipTankInputs.BoFireGun)
            {
                if (null != controlMuzzle)
                {
                    controlMuzzle.PlayTrails();
                }

                ipTankInputs.FireGunAck();
                targetRigidBody.AddForce(direction * _shootGunHitForce);
                ITankGunDamageable iTankGunDamageable = targetRigidBody.GetComponent<ITankGunDamageable>();
                if (null != iTankGunDamageable)
                {
                    iTankGunDamageable.GunShootsThisObject(Vector3.zero, null);
                }
            }
        }

        private void tankUsesGun(Vector3 rayOrigin, Vector3 direction, TankDemo.IP_Tank_Inputs ipTankInputs)
        {
            bool is_locked = Shoot_GunLockTarget(rayOrigin, direction, out Rigidbody targetRigidBody);
            OnGunLockedTarget(is_locked);
            if (is_locked)
            {
                Shoot_GunMode(direction, ipTankInputs, ref targetRigidBody);
            }
        }

        public void TankUsesWeapons(ref Camera cam, ref IndiePixel.Cameras.IP_Minimap_Camera homingMissileTrackingCamera, GameModeEnumerator.CameraMode GameModeCameraMode, TankDemo.IP_Tank_Inputs ipTankInputs)
        {
            if (GameModeCameraMode == GameModeEnumerator.CameraMode.RadarView)
            { // launch missile using radar
                Shoot_RadarMode(ipTankInputs, ref homingMissileTrackingCamera);
                OnGunLockedTarget(false);
            }
            else if (GameModeCameraMode == GameModeEnumerator.CameraMode.SniperView)
            { // shoot with gun
              //cam is only active in the sniper mode

                if (null != cam)
                {
                    Vector3 rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
                    Vector3 direction = cam.transform.forward;

                    tankUsesGun(rayOrigin, direction, ipTankInputs);
                }
            }
            else
            {
                OnGunLockedTarget(false);
            }
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
