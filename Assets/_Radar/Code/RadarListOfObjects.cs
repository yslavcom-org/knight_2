using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarListOfObjects : MonoBehaviour
{
    public Image objIconOnRadarIdle;
    public Image objIconOnRadarLocked;

    private List<RadarObject> radObjects = new List<RadarObject>();

    public void RegisterRadarObject(GameObject o)
    {
        Image imIdle = Instantiate(objIconOnRadarIdle);
        Image imLocked = Instantiate(objIconOnRadarLocked);
        radObjects.Add(new RadarObject() { owner = o, iconIdle = imIdle, iconLocked = imLocked });
    }

    public void RemoveRadarObject(GameObject o)
    {
        List<RadarObject> newList = new List<RadarObject>();
        for (int i = 0; i < radObjects.Count; i++)
        {
            if (radObjects[i].owner == o)
            {
                Destroy(radObjects[i].iconIdle);
                Destroy(radObjects[i].iconLocked);
                continue;
            }
            else
            {
                newList.Add(radObjects[i]);
            }
        }

        radObjects.RemoveRange(0, radObjects.Count);
        radObjects.AddRange(newList);
    }

    public ref List<RadarObject> GetReferenceToListOfObjects()
    {
        return ref radObjects;
    }
}
