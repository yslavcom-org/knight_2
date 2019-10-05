using UnityEngine;
using UnityEngine.Events;

public class SceneManager : MonoBehaviour
{
    //events
    private UnityAction<object> listenerHomingMissileLaunched;
    private UnityAction<object> listenerHomingMissileDestroyed;

    //player & enemy objects
    public GameObject tankPrefab;

    // player objects (main player & enemy objects)
    public GameObject playerTank;
    public GameObject[] enemyTanks;
    private Vector3[] enemyTankStartPosition;
    public Camera trackTopCamera;
    private Radar radar;

    //manager to switch between game modes
    private MyTankGame.GameModeManager gameModeManager;

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
        }
    }

    void InitEvents()
    {
        listenerHomingMissileLaunched = new UnityAction<object>(HomingMissilwWasLaunched);
        listenerHomingMissileDestroyed = new UnityAction<object>(HomingMissilwWasDestroyed);
    }

    void InitTanks()
    {
        playerTank = Instantiate(tankPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        var playerHandle = playerTank.GetComponent<MyTankGame.TankControllerPlayer>();
        playerHandle.Init(trackTopCamera);
        playerHandle.SetThisPlayerMode(true);
        playerHandle.SetSniperCamera(false);
        playerHandle.SetThisTag("Player");

        playerHandle.SetRadar(radar);
        radar?.SetPlayer(playerTank);

        var camHandle = trackTopCamera.GetComponent<IndiePixel.Cameras.IP_TopDown_Camera>();
        camHandle.SetTarget(playerTank.transform);

        //create array of enemy tanks
        enemyTankStartPosition = new Vector3[] {
                new Vector3(-9.41f, -2.45f, 12.54f),
                new Vector3(-13.92f, -2.45f, 12.54f),
                new Vector3(-18f, -2.45f, 12.54f),
                new Vector3(-24f, -2.45f, 12.54f)
            };
        enemyTanks = new GameObject[enemyTankStartPosition.Length];

        for (int tank_idx = 0; tank_idx < enemyTanks.Length; ++tank_idx)
        {
            enemyTanks[tank_idx] = Instantiate(tankPrefab, enemyTankStartPosition[tank_idx], Quaternion.identity) as GameObject;
            var enemyHandle = enemyTanks[tank_idx].GetComponent<MyTankGame.TankControllerPlayer>();
            enemyHandle.Init(null/*no track camera for enemy vehicles*/, enemyTankStartPosition[tank_idx]);
            enemyHandle.SetThisPlayerMode(false);
            enemyHandle.SetSniperCamera(false);
            enemyHandle.SetThisTag("Enemy");
        }

    }

    void InitGameModeManager()
    {
        gameModeManager = new MyTankGame.GameModeManager();
        gameModeManager.SetRadar(radar);
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
    public const string event_name__homing_missile_destroyed = "missileDestroy";
    void OnEnable()
    {
        EventManager.StartListening(event_name__homing_missile_launched, listenerHomingMissileLaunched);
        EventManager.StartListening(event_name__homing_missile_destroyed, listenerHomingMissileDestroyed);
    }

    void OnDisable()
    {
        EventManager.StopListening(event_name__homing_missile_launched, listenerHomingMissileLaunched);
        EventManager.StopListening(event_name__homing_missile_destroyed, listenerHomingMissileDestroyed);
    }

    private void HomingMissilwWasLaunched(object arg)
    {
    }

    private void HomingMissilwWasDestroyed(object arg)
    {
    }
    #endregion
}
