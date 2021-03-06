﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(StaminaBarController))]
[RequireComponent(typeof(ActiveFieldDomeCollectionController))]
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
    ActiveFieldDomeCollectionController activeFieldDomeCollection;

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
    IndiePixel.Cameras.IP_TopDown_Camera trackPlayerTopCameraHandle;
    public Camera vfxTopCamera; // service camera for fun effects such as missile tracking
    private IndiePixel.Cameras.IP_Minimap_Camera vfxTopCameraHandle;
    public GameObject radarObject;
    #endregion

    #region manager to switch between game modes
    private MyTankGame.GameModeManager gameModeManager;
    #endregion

    #region  Canvas
    private Button buttonCameras;
    [SerializeField]
    private string buttonCamerasName = "ButtonCameras";
    public GameObject gunnerCamControls;

    [SerializeField]
    private StaminaBarController indicatorBarControllerPrefab; // prefab
    private StaminaBarController indicatorBarController; // use in this module
    [SerializeField]
    private StaminaBar healthBarPrefab;
    [SerializeField]
    private StaminaBar fuelBarPrefab;
    [SerializeField]
    private StaminaBar ammunitionBarPrefab;

    [SerializeField]
    private GameObject crossHair;

    [SerializeField]
    private ToroidNavigator toroidNavigator;
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

    RadarDisplayObjects GetGuiRadarHandleOnInit()
    {
        return radarObject.GetComponentInChildren<RadarDisplayObjects>();
    }

    // we need it for this class a a singleton
    public static SceneManager Instance { get; private set; }
    private void Init()
    {
        if (Instance == null)
        {
            Instance = this;

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

            //this will update the camera mode
            ButtonCamerasWasClicked();
            CameraModeChange(GameModeEnumerator.CameraMode.RadarView);
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

        //pick up items assigned by default
        string[] pickUpItemsPrefabArray = new string[5];
        pickUpItemsPrefabArray[0] = HardcodedValues.StrHomingMissile;
        pickUpItemsPrefabArray[1] = HardcodedValues.StrHomingMissile;
        pickUpItemsPrefabArray[2] = HardcodedValues.StrHomingMissile;
        pickUpItemsPrefabArray[3] = HardcodedValues.StrHomingMissile;
        pickUpItemsPrefabArray[4] = HardcodedValues.StrForcedFieldDome;

        Tank playerTank = new Tank();
        CreateTank(tankPrefab, ref playerTank, 0,
            new Vector3(21.97f, 1.0f, -10.0f),
            "Player", "CoolTank",
            false/*true*/,
            pickUpItemsPrefabArray,
            true,
            true,
            trackPlayerTopCamera, vfxTopCameraHandle);
        id__playerTank = playerTank.tankHandle.GetId();

        trackPlayerTopCameraHandle = trackPlayerTopCamera.GetComponent<IndiePixel.Cameras.IP_TopDown_Camera>();
        trackPlayerTopCameraHandle.SetTarget(playerTank.tank.transform);

        playerTank.tankHandle.SetCrosshair(crossHair);

        //create array of enemy tanks
        const int enemyTanksCount = 9;
        enemyTankStartPosition = new Vector3[enemyTanksCount] {
                // on the ground
                new Vector3(182.5f, 2.45f, 13.1f),
                new Vector3(-112.8f, 2.45f, 13.1f),
                new Vector3(-21f, 2.45f, 107.4f),
                new Vector3(46.2f, 2.45f, -10.4f),
                new Vector3(77.3f, 2.45f, 27.7f),
                new Vector3(181.1f, 2.45f, 19.7f),
                new Vector3(140.5f, 2.45f, 148.6f),
                new Vector3(-96.7f, 2.45f, 148.6f),

                //on top of a building
                new Vector3(64.3f, 6.43f, 20.3f),
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
                null, null);
        }

        //create lists for the tanks' radars

        //add enemy tanks to the radar of the main player tank
        RadarListOfObjects radarListOfObjects = playerTank.tank.GetComponentInChildren<RadarListOfObjects>();
        for (int tank_idx = 0; tank_idx < enemyTanks.Length; ++tank_idx)
        {
            radarListOfObjects.RegisterRadarObject(enemyTanks[tank_idx].tank, enemyTanks[tank_idx].tankHandle.GetId());
        }

        //connect playerTank to the radar in GUI
        var guiRadar = GetGuiRadarHandleOnInit();
        guiRadar.SetMainPlayer(playerTank.tank);

        //add main player tank to the radars of the enemy tanks
        for (int tank_idx = 0; tank_idx < enemyTanks.Length; ++tank_idx)
        {
            radarListOfObjects = enemyTanks[tank_idx].tank.GetComponentInChildren<RadarListOfObjects>();
            radarListOfObjects.RegisterRadarObject(playerTank.tank, playerTank.tankHandle.GetId());
        }
    }

    void CreateTank(GameObject tankPrefab, ref Tank refTank, int tank_idx,
        Vector3 startPosition,
        string tankTag, string tankName,
        bool attachMenuInventory,
        string[] pickUpItemsPrefabArray,
        bool setGunCamera/*true for tank ontrolled by human player*/,
        bool setHumanMode/*true for tank ontrolled by human player*/,
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

        var radarObj = ReadPrefabAndCreateInstance.GetInstanceFromPrefab(HardcodedValues.StrRadarObj, true);
        radarObj.transform.parent = refTank.tank.transform;
        Radar radar = radarObj.GetComponent<Radar>();
        radar.SetPlayer(refTank.tank);

        refTank.tankHandle.SetRadarHandle(radarObj);

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

        refTank.tankHandle.SetHumanMode(setHumanMode);
        if (false == setHumanMode)
        {
            refTank.tankHandle.DestroyAudioListener();
        }
        refTank.tankHandle.SetGunCamera(setGunCamera);
        refTank.tankHandle.SetThisTag(tankTag);
        refTank.tankHandle.SetThisName(tankName + tank_idx);

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

        gameModeManager.Init(radarObject, false, buttonCameras?.GetComponentInChildren<Text>(), gunnerCamControls);
    }


    void InitExplosionDispatcher()
    {
        explosionDispatcher = gameObject.AddComponent<ExplosionDispatcher>();
        if (explosionDispatcher)
        {
            explosionDispatcher.Init(HardcodedValues.evntName__missileBlowsUp, trackPlayerTopCamera, blowUpAnimationPrefab, sphereBlowUpPrefab, 1.5f);
        }
    }

    void DisableActiveForcedField(int id, ForceFieldDomeController forcedField)
    {
        if (null != forcedField)
        {
            forcedField.Disable();
        }
        activeFieldDomeCollection.Remove(id);
    }

    void SetTankDestroyed__Missile(int tankId, Tank tank)
    {
        var transform = tank.tank.transform;
        transform.Rotate(0, 90, 0);
        tank.destroyedTank__missile.transform.SetPositionAndRotation(tank.tank.transform.position, transform.rotation);
        tank.tank.SetActive(false);
        tank.destroyedTank__missile.SetActive(true);
        EventManager.TriggerEvent(HardcodedValues.evntName__objectDestroyed, tankId);
        tank.tankDestroyedHandle__missile.HomingMissileBlowUp();

        if (activeFieldDomeCollection.GetValue(tankId, out ForceFieldDomeController forcedField))
        {
            DisableActiveForcedField(tankId, forcedField);
        }
    }

    void SetTankDestroyed__Gun(int tankId, Tank tank)
    {
        var transform = tank.tank.transform;
        transform.Rotate(0, 90, 0);
        tank.destroyedTank__gun.transform.SetPositionAndRotation(tank.tank.transform.position, transform.rotation);
        tank.tank.SetActive(false);
        tank.destroyedTank__gun.SetActive(true);
        EventManager.TriggerEvent(HardcodedValues.evntName__objectDestroyed, tankId);
        tank.tankDestroyedHandle__gun.SetDestroyed();

        if (activeFieldDomeCollection.GetValue(tankId, out ForceFieldDomeController forcedField))
        {
            DisableActiveForcedField(tankId, forcedField);
        }
    }

    void Awake()
    {
        var obj = GameObject.Find(HardcodedValues.toroidalNavigationButton);
        if (obj)
        {
            toroidNavigator = obj.GetComponent<ToroidNavigator>();
        }
        activeFieldDomeCollection = new ActiveFieldDomeCollectionController();
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
        var targetIds = transform.gameObject.GetComponentsInChildren<MyTankGame.IObjectId>();
        if (forced_field != null
            && targetIds != null)
        {
            if (forced_field.TryUse(transform, new Vector3(2, 2, 2)))
            {
                //add active forced field to the list
                int targetId = targetIds[0].GetId();
                activeFieldDomeCollection.Add(targetId, forced_field);
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
        OhHomingMissileTerminated((Transform)arg);

        if (countMissilesLaunched > 0)
        {
            countMissilesLaunched--;

            if (0 == countMissilesLaunched)
            {
                //de-active all active forced fields
                while(activeFieldDomeCollection.GetFirstPair(out int id, out ForceFieldDomeController forcedField))
                {
                    DisableActiveForcedField(id, forcedField);
                }
            }
        }

        StartCoroutine(DisableMissileTrackingCamera());

    }

    private IEnumerator DisableMissileTrackingCamera()
    {
        yield return new WaitForSeconds(2.0f);
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
            var colliderIds = col.GetComponentsInChildren<MyTankGame.IObjectId>();
            if (null == colliderIds
                || colliderIds.Length == 0)
            {
                colliderIds = col.GetComponentsInParent<MyTankGame.IObjectId>();
            }
            if (null == colliderIds
                || colliderIds.Length == 0) continue;

            //get ID 
            int id = colliderIds[0].GetId();
            if (tankCollection.TryGetValue(id, out Tank tempTank))
            {//is there tank with such id?
                SetTankDestroyed__Missile(id, tempTank);
            }

            if(activeFieldDomeCollection.GetValue(id, out ForceFieldDomeController forcedField))
            {
                //is there forced field with such id 
                // in the list of colliders affected by the missile?
                DisableActiveForcedField(id, forcedField);
            }


            //is there forced field attached to the target transform?
            //This code may be relevant if missile was destroyed/terminated prior to hitting the forced field or the target
            var targetIds = transform.GetComponentsInChildren<MyTankGame.IObjectId>();
            if (null != targetIds
                && 0 < targetIds.Length)
            {
                int targetId = targetIds[0].GetId();
                if (activeFieldDomeCollection.GetValue(targetId, out ForceFieldDomeController targetForcedField))
                {
                    DisableActiveForcedField(targetId, targetForcedField);
                }
            }
        }
    }
#endregion

#region objects affected by tank gun
    private bool OnCheckValidGunTarget(string str)
    {
        return str == "Enemy"
            || str == "Player";
    }

    private void OnGunLockedTarget(bool is_locked)
    {
        if (null == gameModeManager) return;

        if (gameModeManager.GetIsTankGunLockTarget() != is_locked)
        {
            gameModeManager.SetIsTankGunLockTarget(is_locked);
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
                        int tankId = id.GetId();
                        if (tankCollection.TryGetValue(tankId, out Tank tempTank))
                        {
                            //Got the object.
                            //Check if this object has health packs in its own inventory and use one if there is any.
                            if (false == tempTank.tankHandle.CheckInventoryAndTryToUseHealthPacket())
                            {
                                //Otherwide set this object as destroyed
                                SetTankDestroyed__Gun(tankId, tempTank);
                            }
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
                    //trackPlayerTopCamera.gameObject.SetActive(false);
                    playerTank.tankHandle.SetGunCamera(true);
                    playerTank.tankHandle.IpTankController?.SetGameModeCameraMode(GameModeEnumerator.CameraMode.SniperView);
                    UpdateReferencesToCamera(playerTank.tankHandle.GetGunCamera());

                    if (null != toroidNavigator)
                    {
                        toroidNavigator.SetCamera(playerTank.tankHandle.GetGunCamera());
                    }

                    trackPlayerTopCamera.targetTexture = trackPlayerTopCameraHandle.GetTexRenderMode();
                    trackPlayerTopCamera.cullingMask &= ~(1 << LayerMask.NameToLayer(HardcodedValues.StrLayer__ParticleFire));
                    //trackPlayerTopCamera.cullingMask &= ~(1 << LayerMask.NameToLayer(HardcodedValues.StrLayer__RadarUI));
                    break;

                case GameModeEnumerator.CameraMode.RadarView:
                default:
                    playerTank.tankHandle.SetGunCamera(false);
                    trackPlayerTopCamera.gameObject.SetActive(true);
                    playerTank.tankHandle.IpTankController?.SetGameModeCameraMode(GameModeEnumerator.CameraMode.RadarView);
                    UpdateReferencesToCamera(trackPlayerTopCamera);

                    if (null != toroidNavigator)
                    {
                        toroidNavigator.SetCamera(trackPlayerTopCamera);
                    }

                    trackPlayerTopCamera.targetTexture = null;
                    trackPlayerTopCamera.cullingMask |= (1 << LayerMask.NameToLayer(HardcodedValues.StrLayer__ParticleFire));
                    //trackPlayerTopCamera.cullingMask |= (1 << LayerMask.NameToLayer(HardcodedValues.StrLayer__RadarUI));

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
