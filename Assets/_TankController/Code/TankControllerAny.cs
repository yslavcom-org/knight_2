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
    public class TankControllerAny : MonoBehaviour
    {
        private Rigidbody rb;

        private TankDemo.IP_Tank_Inputs ipTankInputs;
        private TankDemo.IP_Tank_Controller ipTankController;
        private MyTankGame.Tank_Navigation tankNavigation;
        private MyTankGame.TankGunShoot tankGunShoot;
        private MyTankGame.TankLaunchHomingMissile tankLaunchHomingMissile;

        private bool isInitialized = false;

        #region constructor
        public TankControllerAny(Camera trackCamera, Vector3? pos = null, Quaternion? rot = null, Vector3? scale = null)
        {
            Init(trackCamera, pos, rot, scale);
        }
        #endregion

        protected void Init(Camera trackCamera, Vector3? pos = null, Quaternion? rot = null, Vector3? scale = null)
        {
            if (isInitialized) return;
            isInitialized = true;

            if (pos == null)
            {
                pos = new Vector3(-3.34f, 0.28f, 5.54f);
            }
            if (rot == null)
            {
                rot = new Quaternion(0, 0, 0, 0);
            }
            if (scale == null)
            {
                scale = new Vector3(1, 1, 1);
            }

            transform.position = pos.Value;
            transform.rotation = rot.Value;
            transform.localScale = scale.Value;

#if true
            rb = GetComponent<Rigidbody>();
            rb.mass = 5000;
            rb.drag = 5;
            rb.angularDrag = 0.05f;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.constraints = RigidbodyConstraints.None;
#else
            rb = new Rigidbody
            {
                mass = 5000,
                drag = 5,
                angularDrag = 0.05f,
                useGravity = true,
                isKinematic = false,
                interpolation = RigidbodyInterpolation.Interpolate,
                collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic,
                constraints = RigidbodyConstraints.None
            };
#endif

            ipTankInputs = new TankDemo.IP_Tank_Inputs(trackCamera, "FreeSpaceKeyPressed");
            ipTankInputs.FireGunFrequency(0.25f);

            ipTankController = new TankDemo.IP_Tank_Controller
            {
                defTankSpeed = 5f,
                maxTankSpeed = 7f,
                speedStep = 0.2f,
                tankRotationSpeed = 50f
            };

            tankNavigation = new MyTankGame.Tank_Navigation();
            tankGunShoot = new MyTankGame.TankGunShoot
            {
                _gunWeaponRange = 100,
                shootGunHitForce = 100
            };

            tankLaunchHomingMissile = new MyTankGame.TankLaunchHomingMissile();
        }

        virtual public void SetTrackCamera(Camera cam)
        {
            ipTankInputs.SetTrackCamera(cam);
        }
    }
}
