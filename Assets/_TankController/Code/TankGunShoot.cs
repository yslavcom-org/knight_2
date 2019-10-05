using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTankGame
{
    public class TankGunShoot : MonoBehaviour
    {
        #region Variables
        private float _gunWeaponRange = 100f;
        private float _shootGunHitForce = 100f;
        private MyTankGame.TankLaunchHomingMissile tankLaunchHomingMissile;
        private Radar radar;
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
                        Vector3 hitPosition;
                        Vector3 hitNormal;
                        Collider hitCollider;
                        bool boHitSomething = MyTankGame.ShootRaycast.BoRaycastHit(cam, _gunWeaponRange, out hitPosition, out hitNormal, out hitCollider);
                        if (boHitSomething)
                        {
                            //do something
                            Rigidbody targetRigidBody;
                            if (ValidTarget(hitCollider, out targetRigidBody))
                            {
                                targetRigidBody.AddForce(cam.transform.forward * _shootGunHitForce);

                                Debug.Log("ShootGun hit " + hitCollider.tag);
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
