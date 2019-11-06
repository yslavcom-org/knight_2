using UnityEngine;

public class HardcodedValues : MonoBehaviour
{
    public int projectiles_count__max = 5;

    static public string StrPathToHomingMissilePrefab = "Assets/_HomingMissile/Prefab/HomingMissile.prefab";
    static public string StrWeaponManagerName = "WeaponManager";
    static public string StrHomingMissile = "HomingMissile";

    static private string pick_up_item_prefix = "PickUpItem";
    static public int HomingMissilePickUp__ItemId = Animator.StringToHash(pick_up_item_prefix + StrHomingMissile);
}
