using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarListOfObjects : MonoBehaviour
{
    public Image objIconOnRadarIdle;
    public Image objIconOnRadarLocked;

    [SerializeField]
    private int listCount = 0;
    private List<RadarObject> radObjects = new List<RadarObject>();

    public void RegisterRadarObject(GameObject gameObject, int gameObjectId)
    {
        Image imIdle = Instantiate(objIconOnRadarIdle);
        Image imLocked = Instantiate(objIconOnRadarLocked);
        radObjects.Add(new RadarObject() { owner = gameObject, owner_id = gameObjectId, iconIdle = imIdle, iconLocked = imLocked });

        listCount = radObjects.Count;
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

        listCount = radObjects.Count;
    }

    public ref List<RadarObject> GetReferenceToListOfObjects()
    {
        return ref radObjects;
    }
}
