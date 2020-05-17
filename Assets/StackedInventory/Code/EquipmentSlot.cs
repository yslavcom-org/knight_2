using UnityEngine;

public class EquipmentSlot : StackedItemSlots
{
    public EquipmentType EquipmentType;

    private void Init()
    {
        gameObject.name = EquipmentType.ToString() + " Slot";
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        Init();
    }

    protected override void Awake()
    {
        base.Awake();
        Init();
    }
}
