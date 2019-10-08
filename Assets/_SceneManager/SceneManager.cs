using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    #region Var Events
    private UnityAction<object> listenerHomingMissileLaunched;
    private UnityAction<object> listenerHomingMissileDestroyed;
    #endregion

    #region Var player & enemy objects
    public GameObject tankPrefab;

    // player objects (main player & enemy objects)
    public GameObject playerTank;
    MyTankGame.TankControllerPlayer playerHandle;
    public GameObject[] enemyTanks;
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
        }
    }

    void InitEvents()
    {
        listenerHomingMissileLaunched = new UnityAction<object>(HomingMissilwWasLaunched);
        listenerHomingMissileDestroyed = new UnityAction<object>(HomingMissilwWasTerminated);
    }

    void InitTanks()
    {
        playerTank = Instantiate(tankPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        playerHandle = playerTank.GetComponent<MyTankGame.TankControllerPlayer>();
        playerHandle.Init(trackTopCamera);
        playerHandle.SetThisPlayerMode(true);
        playerHandle.SetSniperCamera(false);
        playerHandle.SetThisTag("Player");
        playerHandle.SetThisName("CoolTank");
        var missilePoll = playerTank.GetComponentsInChildren<MyTankGame.HomingMissilePool>();
        playerHandle.SetMissilePoolAndDispatcher(missilePoll);

        playerHandle.SetRadar(radar);
        radar?.SetPlayer(playerTank);
        playerHandle.makeRadarObject?.DeregisterFromRadarAsTarget();

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
            enemyHandle.SetThisName("Not so cool tank_" + tank_idx);
            enemyHandle.makeRadarObject?.RegisterOnRadarAsTarget();
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

        gameModeManager.Init(radar, false, buttonCameras?.GetComponentInChildren<Text>());
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
        //Debug.Log("HomingMissilwWasLaunched");
    }

    private void HomingMissilwWasTerminated(object arg)
    {
        //Debug.Log("HomingMissilwWasTerminated");
    }

    private void CameraModeChange(object arg)
    {
        if (null == arg) return;
        GameModeEnumerator.CameraMode cameraMode = (GameModeEnumerator.CameraMode)arg;
        switch (cameraMode)
        {
            case GameModeEnumerator.CameraMode.SniperView:
                trackTopCamera.gameObject.SetActive(false);
                playerHandle.SetSniperCamera(true);
                playerHandle.IpTankController?.SetRadarMode(false);
                break;

            case GameModeEnumerator.CameraMode.RadarView:
                playerHandle.SetSniperCamera(false);
                trackTopCamera.gameObject.SetActive(true);
                playerHandle.IpTankController?.SetRadarMode(true);
                break;

            default:
                playerHandle.SetSniperCamera(false);
                trackTopCamera.gameObject.SetActive(true);
                playerHandle.IpTankController?.SetRadarMode(false);

                break;
        }
    }
    #endregion
}
