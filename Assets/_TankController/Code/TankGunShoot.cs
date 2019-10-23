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

        public static Func<string, bool> OnCheckValidGunTarget;
        public static Action<bool> OnGunLockedTarget = delegate { };

        #endregion

        #region Built-in Methods
        private void Start()
        {
            tankLaunchHomingMissile = gameObject.GetComponent<MyTankGame.TankLaunchHomingMissile>();
        }
        #endregion

        #region Custom Methods
        public void SetRadar(Radar rad)
        {
            radar = rad;
        }

        public void SetGunParams(float gunWeaponRange, float shootGunHitForce)
        {
            _gunWeaponRange = gunWeaponRange;
            _shootGunHitForce = shootGunHitForce;
        }

        private bool Shoot_GunLockTarget(ref Camera cam, out Rigidbody targetRigidBody)
        {
            bool is_locked = false;


            if (null == cam)
            {
                targetRigidBody = null;
                return is_locked;
            }

            if (!cam.gameObject.activeSelf)
            {
                targetRigidBody = null;
                return is_locked;
            }

            bool boHitSomething = MyTankGame.ShootRaycast.BoRaycastHit(cam, _gunWeaponRange, out Vector3 hitPosition, out Vector3 hitNormal, out Collider hitCollider);

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

        private void Shoot_RadarMode(TankDemo.IP_Tank_Inputs ipTankInputs)
        {
            if (ipTankInputs.BoFireGun)
            {
                ipTankInputs.FireGunAck();
                if (null != tankLaunchHomingMissile)
                {
                    if (null != radar)
                    {
                        tankLaunchHomingMissile.Launch(radar);
                    }
                }
            }
        }

        private void Shoot_GunMode(ref Camera cam, TankDemo.IP_Tank_Inputs ipTankInputs, ref Rigidbody targetRigidBody)
        {
            if (ipTankInputs.BoFireGun)
            {
                ipTankInputs.FireGunAck();
                targetRigidBody.AddForce(cam.transform.forward * _shootGunHitForce);
                ITankGunDamageable iTankGunDamageable = targetRigidBody.GetComponent<ITankGunDamageable>();
                if (null != iTankGunDamageable)
                {
                    iTankGunDamageable.GunShootsThisObject(Vector3.zero, null);
                }
            }
        }

        public void TankOpensFire(ref Camera cam, bool boThisRadarMode, TankDemo.IP_Tank_Inputs ipTankInputs)
        {
            if (boThisRadarMode)
            { // launch missile using radar
                Shoot_RadarMode(ipTankInputs);
            }
            else
            { // shoot with gun
                bool is_locked = Shoot_GunLockTarget(ref cam, out Rigidbody targetRigidBody);
                OnGunLockedTarget(is_locked);
                if (is_locked)
                {
                    Shoot_GunMode(ref cam, ipTankInputs, ref targetRigidBody);
                }
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


        #endregion
    }
}
