using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTankGame
{
    public class TankGunShoot : MonoBehaviour
    {
        #region Custom Enumerators
        enum EnRaycastGuns
        {
            First = 0,
            Left = First,
            //Right = 1,
            Num = 1,
        };
        #endregion

        #region Variables
        private float _gunWeaponRange = 100f;
        private float _shootGunHitForce = 100f;

        private Camera[] _gunShootCamera;

        private MyTankGame.GameModeManager gameModeManager;
        private MyTankGame.TankLaunchHomingMissile tankLaunchHomingMissile;
        private Radar radar;

        #endregion

        #region Built-in Methods
        private void Start()
        {
            gameModeManager = FindObjectOfType<MyTankGame.GameModeManager>();
            tankLaunchHomingMissile = FindObjectOfType<MyTankGame.TankLaunchHomingMissile>();

            _gunShootCamera = new Camera[(int)EnRaycastGuns.Num];
            if (null == _gunShootCamera)
            {
                //Debug.LogError("Out of memory, _gunShootCamera");
            }
            else
            {
                var cam = GameObject.Find("CameraGunner__Left").GetComponent<Camera>();
                _gunShootCamera[(int)EnRaycastGuns.Left] = cam;

                //cam = GameObject.Find("CameraGunner__Right").GetComponent<Camera>();
                //_gunShootCamera[(int)EnRaycastGuns.Right] = cam;
            }

            radar = FindObjectOfType<Radar>();
        }
        #endregion

        #region Custom Methods
        public void SetGunParams(float gunWeaponRange, float shootGunHitForce)
        {
            _gunWeaponRange = gunWeaponRange;
            _shootGunHitForce = shootGunHitForce;
        }

        public void ShootGun(TankDemo.IP_Tank_Inputs ipTankInputs)
        {
            bool boThisRadarMode = (gameModeManager != null && gameModeManager.BoRadarMode)
                ? true : false;

            if (ipTankInputs.BoFireGun)
            {
                ipTankInputs.FireGunAck();
                if (boThisRadarMode)
                {
                    if (null == tankLaunchHomingMissile)
                    {
                        tankLaunchHomingMissile = FindObjectOfType<MyTankGame.TankLaunchHomingMissile>();
                    }
                    if (null != tankLaunchHomingMissile)
                    {
                        if (null == radar)
                        {
                            radar = FindObjectOfType<Radar>();
                        }
                        if (null != radar)
                        {
                            tankLaunchHomingMissile.Launch(radar);
                        }
                    }
                }
                else 
                {
                    for (int itr = (int)EnRaycastGuns.First; itr < (int)EnRaycastGuns.Num; itr++)
                    {
                        Vector3 hitPosition;
                        Vector3 hitNormal;
                        Collider hitCollider;
                        bool boHitSomething = MyTankGame.ShootRaycast.BoRaycastHit(_gunShootCamera[itr], _gunWeaponRange, out hitPosition, out hitNormal, out hitCollider);
                        if (boHitSomething)
                        {
                            //do something
                            if (hitCollider.CompareTag("Target"))
                            {
                                hitCollider.attachedRigidbody.AddForce(_gunShootCamera[itr].transform.forward * _shootGunHitForce);

                                //Debug.Log("ShootGun hit " + hitCollider.tag);
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
