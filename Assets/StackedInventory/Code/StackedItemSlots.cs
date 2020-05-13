using UnityEngine;
using UnityEngine.UI;

public class StackedItemSlots : MonoBehaviour
{
    [SerializeField]
    private Image Image;

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

    private void OnValidate()
    {
        if (Image == null)
        {
            Image = GetComponent<Image>();
        }
    }
}
