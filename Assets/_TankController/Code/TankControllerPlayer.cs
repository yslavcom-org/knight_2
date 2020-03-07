using System;
using UnityEngine;

namespace MyTankGame
{
    [RequireComponent(typeof(Transform))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(TankDemo.IP_Tank_Inputs))]
    [RequireComponent(typeof(TankDemo.IP_Tank_Controller))]
        [RequireComponent(typeof(MyTankGame.TankGunShoot))]
    [RequireComponent(typeof(MakeRadarObject))]
    [RequireComponent(typeof(MyTankGame.IObjectId))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Ammunition))]
    [RequireComponent(typeof(Fuel))]
    [RequireComponent(typeof(MyTankGame.HomingMissilePool))]
    [RequireComponent(typeof(MyTankGame.TankLaunchHomingMissile))]
    [RequireComponent(typeof(ForceFieldDomeController))]
    public class TankControllerPlayer : MonoBehaviour, IObjectId
    {
        private Rigidbody rb;

        private TankDemo.IP_Tank_Inputs ipTankInputs;
        public TankDemo.IP_Tank_Controller IpTankController { get; private set; }
        private MyTankGame.TankGunShoot tankGunShoot;
        IndiePixel.Cameras.IP_Minimap_Camera homingMissileTrackingCamera;
        public MakeRadarObject makeRadarObject { get; private set; }

        public Camera trackCamera; // public scene camera
        private Camera tankGunCamera; // this camera is attached to the tank
        public bool boPlayer = false;

        public Vector3 customPosition = new Vector3(-3.34f, 0.28f, 5.54f);
        public Quaternion customRotation = new Quaternion(0, 0, 0, 0);
        public Vector3 customScale = new Vector3(1, 1, 1);
        public float fireGunFreq = 0.25f;
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

        private Health health;
        private Fuel fuel;
        private Ammunition ammunition;

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
            ipTankInputs.FireGunFrequency(fireGunFreq);

            tankGunShoot = GetComponent<MyTankGame.TankGunShoot>();
            tankGunShoot.SetGunParams(gunWeaponRange, shootGunHitForce);

            IpTankController = GetComponent<TankDemo.IP_Tank_Controller>();
            health = GetComponent<Health>();
            IpTankController.SetParams(transform, rb, ipTankInputs, tankGunShoot,
                 defTankSpeed, maxTankSpeed, speedStep, tankRotationSpeed, health);

            makeRadarObject = GetComponent<MakeRadarObject>();

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

        public void SetThisPlayerMode(bool isPlayer)
        {
            boPlayer = isPlayer;
            ipTankInputs.SetThisPlayerMode(isPlayer);
            if(!isPlayer)
            {
                AudioListener al = GetComponent<AudioListener>();
                Destroy(al);//al.gameObject.SetActive(false);
            }
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

        public void SetRadar(Radar rad)
        {
            tankGunShoot.SetRadar(rad);
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
