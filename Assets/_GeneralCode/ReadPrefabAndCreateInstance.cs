using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadPrefabAndCreateInstance : MonoBehaviour
{

    static public GameObject GetPrefab(string str)
    {
        string path = string.Format("{0}/{1}{2}", str, str, HardcodedValues.StrPickUpItem);

        //Debug.Log(string.Format("GetInstanceFromPrefab::path = {0}", path));
        var prefab = Instantiate(Resources.Load(path)) as GameObject;
        
        return prefab;
    }

    static public GameObject GetInstanceFromPrefab(string str, bool setActive)
    {
        GameObject instance = null;

        var prefab = GetPrefab(str);
        if (null != prefab)
        {
            instance = Instantiate(prefab) as GameObject;
            if (null != instance)
            {
                instance.SetActive(setActive);
            }
        }

        return instance;
    }

}
