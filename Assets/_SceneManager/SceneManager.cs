using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(StaminaBarController))]
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
    public Camera trackPlayerTopCamera;
    public Camera vfxTopCamera; // service camera for fun effects such as missile tracking
    private IndiePixel.Cameras.IP_Minimap_Camera vfxTopCameraHandle;
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
    private  StaminaBarController indicatorBarControllerPrefab; // prefab
    private  StaminaBarController indicatorBarController; // use in this module
    [SerializeField]
    private StaminaBar healthBarPrefab;
    [SerializeField]
    private StaminaBar fuelBarPrefab;
    [SerializeField]
    private StaminaBar ammunitionBarPrefab;
    #endregion

    #region Inventory
    public GameObject inventory;
    public GameObject slotHolder;
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

            vfxTopCameraHandle = vfxTopCamera.GetComponent<IndiePixel.Cameras.IP_Minimap_Camera>();
            vfxTopCamera.gameObject.SetActive(false);


            OhHomingMissileTerminated += OhHomingMissileTerminated__tanks;
            MyTankGame.TankGunShoot.OnCheckValidGunTarget += OnCheckValidGunTarget;

            indicatorBarController = Instantiate(indicatorBarControllerPrefab);
            indicatorBarController.SetStaminaBarPrefab(StaminaBarController.BarType.Health, healthBarPrefab);
            Health.OnBarZero += OnBarZero;

            indicatorBarController.SetStaminaBarPrefab(StaminaBarController.BarType.Fuel, fuelBarPrefab);
            indicatorBarController.SetStaminaBarPrefab(StaminaBarController.BarType.Ammunition, ammunitionBarPrefab);

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

            UpdateReferencesToCamera(trackPlayerTopCamera);
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

        //add inventor component
        playerTank.tank.AddComponent<GameInventory.Inventory>();
        //add inventory manager 
        playerTank.tank.AddComponent<InventoryItemsManager>();

        //var newGameObject = new GameObject("GameObject");
        //newGameObject.AddComponent<InventoryItemsManager>();
        //newGameObject.transform.parent = playerTank.tank.transform;

        var playerTankInventory = playerTank.tank.GetComponent<GameInventory.Inventory>();
        playerTankInventory.inventoryObj = inventory;
        playerTankInventory.slotHolder = slotHolder;
        playerTankInventory.inventoryEnabled = true;

        playerTank.tankHandle = playerTank.tank.GetComponent<MyTankGame.TankControllerPlayer>();
        playerTank.tankHandle.Init(trackPlayerTopCamera, vfxTopCameraHandle);
        playerTank.tankHandle.SetThisPlayerMode(true);
        playerTank.tankHandle.SetGunCamera(false);
        playerTank.tankHandle.SetThisTag("Player");
        playerTank.tankHandle.SetThisName("CoolTank");

        playerTank.tankHandle.SetRadar(radar);
        radar?.SetPlayer(playerTank.tank);
        playerTank.tankHandle.makeRadarObject?.DeregisterFromRadarAsTarget();

        var camHandle = trackPlayerTopCamera.GetComponent<IndiePixel.Cameras.IP_TopDown_Camera>();
        camHandle.SetTarget(playerTank.tank.transform);

        id__playerTank = playerTank.tankHandle.GetHashCode();
        playerTank.tankHandle.SetId(id__playerTank); // set the unique object id
        InitDestroyedCopyOfTanks__Missile(ref playerTank);
        InitDestroyedCopyOfTanks__Gun(ref playerTank);
        tankCollection.Add(playerTank.tankHandle.GetId(), playerTank);

        

        //create array of enemy tanks
        enemyTankStartPosition = new Vector3[enemyTanksCount] {
                new Vector3(-10f, -2.45f, 12.54f),
                new Vector3(-25f, -2.45f, 12.54f),
                new Vector3(-40f, -2.45f, 12.54f),
                new Vector3(-55f, -2.45f, 12.54f)
            };

        Tank[] enemyTanks = new Tank[enemyTanksCount];

        for (int tank_idx = 0; tank_idx < enemyTanks.Length; ++tank_idx)
        {
            enemyTanks[tank_idx].tank = Instantiate(tankPrefab, enemyTankStartPosition[tank_idx], Quaternion.identity) as GameObject;

            enemyTanks[tank_idx].tankHandle = enemyTanks[tank_idx].tank.GetComponent<MyTankGame.TankControllerPlayer>();
            enemyTanks[tank_idx].tankHandle.Init(
                null/*no track camera for enemy vehicles*/, 
                null/*no homing missile track camera for enemy vehicles*/, 
                enemyTankStartPosition[tank_idx]);
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
            explosionDispatcher.Init("SphereBlowsUp", trackPlayerTopCamera, blowUpAnimationPrefab, sphereBlowUpPrefab, 1.5f);
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
        if (null == vfxTopCameraHandle) return;
        if (null == arg) return;

        Transform trnsfrm = (Transform)arg;
        vfxTopCamera.gameObject.SetActive(true);

        vfxTopCameraHandle.SetTarget(trnsfrm);
    }

    private void HomingMissilwWasTerminated(object arg)
    {
        OhHomingMissileTerminated(((Transform)arg).position);
        StartCoroutine(DisableMissileTrackingCamera());
    }

    private IEnumerator DisableMissileTrackingCamera()
    {
        yield return new WaitForSeconds(1.0f);
        vfxTopCamera.gameObject.SetActive(false);
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
    private void OnBarZero(StaminaBarController.BarType type, Health bar)
    {
        if (bar == null) return;

        switch (type)
        {
            case StaminaBarController.BarType.Health:
                {
                    //get the id of the object
                    var id = bar.GetComponentInParent<MyTankGame.IObjectId>();
                    if (id != null)
                    {
                        if (tankCollection.TryGetValue(id.GetId(), out Tank tempTank))
                        {
                            SetTankDestroyed__Gun(tempTank);
                        }
                    }
                }
                break;
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
                    trackPlayerTopCamera.gameObject.SetActive(false);
                    playerTank.tankHandle.SetGunCamera(true);
                    playerTank.tankHandle.IpTankController?.SetGameModeCameraMode(GameModeEnumerator.CameraMode.SniperView);
                    UpdateReferencesToCamera(playerTank.tankHandle.GetGunCamera());
                    break;

                case GameModeEnumerator.CameraMode.RadarView:
                    playerTank.tankHandle.SetGunCamera(false);
                    trackPlayerTopCamera.gameObject.SetActive(true);
                    playerTank.tankHandle.IpTankController?.SetGameModeCameraMode(GameModeEnumerator.CameraMode.RadarView);
                    UpdateReferencesToCamera(trackPlayerTopCamera);
                    break;

                default:
                    playerTank.tankHandle.SetGunCamera(false);
                    trackPlayerTopCamera.gameObject.SetActive(true);
                    playerTank.tankHandle.IpTankController?.SetGameModeCameraMode(GameModeEnumerator.CameraMode.TopView);

                    UpdateReferencesToCamera(trackPlayerTopCamera);

                    break;
            }
        }
    }

    private void UpdateReferencesToCamera(Camera cam)
    {
        foreach(var tank in tankCollection)
        {
            indicatorBarController.SetTrackingCamera(cam);
        }
    }

    #endregion
}
