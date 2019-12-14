using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenuId : MonoBehaviour, MyTankGame.IObjectId
{
    #region IObjectId implementation
    int ID;
    public void SetId(int id)
    {
        ID = id;
    }
    public int GetId()
    {
        return ID;
    }
    #endregion
}
