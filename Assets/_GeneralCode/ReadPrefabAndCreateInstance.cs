using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadPrefabAndCreateInstance : MonoBehaviour
{

    static Object GetPrefab(string str)
    {
        string path = string.Format("{0}/{1}{2}", str, str, HardcodedValues.StrPickUpItem);

        //Debug.Log(string.Format("GetInstanceFromPrefab::path = {0}", path));
        var prefab = Instantiate(Resources.Load(path)) ;
        
        return prefab;
    }

    static public GameObject GetInstanceFromPrefab(string str, bool setActive)
    {
        //GameObject instance = null;
        GameObject instance/*prefab*/ = (GameObject)GetPrefab(str);
        if (null != instance)
        {
            //instance = Instantiate(prefab) as GameObject;
            instance.SetActive(setActive);
        }

        return instance;
    }

}
