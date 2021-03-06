﻿using System;
using UnityEngine;

namespace MyTankGame
{
    [RequireComponent(typeof(Transform))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(TankDemo.IP_Tank_Inputs))]
    [RequireComponent(typeof(TankDemo.IP_Tank_Controller))]
    [RequireComponent(typeof(MyTankGame.TankGunShoot))]
    [RequireComponent(typeof(MyTankGame.IObjectId))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Ammunition))]
    [RequireComponent(typeof(Fuel))]
    [RequireComponent(typeof(MyTankGame.HomingMissilePool))]
    [RequireComponent(typeof(MyTankGame.TankLaunchHomingMissile))]
    [RequireComponent(typeof(ForceFieldDomeController))]
    [RequireComponent(typeof(PlayerTurretControl))]
    [RequireComponent(typeof(InventoryItemsManager))]
    
    public class TankControllerPlayer : MonoBehaviour, IObjectId
    {
        private Rigidbody rb;

        private TankDemo.IP_Tank_Inputs ipTankInputs;
        public TankDemo.IP_Tank_Controller IpTankController { get; private set; }
        private MyTankGame.TankGunShoot tankGunShoot;
        IndiePixel.Cameras.IP_Minimap_Camera homingMissileTrackingCamera;

        public Camera trackCamera; // public scene camera
        private Camera tankGunCamera; // this camera is attached to the tank

        public Vector3 customPosition = new Vector3(-3.34f, 0.28f, 5.54f);
        public Quaternion customRotation = new Quaternion(0, 0, 0, 0);
        public Vector3 customScale = new Vector3(1, 1, 1);
        //public float fireGunFreq = 0.25f;
        public float gunWeaponRange = 100f;
        public float shootGunHitForce = 100f;
        public float defTankSpeed = 5f;
        public float maxTankSpeed = 7f;
        public float speedStep = 0.2f;
        public float tankRotationSpeed = 50f;
        public float mass = 5000;
        public float drag = 5;
        public float angularDrag = 0.05f;
        public bool useGravity = true;
        public bool isKinematic = false;

        private bool isHuman;
        private RadarControl.RadarResource radarResource;

        private Health health;
        private Fuel fuel;
        private Ammunition ammunition;

        private InventoryItemsManager inventoryItemsManager;

        private PlayerTurretControl playerTurretControl;

        #region custom methods
        public void Init(Camera cam, IndiePixel.Cameras.IP_Minimap_Camera homingMissileTrackingCamera, Vector3? pos = null, Quaternion? rot = null, Vector3? scale = null)
        {
            if (pos == null)
            {
                pos = customPosition;
            }
            if (rot == null)
            {
                rot = customRotation;
            }
            if (scale == null)
            {
                scale = customScale;
            }

            transform.position = pos.Value;
            transform.rotation = rot.Value;
            transform.localScale = scale.Value;

            rb = GetComponent<Rigidbody>();
            rb.mass = mass;
            rb.drag = drag;
            rb.angularDrag = angularDrag;
            rb.useGravity = useGravity;
            rb.isKinematic = isKinematic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.constraints = RigidbodyConstraints.None;

            ipTankInputs = GetComponent<TankDemo.IP_Tank_Inputs>();
            SetTrackCamera(cam);
            this.homingMissileTrackingCamera = homingMissileTrackingCamera;
            //ipTankInputs.FireGunFrequency(fireGunFreq);

            tankGunShoot = GetComponent<MyTankGame.TankGunShoot>();
            tankGunShoot.SetGunParams(gunWeaponRange, shootGunHitForce);

            IpTankController = GetComponent<TankDemo.IP_Tank_Controller>();
            health = GetComponent<Health>();
            IpTankController.SetParams(transform, rb, ipTankInputs, tankGunShoot,
                 defTankSpeed, maxTankSpeed, speedStep, tankRotationSpeed, health);

            playerTurretControl = GetComponentInChildren<PlayerTurretControl>();
            inventoryItemsManager = GetComponentInParent<InventoryItemsManager>();

            gameObject.AddComponent<ForceFieldDomeController>();
            gameObject.AddComponent<ForceFieldDomePool>();
            gameObject.AddComponent<MyTankGame.HomingMissilePool>();

            //get the sniper/gun shooter camera
            var tankCams = gameObject.GetComponentsInChildren<Camera>();
            if (tankCams != null)
            {
                foreach (var tankCam in tankCams)
                {
                    if(tankCam.name == HardcodedValues.sniperCameraName)
                    {
                        tankGunCamera = tankCam;
                        IpTankController.SetWeaponCameras(tankGunCamera, homingMissileTrackingCamera);
                        break;
                    }
                }
            }

        }

        public bool CheckInventoryAndTryToUseHealthPacket()
        {
            bool result = false;

            if (null == inventoryItemsManager)
            {
                inventoryItemsManager = GetComponentInParent<InventoryItemsManager>();
            }

            if (null != inventoryItemsManager 
                && null != health)
            {
                const int amount_requested = 1;
                int amount_dispatched = inventoryItemsManager.RequestItemsDispatch(HardcodedValues.HealthPackPickUp__ItemId, amount_requested);

                if (0 < amount_dispatched)
                {
                    //use this health pack to recover
                    health.SetStaminaToMaxLevel();

                    return true;
                }
            }

            return result;
        }

        public void CustomInitFuel()
        {
            fuel = GetComponent<Fuel>();
        }

        public void CustomInitAmmunition()
        {
            ammunition = GetComponent<Ammunition>();
        }

        public void SetTrackCamera(Camera cam)
        {
            ipTankInputs.SetTrackCamera(cam);
        }

        public void SetHumanMode(bool isHuman)
        {
            this.isHuman = isHuman;
            ipTankInputs.SetHumanMode(isHuman);
            playerTurretControl.SetHumanMode(isHuman);
        }

        public void SetRadarHandle(GameObject radarObj)
        {
            if (null != radarObj)
            {
                var radarControl = radarObj.GetComponent<RadarControl>();
                if (null != radarControl)
                {
                    radarResource = radarControl.GetRadarResource();
                }
            }
        }

        public void DestroyAudioListener()
        {
            AudioListener al = GetComponent<AudioListener>();
            Destroy(al);//al.gameObject.SetActive(false);
        }

        public void SetGunCamera(bool isActive)
        {
            if (null == tankGunCamera) return;

            tankGunCamera.gameObject.SetActive(isActive);
        }

        public Camera GetGunCamera()
        {
            return tankGunCamera;
        }

        public void SetCrosshair(GameObject crossHair)
        {
            playerTurretControl.SetCrosshair(crossHair);
        }

        public void SetThisTag(string tag)
        {
            gameObject.tag = tag;
        }

        public void SetThisName(string name)
        {
            gameObject.name = name;
        }


        #endregion

        #region Built-in methods
        void Update()
        {
            if ((!isHuman)// || IpTankController.GetGameModeCameraMode() == GameModeEnumerator.CameraMode.RadarView) 
                && null != radarResource)
            {
                var radarObjectsList = radarResource.radarListOfObjects.GetReferenceToListOfObjects();
                if (null == radarObjectsList) return;

                foreach(var obj in radarObjectsList)
                {
                    var transform = obj.owner.transform;
                    if (radarResource.radar.CheckObjectIsInLineOfSight(transform))
                    {
                        playerTurretControl.SetAutomaticGunPoint(transform);
                    }
                }
            }

            if (isHuman)
            {
                playerTurretControl.HumanPointGun();
            }

        }
        #endregion

        #region IObjectId implementation
        int ID;
        public void SetId(int id)
        {
            ID = id;
        }
        public int GetId()
        {
            return ID ;
        }
        #endregion
    }
}
