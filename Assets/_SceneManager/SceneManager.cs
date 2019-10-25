using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(HealthBarController))]
public class SceneManager : MonoBehaviour
{
    private struct Tank
    {
        public GameObject tank;
        public MyTankGame.TankControllerPlayer tankHandle;

        public GameObject destroyedTank__missile;
        public MyTankGame.HomingMissileDamage tankDestroyedHandle__missile;

        public GameObject destroyedTank__gun;
        public MyTankGame.TankGunDamage tankDestroyedHandle__gun;
    };

    #region Var Events
    private UnityAction<object> listenerHomingMissileLaunched;
    private UnityAction<object> listenerHomingMissileDestroyed;
    #endregion

    #region Var delegates
    private event Action<Vector3> OhHomingMissileTerminated = delegate { }; // position of the missile which was terminated
    #endregion

    #region Var player & enemy objects
    public GameObject tankPrefab;
    public GameObject destroyedTankPrefab__missile;
    public GameObject destroyedTankPrefab__gun;

    // player objects (main player & enemy objects)
    //Tank playerTank;
    //Tank []enemyTanks;
    int id__playerTank;
    private Dictionary <int, Tank> tankCollection;
    private Vector3[] enemyTankStartPosition;
    public Camera trackTopCamera;
    private Radar radar;
    #endregion

    #region manager to switch between game modes
    private MyTankGame.GameModeManager gameModeManager;
    #endregion

    #region  Canvas
    private Button buttonCameras;
    [SerializeField]
    private string buttonCamerasName = "ButtonCameras";
    public GameObject[] gunnerCamControls;

    [SerializeField]
    private  HealthBarController healthBarControllerPrefab;
    private  HealthBarController healthBarController;
    [SerializeField]
    private HealthBar healthBarPrefab;
    private const float healthBarPositionOffset = 2f;
    #endregion

    #region explosion dispatcher
    private ExplosionDispatcher explosionDispatcher;
    public GameObject blowUpAnimationPrefab;
    public GameObject sphereBlowUpPrefab;
    #endregion

    private void OnDestroy()
    {
        OhHomingMissileTerminated -= OhHomingMissileTerminated__tanks;
        MyTankGame.TankGunShoot.OnCheckValidGunTarget -= OnCheckValidGunTarget;
        if (tankCollection.TryGetValue(id__playerTank, out Tank playerTank))
        {
            var tankGunShoot = GetComponentInChildren<MyTankGame.TankGunShoot>();
            if (null != tankGunShoot)
            {
                tankGunShoot.OnGunLockedTarget -= OnGunLockedTarget;
            }
        }
    }

    // we need it for this class a a singleton
    public static SceneManager Instance { get; private set; }
    private void Init()
    {
        if (Instance == null)
        {
            Instance = this;

            radar = FindObjectOfType<Radar>();

            OhHomingMissileTerminated += OhHomingMissileTerminated__tanks;
            MyTankGame.TankGunShoot.OnCheckValidGunTarget += OnCheckValidGunTarget;

            healthBarController = Instantiate(healthBarControllerPrefab);
            healthBarController.SetPrefab(healthBarPrefab);
            Health.OnHealthZero += OnHealthZero;

            InitEvents();
            InitTanks();

            if (tankCollection.TryGetValue(id__playerTank, out Tank playerTank))
            {
                var tankGunShoot = playerTank.tankHandle.GetComponentInChildren<MyTankGame.TankGunShoot>();
                if (null != tankGunShoot)
                {
                    tankGunShoot.OnGunLockedTarget += OnGunLockedTarget;
                }
            }

            InitGameModeManager(); // call after InitTanks
            InitExplosionDispatcher();

            UpdateReferencesToCamera(trackTopCamera);
        }
    }

    void InitEvents()
    {
        listenerHomingMissileLaunched = new UnityAction<object>(HomingMissilwWasLaunched);
        listenerHomingMissileDestroyed = new UnityAction<object>(HomingMissilwWasTerminated);
    }

