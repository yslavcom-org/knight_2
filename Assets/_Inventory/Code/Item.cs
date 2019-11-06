using UnityEngine;

namespace GameInventory
{
    public class Item : MonoBehaviour
    {
        [HideInInspector]
        public int id;
        public string type;
        public string description;
        public Sprite icon;
        public int amount;//this is the quantity of this item

        public bool pickedUp;

        [HideInInspector]
        public bool equipped;

        [HideInInspector]
        public GameObject weapon;

        [HideInInspector]
        public GameObject weaponManager;

        public bool playersWeapon; //if true => in use by player

        private void Awake()
        {
            if(description == HardcodedValues.StrHomingMissile)
            {
                id = HardcodedValues.HomingMissilePickUp__ItemId;
            }
        }

        public void Start()
        {
            weaponManager = GetComponent<WeaponManager>();

            if (!playersWeapon)
            {
                int allWeapons = weaponManager.transform.childCount;
                for(int i=0;i<allWeapons;i++)
                {
                    if (weaponManager.transform.GetChild(i).gameObject.GetComponent<Item>().id == id)
                    {
                        weapon = weaponManager.transform.GetChild(i).gameObject;
                        break;
                    }
                }
            }
        }

        public void ItemUsage()
        {
            if (type == "weapon")
            {
                //weapon
                if (null != weapon)
                {
                    weapon.SetActive(true);
                }
                equipped = true;
            }
            else if (type == "health")
            {
                //health item
            }
            else if (type == "fuel")
            {
                //fuel
            }
        }

        public void Update()
        {
            if(equipped)
            {
                //peform weapon acts
            }
        }
    }
}
