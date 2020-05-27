using UnityEngine;

namespace GameInventory
{
    public class Item : MonoBehaviour
    {
        [HideInInspector]
        public Iar.StackedInventory.EquipmentType EquipmentType;
        public int amount;//this is the quantity of this item
    }
}
