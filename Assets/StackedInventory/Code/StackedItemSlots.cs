using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StackedItemSlots : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Image Image;

    public event Action<StackedItem> OnRightClickEvent;

    private StackedItem _item;
    public StackedItem Item
    {
        get { return _item; }
        set {
            _item = value;
            if(_item == null)
            {
                Image.enabled = false;
            }
            else
            {
                Image.sprite = _item.Icon;
                Image.enabled = true;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData != null
            && eventData.button == PointerEventData.InputButton.Right)
        {
            if (Item != null
                && OnRightClickEvent != null)
            {
                OnRightClickEvent(Item);
            }
        }
    }

    void Init()
    {
        if (Image == null)
        {
            Image = GetComponent<Image>();
        }
    }

    protected virtual void OnValidate()
    {
        Init();
    }

    protected virtual void Awake()
    {
        Init();
    }
}
