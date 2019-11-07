using UnityEditor;
using UnityEngine;

using Item = GameInventory.Item;

public class InventoryItemsManager : MonoBehaviour
{
    private GameObject homingMissile;

    private void Awake()
    {
        GameInventory.Slot.OnPickedItemId += OnPickedItemId;
        GameInventory.Slot.OnEmptiedItemId += OnEmptiedItemId;

        this.name = HardcodedValues.StrInventoryItemsManagerName;
        var prefab = AssetDatabase.LoadAssetAtPath(HardcodedValues.StrPathToHomingMissilePrefab, typeof(GameObject));
        if(prefab != null)
        {
            homingMissile = (GameObject)Instantiate(prefab, new Vector3(0,0,0), Quaternion.identity);
            if(homingMissile != null)
            {
                homingMissile.gameObject.AddComponent<Item>();
                var item = homingMissile.GetComponent<Item>();
                item.id = HardcodedValues.HomingMissilePickUp__ItemId;
            }
        }
    }

    private void OnDisable()
    {
        GameInventory.Slot.OnPickedItemId -= OnPickedItemId;
        GameInventory.Slot.OnEmptiedItemId -= OnEmptiedItemId;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    #region events
    private void OnPickedItemId(int id)
    {
        if(HardcodedValues.HomingMissilePickUp__ItemId == id)
        {
            //activate homing misile system
        }
    }

    private void OnEmptiedItemId(int id)
    {
        if (HardcodedValues.HomingMissilePickUp__ItemId == id)
        {
            //disable homing misile system
        }
    }
    #endregion
}
