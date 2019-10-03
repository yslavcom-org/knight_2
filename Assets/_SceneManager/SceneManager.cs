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
            playerHandle.SetThisTag("Player");

            var camHandle = trackTopCamera.GetComponent<IndiePixel.Cameras.IP_TopDown_Camera>();
            camHandle.SetTarget(playerTank.transform);

            //create array of 2 enemy tanks
            enemyTanks = new GameObject[2];
            enemyTankStartPosition = new Vector3[] { new Vector3(-9.41f, -2.45f, 12.54f), new Vector3(-13.92f, -2.45f, 12.54f) };
            for (int tank_idx = 0; tank_idx < enemyTanks.Length; ++tank_idx)
            {
                enemyTanks[tank_idx] = Instantiate(tankPrefab, enemyTankStartPosition[tank_idx], Quaternion.identity) as GameObject;
                var enemyHandle = enemyTanks[tank_idx].GetComponent<MyTankGame.TankControllerPlayer>();
                enemyHandle.Init(trackTopCamera, enemyTankStartPosition[tank_idx]);
                enemyHandle.SetThisPlayerMode(false);
                enemyHandle.SetThisTag("Enemy");
            }
        }
    }

    void Awake()
    {
        Init();
    }
}
