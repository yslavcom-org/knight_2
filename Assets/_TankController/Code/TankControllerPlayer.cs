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
    public class TankControllerPlayer : MonoBehaviour
    {
        private Rigidbody rb;

        private TankDemo.IP_Tank_Inputs ipTankInputs;
        private TankDemo.IP_Tank_Controller ipTankController;
        private MyTankGame.Tank_Navigation tankNavigation;
        private MyTankGame.TankGunShoot tankGunShoot;
        private MyTankGame.TankLaunchHomingMissile tankLaunchHomingMissile;

        public Camera trackCamera; // public scene camera
        private Camera sniperCamera; // this camera is attached to the tank
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

            ipTankController = GetComponent<TankDemo.IP_Tank_Controller>();
            ipTankController.SetParams(transform, rb, ipTankInputs, tankNavigation, tankGunShoot,
                 defTankSpeed, maxTankSpeed, speedStep, tankRotationSpeed);

            tankLaunchHomingMissile = GetComponent<MyTankGame.TankLaunchHomingMissile>();

            sniperCamera = gameObject.GetComponentInChildren<Camera>();
        }

        public void SetTrackCamera(Camera cam)
        {
            ipTankInputs.SetTrackCamera(cam);
        }

        public void SetThisPlayerMode(bool isPlayer)
        {
            boPlayer = isPlayer;
            ipTankInputs.SetThisPlayerMode(isPlayer);
        }

        public void SetSniperCamera(bool isActive)
        {
            sniperCamera?.gameObject.SetActive(isActive);
        }

        public void SetRadar(Radar rad)
        {
            tankGunShoot.SetRadar(rad);
        }

        public void SetThisTag(string tag)
        {
            gameObject.tag = tag;
        }       
    }
}
