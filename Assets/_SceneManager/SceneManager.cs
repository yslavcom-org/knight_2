﻿using System;
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
    int countMissilesLaunched = 0;

    private Dictionary<int, ForceFieldDomeController> forceFieldDomeActiveDictionary;

    private UnityAction<object> listenerHomingMissileLaunched;
    private UnityAction<object> listenerHomingMissileDestroyed;
    #endregion

    #region Var delegates
    private event Action<Transform> OhHomingMissileTerminated = delegate { }; // position of the missile which was terminated
    #endregion

    #region Var player & enemy objects
    public GameObject tankPrefab;
    public GameObject destroyedTankPrefab__missile;
    public GameObject destroyedTankPrefab__gun;

    // player objects (main player & enemy objects)
    //Tank playerTank;
    //Tank []enemyTanks;
    int id__playerTank;
    private Dictionary<int, Tank> tankCollection;
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
    private StaminaBarController indicatorBarControllerPrefab; // prefab
    private StaminaBarController indicatorBarController; // use in this module
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

    void AddInventoryToPlayerObj(ref GameObject go)
    {
        //add inventor component
        go.AddComponent<GameInventory.Inventory>();
        //add inventory manager 
        go.AddComponent<InventoryItemsManager>();
    }

    void AddItemToPlayerObjInventory(GameInventory.Inventory playerInventory, string[] prefabStrings)
    {
        foreach (var str in prefabStrings)
        {
            if (null == str) continue;

            //read pick-up item prefab from Resource and add its contents to the player's inventory
            var pickUpItem = ReadPrefabAndCreateInstance.GetPickUpInstanceFromPrefab(str, false);
            if (null == pickUpItem)
            {
                PrintDebugLog.PrintDebug(string.Format("Scene manager, failed load pick up item {0}", str));
                continue;
            }
            else
            {
                playerInventory.AddItemToInventoryManually(pickUpItem); // add homing missiles by default
            }
        }
    }



    void InitTanks()
    {

        tankCollection = new Dictionary<int, Tank>();
        forceFieldDomeActiveDictionary = new Dictionary<int, ForceFieldDomeController>();

        //pick up items assigned by default
        string[] pickUpItemsPrefabArray = new string[2];
        pickUpItemsPrefabArray[0] = HardcodedValues.StrHomingMissile;
        pickUpItemsPrefabArray[1] = HardcodedValues.StrForcedFieldDome;

        Tank playerTank = new Tank();
        CreateTank(tankPrefab, ref playerTank, 0,
            new Vector3(0, 0, 0),
            "Player", "CoolTank",
            false/*true*/,
            pickUpItemsPrefabArray,
            true,
            true,
            false,
            trackPlayerTopCamera, vfxTopCameraHandle);
        id__playerTank = playerTank.tankHandle.GetId();

        playerTank.tankHandle.SetRadar(radar);
        radar?.SetPlayer(playerTank.tank);
        playerTank.tankHandle.makeRadarObject?.DeregisterFromRadarAsTarget();

        var camHandle = trackPlayerTopCamera.GetComponent<IndiePixel.Cameras.IP_TopDown_Camera>();
        camHandle.SetTarget(playerTank.tank.transform);



        //create array of enemy tanks
        const int enemyTanksCount = 6;
        enemyTankStartPosition = new Vector3[enemyTanksCount] {
                new Vector3(-10f, -2.45f, 12.54f),
                new Vector3(-25f, -2.45f, 12.54f),
                new Vector3(-40f, -2.45f, 12.54f),
                new Vector3(-55f, -2.45f, 12.54f),
                new Vector3(-60f, -2.45f, 12.54f),
                new Vector3(-75f, -2.45f, 12.54f)
            };

        Tank[] enemyTanks = new Tank[enemyTanksCount];

        for (int tank_idx = 0; tank_idx < enemyTanks.Length; ++tank_idx)
        {
            CreateTank(tankPrefab, ref enemyTanks[tank_idx], tank_idx,
                enemyTankStartPosition[tank_idx],
                "Enemy", "Enemy_",
                false,
                pickUpItemsPrefabArray,
                false,
                false,
                true,
                null, null);
        }
    }

    void CreateTank(GameObject tankPrefab, ref Tank refTank, int tank_idx,
        Vector3 startPosition,
        string tankTag, string tankName,
        bool attachMenuInventory,
        string[] pickUpItemsPrefabArray,
        bool setGunCamera/*true for tank ontrolled by human player*/,
        bool setThisPlayerMode/*true for tank ontrolled by human player*/,
        bool registerOnRadar,
        Camera trackPlayerTopCamera, IndiePixel.Cameras.IP_Minimap_Camera vfxTopCameraHandle)
    {
        refTank.tank = Instantiate(tankPrefab, startPosition, Quaternion.identity) as GameObject;

#if false
        //add inventor component
        AddInventoryToPlayerObj(ref refTank.tank);

        //add items to player's inventory on starts
        var tankInventory = refTank.tank.GetComponent<GameInventory.Inventory>();
        if (attachMenuInventory)
        {
            tankInventory.inventoryObj = inventory;
            tankInventory.slotHolder = slotHolder;
            tankInventory.inventoryEnabled = false;
        }
#endif
        refTank.tankHandle = refTank.tank.GetComponent<MyTankGame.TankControllerPlayer>();
        refTank.tankHandle.Init(
            trackPlayerTopCamera/* track camera for enemy vehicles*/,
            vfxTopCameraHandle/* homing missile track camera for enemy vehicles*/,
            startPosition);

#if true
        //add inventor component
        AddInventoryToPlayerObj(ref refTank.tank);

        //add items to player's inventory on starts
        var tankInventory = refTank.tank.GetComponent<GameInventory.Inventory>();
        if (attachMenuInventory)
        {
            tankInventory.inventoryObj = inventory;
            tankInventory.slotHolder = slotHolder;
            tankInventory.inventoryEnabled = false;
        }
#endif

        refTank.tankHandle.SetThisPlayerMode(setThisPlayerMode);
        refTank.tankHandle.SetGunCamera(setGunCamera);
        refTank.tankHandle.SetThisTag(tankTag);
        refTank.tankHandle.SetThisName(tankName + tank_idx);
        if (registerOnRadar)
        {
            refTank.tankHandle.makeRadarObject?.RegisterOnRadarAsTarget();
        }

        int id_tank = refTank.tankHandle.GetHashCode();
        refTank.tankHandle.SetId(id_tank); // set the unique object id
        tankInventory.SetId(id_tank); // set this id to the inventory which is linked to the menu inventory
        if (null != pickUpItemsPrefabArray)
        {
            AddItemToPlayerObjInventory(tankInventory, pickUpItemsPrefabArray);
        }

        InitDestroyedCopyOfTanks__Missile(ref refTank);
        InitDestroyedCopyOfTanks__Gun(ref refTank);

        //add to dictionatry 
        tankCollection.Add(refTank.tankHandle.GetId(), refTank);
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
        foreach (var button in buttons)
        {
            if (button.name == buttonCamerasName)
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
        if (explosionDispatcher)
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
    void OnEnable()
    {
        EventManager.StartListening(HardcodedValues.evntName__missileLaunched, listenerHomingMissileLaunched);
        EventManager.StartListening(HardcodedValues.evntName__missileDestroyed, listenerHomingMissileDestroyed);
        EventManager.StartListening(HardcodedValues.evntName__change_camera_mode, CameraModeChange);
    }

    void OnDisable()
    {
        EventManager.StopListening(HardcodedValues.evntName__missileLaunched, listenerHomingMissileLaunched);
        EventManager.StopListening(HardcodedValues.evntName__missileDestroyed, listenerHomingMissileDestroyed);
        EventManager.StopListening(HardcodedValues.evntName__change_camera_mode, CameraModeChange);
    }

    private void SetVfxCameraTrackMissile(Transform transform)
    {
        vfxTopCamera.gameObject.SetActive(true);
        vfxTopCameraHandle.SetTarget(transform);
    }

    void TargetActivateAntimissileProtection(Transform transform)
    {
        // Target will activate protection from the missile if there is any
        var obj = transform.gameObject;
        var forced_field = obj.GetComponent<ForceFieldDomeController>();
        if(forced_field != null)
        {
            if (forced_field.TryUse(transform, new Vector3(1, 1, 1)))
            {
                //add active forced field to the list
                var ids = transform.gameObject.GetComponentsInChildren<MyTankGame.IObjectId>();
                if (null != ids)
                {
                    int id = ids[0].GetId();
                    forceFieldDomeActiveDictionary.Add(id, forced_field);
                }
            }
        }
    }

    private void HomingMissilwWasLaunched(object arg)
    {
        countMissilesLaunched++;

        if (null == vfxTopCameraHandle) return;
        if (null == arg) return;

        Transform trnsfrm = (Transform)arg;
        SetVfxCameraTrackMissile(trnsfrm); // VFX camera will be heading to the target transform
        TargetActivateAntimissileProtection(trnsfrm); // Target will activate protection from the missile if there is any

    }

    private void HomingMissilwWasTerminated(object arg)
    {
        if(countMissilesLaunched > 0)
        {
            countMissilesLaunched--;
        }
        if(0 == countMissilesLaunched)
        {
            //deactivate active defence
            PrintDebugLog.PrintDebug("de-activate active protection");
        }

        OhHomingMissileTerminated((Transform)arg);
        StartCoroutine(DisableMissileTrackingCamera());
    }

    private IEnumerator DisableMissileTrackingCamera()
    {
        yield return new WaitForSeconds(1.0f);
        vfxTopCamera.gameObject.SetActive(false);
    }

#region objects affected by homing missile
    private void OhHomingMissileTerminated__tanks(Transform transform)
    {
        if (transform == null) return;

        Vector3 position = transform.position;

        Collider[] colliders = Physics.OverlapSphere(position, HardcodedValues.HomingMissileDamageRadius);
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
