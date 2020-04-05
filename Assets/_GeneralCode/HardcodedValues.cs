using UnityEngine;

public class HardcodedValues : MonoBehaviour
{
    public int projectiles_count__max = 5;

    static readonly public bool boAndroidOrIphone = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer);

    //Scene components
    static readonly public string sniperCameraName = "CameraGunner";

    static readonly public string StrTag__ActiveDefence = "ActiveDefence";
    static readonly public string StrTag__Ground = "ground";
    static readonly public string StrTag__Building = "Building";
    static readonly public string StrTag__StreetPole = "StreetPole";
    static readonly public string StrTag__Tree = "Tree";
    static readonly public string StrTag__LevelBoundaryPlain = "RayCastBoundaryPlain";
    static readonly public float HomingMissileDamageRadius = 2f;

    //layers
    static readonly public string Layer__ActiveDefence = StrTag__ActiveDefence;

    //events
    static readonly public string evntName__tankShootEventString = "FreeSpaceKeyPressed";
    static readonly public string evntName__missileLaunched = "missileLaunch";
    static readonly public string evntName__missileDestroyed = "missileTerminate";
    static readonly public string evntName__change_camera_mode = "changeCameraMode";
    static readonly public string evntName__missileBlowsUp = "SphereBlowsUp";
    static readonly public string evntName__dismissTurret = "DissmissTurret";

    //GUI components
    static readonly public string toroidalNavigationButton = "ToroidalNavigation";


    //inventory
    static readonly public string StrInventoryItemsManagerName = "InventoryItemsManager";
    static readonly public string StrHomingMissile = "HomingMissile";
    static readonly public string StrForcedFieldDome = "ForcedFieldDome";
    static readonly public string StrPickUpObjectTag = "Items";
    static readonly private string pick_up_item_prefix = "PickUpItem";
    static readonly public int HomingMissilePickUp__ItemId = Animator.StringToHash(pick_up_item_prefix + StrHomingMissile);
    static readonly public int ForcedFieldDomePickUp__ItemId = Animator.StringToHash(pick_up_item_prefix + StrForcedFieldDome);

    static readonly public string StrResource_CrossHair = "CrossHair" + " / " + "CrossHair";

    //resources folder
    static readonly public string StrPickUpItem = "__PickUpItem";

    static readonly public string StrResource_HomingMissile = "HomingMissile" + "/" + StrHomingMissile;
    static readonly public string StrResource_HomingMissile_PickUpItem = "HomingMissile" + "/" + StrHomingMissile + StrPickUpItem;

    static readonly public string StrResource_ForcedFieldDome = "ForcedFieldDome" + " / " + StrForcedFieldDome;
    static readonly public string StrResource_ForcedFieldDome_PickUpItem = "ForcedFieldDome" + "/" + StrForcedFieldDome + StrPickUpItem;
}



