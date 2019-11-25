using UnityEngine;

public class HardcodedValues : MonoBehaviour
{
    //flag the platform
    static readonly public bool boAndroidOrIphone = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer);

    //cameras
    static readonly public string sniperCameraName = "CameraGunner";

    //actions/events
    static readonly public string tankShootEventString = "FreeSpaceKeyPressed";

    //gui controls
    static readonly public string toroidalNavigationButton = "ToroidalNavigation";

    //prefabs
    static readonly public string StrHomingMissile = "HomingMissile";
    static readonly public string StrForceFieldDome = "ForceFieldDome";

    static readonly public string PathTo__HomingMissilePRefab = StrHomingMissile + "/" + StrHomingMissile;
    static readonly public string PathTo__ForceFieldDomePRefab = StrForceFieldDome + "/" + StrForceFieldDome;

    //inventory
    static readonly public string StrInventoryItemsManagerName = "InventoryItemsManager";
    static readonly public string StrPickUpObjectTag = "Items";
    static readonly private string pick_up_item_prefix = "PickUpItem";
    static readonly public int HomingMissilePickUp__ItemId = Animator.StringToHash(pick_up_item_prefix + StrHomingMissile);
    static readonly public int ForceFieldDomePickUp__ItemId = Animator.StringToHash(pick_up_item_prefix + StrForceFieldDome);

    //misc
    public int projectiles_count__max = 5;

}
