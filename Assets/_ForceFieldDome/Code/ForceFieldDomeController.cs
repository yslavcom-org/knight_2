using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldDomeController : MonoBehaviour
{

    #region Variables
    GameObject forcedFieldInstance;
    #endregion


    #region Built-in methods
    private void Awake()
    {
        forcedFieldInstance = ReadPrefabAndCreateInstance.GetInstanceFromPrefab(HardcodedValues.StrForcedFieldDome, false);
    }
    #endregion


    #region custom methods

    public void Activate(Transform refrerenceTransform)
    {
        if (null == forcedFieldInstance) return;

        forcedFieldInstance.transform.position = refrerenceTransform.position;
        forcedFieldInstance.transform.rotation = refrerenceTransform.rotation;
        forcedFieldInstance.SetActive(true);
    }

    void Disable()
    {
        forcedFieldInstance.SetActive(false);
    }
    #endregion
}
