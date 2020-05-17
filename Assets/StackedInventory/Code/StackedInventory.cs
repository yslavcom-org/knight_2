using System;
using System.Collections.Generic;
using UnityEngine;

public class StackedInventory : MonoBehaviour
{
    [SerializeField] List<StackedItem> items;
    [SerializeField] Transform itemsParent;
    [SerializeField] StackedItemSlots[] itemsSlots;

    public event Action<StackedItem> OnItemRightClickedEvent;

    private void Start()
    {
        for (int i = 0; i < itemsSlots.Length; i++)
        {
            itemsSlots[i].OnRightClickEvent += OnItemRightClickedEvent;
        }
    }


    void Init()
    {
        if (itemsParent != null)
        {
            itemsSlots = itemsParent.GetComponentsInChildren<StackedItemSlots>();
        }

        RefreshUI();
    }

    private void OnValidate()
    {
        Init();
    }

    private void Awake()
    {
        Init();
    }

    private void RefreshUI()
    {
        int i = 0;
        for (; i < items.Count && i < itemsSlots.Length; i++)
        {
            itemsSlots[i].Item = items[i];
        }

        for (; i < itemsSlots.Length; i++)
        {
            itemsSlots[i].Item = null;
        }
    }

    public bool AddItem(StackedItem item)
    {
        if (IsFull())
        {
            return false;
        }
        else {
            items.Add(item);
            RefreshUI();
            return true;
        }
    }

    public bool RemoveItem(StackedItem item)
    {
        if (items.Remove(item))
        {
            RefreshUI();
            return true;
        }
        return false;
    }

    public bool IsFull()
    {
        return (items.Count >= itemsSlots.Length);
    }
}
