using UnityEngine;
using Kryz.CharacterStats;


public class StackedCharacter : MonoBehaviour
{
    public CharacterStat Strength;
    public CharacterStat Agility;
    public CharacterStat Intelligence;
    public CharacterStat Vitality;

    [SerializeField] StackedInventory inventory;
    [SerializeField] EquipmentPanel equipmentPanel;
    [SerializeField] StatPanel statPanel;

    private void Awake()
    {
        statPanel.SetStats(Strength, Agility, Intelligence, Vitality);
        statPanel.UpdateStatValues();

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
                    previousItem.Unequip(this);
                    statPanel.UpdateStatValues();
                }
                item.Equip(this);
                statPanel.UpdateStatValues();
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
            item.Unequip(this);
            statPanel.UpdateStatValues();

            inventory.AddItem(item);
        }
    }
}
