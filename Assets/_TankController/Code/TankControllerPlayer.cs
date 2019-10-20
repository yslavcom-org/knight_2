using System;
using UnityEngine;

namespace MyTankGame
{
    [RequireComponent(typeof(Transform))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(TankDemo.IP_Tank_Inputs))]
    [RequireComponent(typeof(TankDemo.IP_Tank_Controller))]
    [RequireComponent(typeof(MyTankGame.Tank_Navigation))]
    [RequireComponent(typeof(MyTankGame.TankGunShoot))]
    [RequireComponent(typeof(MyTankGame.TankLaunchHomingMissile))]
    [RequireComponent(typeof(MakeRadarObject))]
    [RequireComponent(typeof(HomingMissilePoolDispatch))]
    [RequireComponent(typeof(MyTankGame.IObjectId))]
    [RequireComponent(typeof(Health))]
    public class TankControllerPlayer : MonoBehaviour, IObjectId
    {
        private Rigidbody rb;

        private TankDemo.IP_Tank_Inputs ipTankInputs;
        public TankDemo.IP_Tank_Controller IpTankController { get; private set; }
        private MyTankGame.Tank_Navigation tankNavigation;
        private MyTankGame.TankGunShoot tankGunShoot;
        private MyTankGame.TankLaunchHomingMissile tankLaunchHomingMissile;
        public MakeRadarObject makeRadarObject { get; private set; }

        public Camera trackCamera; // public scene camera
        private Camera tankGunCamera; // this camera is attached to the tank
        [SerializeField]
        private readonly string sniperCameraName = "CameraGunner";
        public bool boPlayer = false;

        public Vector3 customPosition = new Vector3(-3.34f, 0.28f, 5.54f);
        public Quaternion customRotation = new Quaternion(0, 0, 0, 0);
        public Vector3 customScale = new Vector3(1, 1, 1);
        public string tankShootEventString = "FreeSpaceKeyPressed";
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

        private HomingMissilePoolDispatch homingMissilePoolDispatch = null;

        #region built-in methods
        private void OnDestroy()
        {
            health.OnHealthZero -= HealthZero;
        }
        #endregion

        #region custom methods
        public void Init(Camera cam, Vector3? pos = null, Quaternion? rot = null, Vector3? scale = null)
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
            ipTankInputs.SetEventString(tankShootEventString);
            ipTankInputs.FireGunFrequency(fireGunFreq);

            tankNavigation = GetComponent<MyTankGame.Tank_Navigation>();
            tankGunShoot = GetComponent<MyTankGame.TankGunShoot>();
            tankGunShoot.SetGunParams(gunWeaponRange, shootGunHitForce);

            IpTankController = GetComponent<TankDemo.IP_Tank_Controller>();
            health = GetComponent<Health>();
            health.OnHealthZero += HealthZero;
            IpTankController.SetParams(transform, rb, ipTankInputs, tankNavigation, tankGunShoot,
                 defTankSpeed, maxTankSpeed, speedStep, tankRotationSpeed, health);

            tankLaunchHomingMissile = GetComponent<MyTankGame.TankLaunchHomingMissile>();

            makeRadarObject = GetComponent<MakeRadarObject>();

            //get the sniper/gun shooter camera
            var tankCams = gameObject.GetComponentsInChildren<Camera>();
            if (tankCams != null)
            {
                foreach (var tankCam in tankCams)
                {
                    if(tankCam.name == sniperCameraName)
                    {
                        tankGunCamera = tankCam;
                        IpTankController.SetGunCamera(tankGunCamera);
                        break;
                    }
                }
            }
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

        public void SetMissilePoolAndDispatcher(MyTankGame.HomingMissilePool []missilePool)
        {
            if (null != homingMissilePoolDispatch) return;

            homingMissilePoolDispatch = gameObject.AddComponent<HomingMissilePoolDispatch>() as HomingMissilePoolDispatch;
            if (null != homingMissilePoolDispatch)
            {
                homingMissilePoolDispatch.Init(missilePool);
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

        private void HealthZero(bool status)
        {
            if(status)
            {
                IHomingMissileDamageable iHomingMissileDamageable = GetComponent<IHomingMissileDamageable>();
                iHomingMissileDamageable.HomingMissileBlowUp();
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
