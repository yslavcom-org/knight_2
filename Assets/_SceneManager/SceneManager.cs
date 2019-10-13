using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    private struct Tank
    {
        public GameObject tank;
        public MyTankGame.TankControllerPlayer tankHandle;

        public GameObject destroyedTank__missile;
        public MyTankGame.HomingMissileDamage tankDestroyedHandle__missile;
    };

    #region Var Events
    private UnityAction<object> listenerHomingMissileLaunched;
    private UnityAction<object> listenerHomingMissileDestroyed;
    #endregion

    #region Var player & enemy objects
    public GameObject tankPrefab;
    public GameObject destroyedTankPrefab__missile;

    // player objects (main player & enemy objects)
    Tank playerTank;
    Tank []enemyTanks;
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
    #endregion

    #region explosion dispatcher
    private ExplosionDispatcher explosionDispatcher;
    public GameObject blowUpAnimationPrefab;
    public GameObject sphereBlowUpPrefab;
    #endregion

    // we need it for this class a a singleton
    public static SceneManager Instance { get; private set; }
    private void Init()
    {
        if (Instance == null)
        {
            Instance = this;

            radar = FindObjectOfType<Radar>();

            InitEvents();
            InitTanks();
            InitGameModeManager(); // call after InitTanks
            InitExplosionDispatcher();
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

        playerTank.tank = Instantiate(tankPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        playerTank.tankHandle = playerTank.tank.GetComponent<MyTankGame.TankControllerPlayer>();
        playerTank.tankHandle.Init(trackTopCamera);
        playerTank.tankHandle.SetThisPlayerMode(true);
        playerTank.tankHandle.SetSniperCamera(false);
        playerTank.tankHandle.SetThisTag("Player");
        playerTank.tankHandle.SetThisName("CoolTank");
        var missilePoll = playerTank.tank.GetComponentsInChildren<MyTankGame.HomingMissilePool>();
        playerTank.tankHandle.SetMissilePoolAndDispatcher(missilePoll);

        playerTank.tankHandle.SetRadar(radar);
        radar?.SetPlayer(playerTank.tank);
        playerTank.tankHandle.makeRadarObject?.DeregisterFromRadarAsTarget();

        var camHandle = trackTopCamera.GetComponent<IndiePixel.Cameras.IP_TopDown_Camera>();
        camHandle.SetTarget(playerTank.tank.transform);

        playerTank.tankHandle.SetId(playerTank.tankHandle.GetHashCode()); // set the unique object id
        tankCollection.Add(playerTank.tankHandle.GetId(), playerTank);

        //create array of enemy tanks
        enemyTankStartPosition = new Vector3[enemyTanksCount] {
                new Vector3(-9.41f, -2.45f, 12.54f),
                new Vector3(-13.92f, -2.45f, 12.54f),
                new Vector3(-18f, -2.45f, 12.54f),
                new Vector3(-24f, -2.45f, 12.54f)
            };
        enemyTanks = new Tank[enemyTanksCount];

        for (int tank_idx = 0; tank_idx < enemyTanks.Length; ++tank_idx)
        {
            enemyTanks[tank_idx].tank = Instantiate(tankPrefab, enemyTankStartPosition[tank_idx], Quaternion.identity) as GameObject;
            enemyTanks[tank_idx].tankHandle = enemyTanks[tank_idx].tank.GetComponent<MyTankGame.TankControllerPlayer>();
            enemyTanks[tank_idx].tankHandle.Init(null/*no track camera for enemy vehicles*/, enemyTankStartPosition[tank_idx]);
            enemyTanks[tank_idx].tankHandle.SetThisPlayerMode(false);
            enemyTanks[tank_idx].tankHandle.SetSniperCamera(false);
            enemyTanks[tank_idx].tankHandle.SetThisTag("Enemy");
            enemyTanks[tank_idx].tankHandle.SetThisName("Not so cool tank_" + tank_idx);
            enemyTanks[tank_idx].tankHandle.makeRadarObject?.RegisterOnRadarAsTarget();

            enemyTanks[tank_idx].tankHandle.SetId(enemyTanks[tank_idx].tankHandle.GetHashCode()); // set the unique object id
            tankCollection.Add(enemyTanks[tank_idx].tankHandle.GetId(), enemyTanks[tank_idx]);
        }

        InitDestroyedCopiesOfTanks();
    }

    void InitDestroyedCopiesOfTanks()
    {
        if (null == destroyedTankPrefab__missile) return;
        playerTank.destroyedTank__missile = Instantiate(destroyedTankPrefab__missile, new Vector3(0, 0, 0), Quaternion.identity) as GameObject; // cache the destroyed tank, but keep it as inactive
        if (null == playerTank.destroyedTank__missile) return;
        playerTank.destroyedTank__missile.SetActive(false);
        playerTank.tankDestroyedHandle__missile = playerTank.destroyedTank__missile.GetComponent<MyTankGame.HomingMissileDamage>();


        for (int i = 0; i < enemyTanksCount; i++)
        {
            enemyTanks[i].destroyedTank__missile = Instantiate(destroyedTankPrefab__missile, new Vector3(0, 0, 0), Quaternion.identity) as GameObject; // cache the destroyed tank, but keep it as inactive
            enemyTanks[i].destroyedTank__missile.SetActive(false);

            enemyTanks[i].tankDestroyedHandle__missile = enemyTanks[i].destroyedTank__missile.GetComponent<MyTankGame.HomingMissileDamage>();
        }
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

    void SetTankDestroyed(Tank tank)
    {
        tank.destroyedTank__missile.transform.SetPositionAndRotation(tank.tank.transform.position, tank.tank.transform.rotation);
        tank.tank.SetActive(false);
        tank.destroyedTank__missile.SetActive(true);
        tank.tankDestroyedHandle__missile.HomingMissileBlowUp();
    }

    void Awake()
    {
        Init();
    }


#region Custom methods
    public void ButtonCamerasWasClicked()
    {
        gameModeManager?.ChooseCamera();
     
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

    public const float homingMissileDamageRadius = 5f;
    private void HomingMissilwWasTerminated(object arg)
    {
        if (arg == null) return;
        Vector3 position = (Vector3)arg;
        Collider[] colliders = Physics.OverlapSphere(position, homingMissileDamageRadius);
        if (colliders == null) return;
        foreach (var col in colliders)
        {
            var ids = col.GetComponents <MyTankGame.IObjectId>();
            if (null == ids) continue;

            //I do not like this code, make it generic
            //look up the object using its ID
            int id = ids[0].GetId();
            if (tankCollection.TryGetValue(id, out Tank tempTank))
            {
                SetTankDestroyed(tempTank);
            }
        }
        
    }

    private void CameraModeChange(object arg)
    {
        if (null == arg) return;
        GameModeEnumerator.CameraMode cameraMode = (GameModeEnumerator.CameraMode)arg;
        switch (cameraMode)
        {
            case GameModeEnumerator.CameraMode.SniperView:
                trackTopCamera.gameObject.SetActive(false);
                playerTank.tankHandle.SetSniperCamera(true);
                playerTank.tankHandle.IpTankController?.SetRadarMode(false);
                break;

            case GameModeEnumerator.CameraMode.RadarView:
                playerTank.tankHandle.SetSniperCamera(false);
                trackTopCamera.gameObject.SetActive(true);
                playerTank.tankHandle.IpTankController?.SetRadarMode(true);
                break;

            default:
                playerTank.tankHandle.SetSniperCamera(false);
                trackTopCamera.gameObject.SetActive(true);
                playerTank.tankHandle.IpTankController?.SetRadarMode(false);

                break;
        }
    }
#endregion
}
