using System.Collections;
using System.Collections.Generic;
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
        [SerializeField]
        private string validTarget = "Enemy";
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

        public void ShootGun(ref Camera cam, bool boThisRadarMode, TankDemo.IP_Tank_Inputs ipTankInputs)
        {
            if (ipTankInputs.BoFireGun)
            {
                ipTankInputs.FireGunAck();
                if (boThisRadarMode)
                { // launch missile using radar
                    if (null != tankLaunchHomingMissile)
                    {
                        if (null != radar)
                        {
                            tankLaunchHomingMissile.Launch(radar);
                        }
                    }
                }
                else  if(null != cam)
                { // shoot with gun
                    if (cam.gameObject.activeSelf)
                    {
                        bool boHitSomething = MyTankGame.ShootRaycast.BoRaycastHit(cam, _gunWeaponRange, out Vector3 hitPosition, out Vector3 hitNormal, out Collider hitCollider);
                        if (boHitSomething)
                        {
                           //do something
                           if (IsValidTarget(hitCollider, out Rigidbody targetRigidBody))
                           {
                               targetRigidBody.AddForce(cam.transform.forward * _shootGunHitForce);
                           
                               Debug.Log("ShootGun hit " + hitCollider.tag);
                           }
                        }
                    }
                }
            }
        }

        bool IsValidTarget(Collider hitCollider, out Rigidbody targetRigidBody)
        {
            if (null != hitCollider.attachedRigidbody)
            {
                if (hitCollider.attachedRigidbody.tag == validTarget)
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
