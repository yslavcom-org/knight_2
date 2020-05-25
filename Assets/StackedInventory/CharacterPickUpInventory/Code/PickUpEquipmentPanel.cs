using UnityEngine;

namespace Iar.StackedInventory
{
    public class PickUpEquipmentPanel : EquipmentPanel
    {
        public bool AddItem(EquipableStackedItem item)
        {
            for (int i = 0; i < equipmentSlots.Length; i++)
            {
                if (equipmentSlots[i].EquipmentType == item.EquipmentType)
                {
                    equipmentSlots[i].Item = item;
                    return true;
                }
            }

            return false;
        }
    }
}
