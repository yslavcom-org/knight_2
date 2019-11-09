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

        public bool playersWeapon; //if true => in use by player

        private void Awake()
        {
            if(description == HardcodedValues.StrHomingMissile)
            {
                id = HardcodedValues.HomingMissilePickUp__ItemId;
            }
        }
    }
}
