using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public GameObject tankPrefab;

    //
    public GameObject playerTank;
    public GameObject[] enemyTanks;
    [SerializeField]
    private Vector3[] enemyTankStartPosition;
    public Camera trackTopCamera;
    private Radar radar; 

    public static SceneManager Instance { get; private set; }
    private void Init()
    {
        if (Instance == null)
        {
            Instance = this;
            playerTank = Instantiate(tankPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            var playerHandle = playerTank.GetComponent<MyTankGame.TankControllerPlayer>();
            playerHandle.Init(trackTopCamera);
            playerHandle.SetThisPlayerMode(true);
            playerHandle.SetSniperCamera(false);
            playerHandle.SetThisTag("Player");
            radar = FindObjectOfType<Radar>();

            playerHandle.SetRadar(radar);
            radar?.SetPlayer(playerTank);

            var camHandle = trackTopCamera.GetComponent<IndiePixel.Cameras.IP_TopDown_Camera>();
            camHandle.SetTarget(playerTank.transform);

            //create array of enemy tanks
            enemyTanks = new GameObject[2];
            enemyTankStartPosition = new Vector3[] { new Vector3(-9.41f, -2.45f, 12.54f), new Vector3(-13.92f, -2.45f, 12.54f) };
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
    }

    void Awake()
    {
        Init();
    }
}
