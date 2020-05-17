using System;
using UnityEngine;

public class EquipmentPanel : MonoBehaviour
{
    [SerializeField] Transform equipmentSlotsParent;
    [SerializeField] EquipmentSlot[] equipmentSlots;

    public event Action<StackedItem> OnItemRightClickedEvent;

    private void Start()
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i].OnRightClickEvent += OnItemRightClickedEvent;
        }
    }

    private void Init()
    {
        equipmentSlots = equipmentSlotsParent.GetComponentsInChildren<EquipmentSlot>();
    }

    private void Awake()
    {
        Init();
    }

    private void OnValidate()
    {
        Init();
    }

    public bool AddItem(EquipableStackedItem item, out EquipableStackedItem previousItem)
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].EquipmentType == item.EquipmentType)
            {
                previousItem = (EquipableStackedItem)equipmentSlots[i].Item;
                equipmentSlots[i].Item = item;
                return true;
            }
        }

        previousItem = null;
        return false;
    }


    public bool RemoveItem(EquipableStackedItem item)
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].EquipmentType == item.EquipmentType)
            {
                equipmentSlots[i].Item = null;
                return true;
            }
        }

        return false;
    }
}
