using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackedInventoryManager : MonoBehaviour
{
    [SerializeField] StackedInventory inventory;
    [SerializeField] EquipmentPanel equipmentPanel;

    private void Awake()
    {
        inventory.OnItemRightClickedEvent += EquipFromInventory;
        equipmentPanel.OnItemRightClickedEvent += UnequipFromEquipPanel;
    }

    private void EquipFromInventory(StackedItem item)
    {
        if (item is EquipableStackedItem)
        {
            Equip((EquipableStackedItem)item);
        }
    }

    private void UnequipFromEquipPanel(StackedItem item)
    {
        if (item is EquipableStackedItem)
        {
            Unequip((EquipableStackedItem)item);
        }
    }

    public void Equip(EquipableStackedItem item)
    {
        if (inventory.RemoveItem(item))
        {
            EquipableStackedItem previousItem;
            if (equipmentPanel.AddItem(item, out previousItem))
            {
                if (previousItem != null)
                {
                    inventory.AddItem(previousItem);
                }
            }
            else
            {
                inventory.AddItem(item);
            }
        }
    }

    public void Unequip(EquipableStackedItem item)
    {
        if (!inventory.IsFull() 
            && equipmentPanel.RemoveItem(item))
        {
            inventory.AddItem(item);
        }
    }
}
