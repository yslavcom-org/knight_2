﻿using UnityEngine;

public class HardcodedValues : MonoBehaviour
{
    public int projectiles_count__max = 5;

    static readonly public string StrPathToHomingMissilePrefab = "Assets/_HomingMissile/Prefab/HomingMissile.prefab";
    static readonly public string StrInventoryItemsManagerName = "InventoryItemsManager";
    static readonly public string StrHomingMissile = "HomingMissile";
    static readonly public string StrPickUpObjectTag = "Items";

    static readonly private string pick_up_item_prefix = "PickUpItem";
    static readonly public int HomingMissilePickUp__ItemId = Animator.StringToHash(pick_up_item_prefix + StrHomingMissile);
    static readonly public string sniperCameraName = "CameraGunner";
    static readonly public string tankShootEventString = "FreeSpaceKeyPressed";
}