    public const int enemyTanksCount = 4;
    void InitTanks()
    {
        tankCollection = new Dictionary<int, Tank>();

        Tank playerTank = new Tank();
        playerTank.tank = Instantiate(tankPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

        playerTank.tankHandle = playerTank.tank.GetComponent<MyTankGame.TankControllerPlayer>();
        playerTank.tankHandle.Init(trackTopCamera);
        playerTank.tankHandle.SetThisPlayerMode(true);
        playerTank.tankHandle.SetGunCamera(false);
        playerTank.tankHandle.SetThisTag("Player");
        playerTank.tankHandle.SetThisName("CoolTank");
        var missilePoll = playerTank.tank.GetComponentsInChildren<MyTankGame.HomingMissilePool>();
        playerTank.tankHandle.SetMissilePoolAndDispatcher(missilePoll);

        playerTank.tankHandle.SetRadar(radar);
        radar?.SetPlayer(playerTank.tank);
        playerTank.tankHandle.makeRadarObject?.DeregisterFromRadarAsTarget();

        var camHandle = trackTopCamera.GetComponent<IndiePixel.Cameras.IP_TopDown_Camera>();
        camHandle.SetTarget(playerTank.tank.transform);

        id__playerTank = playerTank.tankHandle.GetHashCode();
        playerTank.tankHandle.SetId(id__playerTank); // set the unique object id
        InitDestroyedCopyOfTanks__Missile(ref playerTank);
        InitDestroyedCopyOfTanks__Gun(ref playerTank);
        tankCollection.Add(playerTank.tankHandle.GetId(), playerTank);

        //create array of enemy tanks
        enemyTankStartPosition = new Vector3[enemyTanksCount] {
                new Vector3(-9.41f, -2.45f, 12.54f),
                new Vector3(-13.92f, -2.45f, 12.54f),
                new Vector3(-18f, -2.45f, 12.54f),
                new Vector3(-24f, -2.45f, 12.54f)
            };

        Tank[] enemyTanks = new Tank[enemyTanksCount];

        for (int tank_idx = 0; tank_idx < enemyTanks.Length; ++tank_idx)
        {
            enemyTanks[tank_idx].tank = Instantiate(tankPrefab, enemyTankStartPosition[tank_idx], Quaternion.identity) as GameObject;

            enemyTanks[tank_idx].tankHandle = enemyTanks[tank_idx].tank.GetComponent<MyTankGame.TankControllerPlayer>();
            enemyTanks[tank_idx].tankHandle.Init(null/*no track camera for enemy vehicles*/, enemyTankStartPosition[tank_idx]);
            enemyTanks[tank_idx].tankHandle.SetThisPlayerMode(false);
            enemyTanks[tank_idx].tankHandle.SetGunCamera(false);
            enemyTanks[tank_idx].tankHandle.SetThisTag("Enemy");
            enemyTanks[tank_idx].tankHandle.SetThisName("Not so cool tank_" + tank_idx);
            enemyTanks[tank_idx].tankHandle.makeRadarObject?.RegisterOnRadarAsTarget();

            enemyTanks[tank_idx].tankHandle.SetId(enemyTanks[tank_idx].tankHandle.GetHashCode()); // set the unique object id
            InitDestroyedCopyOfTanks__Missile(ref enemyTanks[tank_idx]);
            InitDestroyedCopyOfTanks__Gun(ref enemyTanks[tank_idx]);
            tankCollection.Add(enemyTanks[tank_idx].tankHandle.GetId(), enemyTanks[tank_idx]);
        }
    }

    void InitDestroyedCopyOfTanks__Missile(ref Tank tank)
    {
        if (null == destroyedTankPrefab__missile) return;
        tank.destroyedTank__missile = Instantiate(destroyedTankPrefab__missile, new Vector3(0, 0, 0), Quaternion.identity) as GameObject; // cache the destroyed tank, but keep it as inactive
        if (null == tank.destroyedTank__missile) return;
        tank.tankDestroyedHandle__missile = tank.destroyedTank__missile.GetComponent<MyTankGame.HomingMissileDamage>();
        tank.destroyedTank__missile.SetActive(false);
    }

    void InitDestroyedCopyOfTanks__Gun(ref Tank tank)
    {
        if (null == destroyedTankPrefab__gun) return;
        tank.destroyedTank__gun = Instantiate(destroyedTankPrefab__gun, new Vector3(0, 0, 0), Quaternion.identity) as GameObject; // cache the destroyed tank, but keep it as inactive
        if (null == tank.destroyedTank__gun) return;
        tank.tankDestroyedHandle__gun = tank.destroyedTank__gun.GetComponent<MyTankGame.TankGunDamage>();
        tank.destroyedTank__gun.SetActive(false);
    }


    void InitGameModeManager()
    {
        gameModeManager = gameObject.AddComponent<MyTankGame.GameModeManager>();
        var buttons = FindObjectsOfType<Button>();
        foreach(var button in buttons)
        {
            if(button.name == buttonCamerasName)
            {
                buttonCameras = button;
                break;
            }
        }

        gameModeManager.Init(radar, false, buttonCameras?.GetComponentInChildren<Text>(), gunnerCamControls);
    }


    void InitExplosionDispatcher()
    {
        explosionDispatcher = gameObject.AddComponent<ExplosionDispatcher>();
        if(explosionDispatcher)
        {
            explosionDispatcher.Init("SphereBlowsUp", trackTopCamera, blowUpAnimationPrefab, sphereBlowUpPrefab, 1.5f);
        }
    }

    void SetTankDestroyed__Missile(Tank tank)
    {
        var transform = tank.tank.transform;
        transform.Rotate(0, 90, 0);
        tank.destroyedTank__missile.transform.SetPositionAndRotation(tank.tank.transform.position, transform.rotation);
        tank.tank.SetActive(false);
        tank.destroyedTank__missile.SetActive(true);
        tank.tankDestroyedHandle__missile.HomingMissileBlowUp();
    }

    void SetTankDestroyed__Gun(Tank tank)
    {
        var transform = tank.tank.transform;
        transform.Rotate(0, 90, 0);
        tank.destroyedTank__gun.transform.SetPositionAndRotation(tank.tank.transform.position, transform.rotation);
        tank.tank.SetActive(false);
        tank.destroyedTank__gun.SetActive(true);
        tank.tankDestroyedHandle__gun.SetDestroyed();
    }

    void Awake()
    {
        Init();
    }


#region Custom methods
    public void ButtonCamerasWasClicked()
    {
        if (null == gameModeManager) return;
        gameModeManager.ChooseCamera();
     
    }

#endregion

#region Events
    public const string event_name__homing_missile_launched = "missileLaunch";
    public const string event_name__homing_missile_terminated = "missileTerminate";
    public const string event_name__change_camera_mode = "changeCameraMode";
    void OnEnable()
    {
        EventManager.StartListening(event_name__homing_missile_launched, listenerHomingMissileLaunched);
        EventManager.StartListening(event_name__homing_missile_terminated, listenerHomingMissileDestroyed);
        EventManager.StartListening(event_name__change_camera_mode, CameraModeChange);
    }

    void OnDisable()
    {
        EventManager.StopListening(event_name__homing_missile_launched, listenerHomingMissileLaunched);
        EventManager.StopListening(event_name__homing_missile_terminated, listenerHomingMissileDestroyed);
        EventManager.StopListening(event_name__change_camera_mode, CameraModeChange);
    }

    private void HomingMissilwWasLaunched(object arg)
    {
        
    }

    private void HomingMissilwWasTerminated(object arg)
    {
        OhHomingMissileTerminated((Vector3)arg);
    }

    #region objects affected by homing missile
    public const float homingMissileDamageRadius = 5f;
    private void OhHomingMissileTerminated__tanks(Vector3 position)
    {
        if (position == null) return;

        Collider[] colliders = Physics.OverlapSphere(position, homingMissileDamageRadius);
        if (colliders == null) return;
        foreach (var col in colliders)
        {
            var ids = col.GetComponentsInChildren<MyTankGame.IObjectId>();
            if (null == ids
                || ids.Length == 0)
            {
                ids = col.GetComponentsInParent<MyTankGame.IObjectId>();
            }
            if (null == ids
                || ids.Length == 0) continue;

            //I do not like this code, make it generic
            //look up the object using its ID
            int id = ids[0].GetId();
            if (tankCollection.TryGetValue(id, out Tank tempTank))
            {
                SetTankDestroyed__Missile(tempTank);
            }
        }
    }
    #endregion

    #region objects affected by tank gun
    private bool OnCheckValidGunTarget(string str)
    {
        return str == "Enemy";
    }

    private void OnGunLockedTarget(bool is_locked)
    {
        if (null == gameModeManager) return;

        if (gameModeManager.IsTankGunLockTarget != is_locked)
        {
            gameModeManager.IsTankGunLockTarget = is_locked;
            gameModeManager.SetGunCrossHairIfAny();
        }
    }

    #endregion

    #region objects affected by health status
    private void OnHealthZero(Health health)
    {
        if(health==null) return;

        //get the id of the object
        var id = health.GetComponentInParent<MyTankGame.IObjectId>();
        if(id!=null)
        {
            if (tankCollection.TryGetValue(id.GetId(), out Tank tempTank))
            {
                SetTankDestroyed__Gun(tempTank);
            }
        }
    }
    #endregion

    private void CameraModeChange(object arg)
    {
        if (null == arg) return;

        if (tankCollection.TryGetValue(id__playerTank, out Tank playerTank))
        {
            GameModeEnumerator.CameraMode cameraMode = (GameModeEnumerator.CameraMode)arg;
            switch (cameraMode)
            {
                case GameModeEnumerator.CameraMode.SniperView:
                    trackTopCamera.gameObject.SetActive(false);
                    playerTank.tankHandle.SetGunCamera(true);
                    playerTank.tankHandle.IpTankController?.SetGameModeCameraMode(GameModeEnumerator.CameraMode.SniperView);
                    UpdateReferencesToCamera(playerTank.tankHandle.GetGunCamera());
                    break;

                case GameModeEnumerator.CameraMode.RadarView:
                    playerTank.tankHandle.SetGunCamera(false);
                    trackTopCamera.gameObject.SetActive(true);
                    playerTank.tankHandle.IpTankController?.SetGameModeCameraMode(GameModeEnumerator.CameraMode.RadarView);
                    UpdateReferencesToCamera(trackTopCamera);
                    break;

                default:
                    playerTank.tankHandle.SetGunCamera(false);
                    trackTopCamera.gameObject.SetActive(true);
                    playerTank.tankHandle.IpTankController?.SetGameModeCameraMode(GameModeEnumerator.CameraMode.TopView);

                    UpdateReferencesToCamera(trackTopCamera);

                    break;
            }
        }
    }

    private void UpdateReferencesToCamera(Camera cam)
    {
        foreach(var tank in tankCollection)
        {
            healthBarController.SetTrackingCamera(cam);
        }
    }

    #endregion
}
