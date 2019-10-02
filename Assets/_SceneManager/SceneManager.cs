using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private MyTankGame.TankControllerPlayer player;
    public Camera trackTopCamera;

    public static SceneManager Instance { get; private set; }
    private void Init()
    {
        if (Instance == null)
        {
            Instance = this;
            player = FindObjectOfType<MyTankGame.TankControllerPlayer>();
            player.Init(trackTopCamera);
        }
    }

    void Awake()
    {
        Init();
    }
}
