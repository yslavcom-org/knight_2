using UnityEditor;
using UnityEngine;

using Item = GameInventory.Item;

public class WeaponManager : MonoBehaviour
{
    

    private GameObject homingMissile;

    private void Awake()
    {
        this.tag = HardcodedValues.StrWeaponManagerName;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
