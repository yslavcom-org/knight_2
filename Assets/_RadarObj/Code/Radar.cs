using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarObject
{
    public Image iconIdle { get; set; }
    public Image iconLocked { get; set; }
    public GameObject owner { get; set; }
    public int owner_id { get; set; }
}

public class LockedNearObj
{// position of the locked object wich is the closest
    [SerializeField]
    bool boLocked;
    public bool IsLocked { get { return boLocked; } }
    Transform m_Transform;
    public Transform tRansform { get { return m_Transform; } }

    public LockedNearObj()
    {
        Reset();
    }

    public void Reset()
    {
        boLocked = false;
        m_Transform = null;
    }

    public void SetLockedNearObj(Vector3 radarCentre, Transform newTransform)
    {
        if(!boLocked)
        {
            m_Transform = newTransform;
        }
        else
        {
            if (Vector3.Distance(radarCentre, newTransform.position) < Vector3.Distance(radarCentre, (m_Transform != null) ? m_Transform.position : new Vector3(0,0,0)))
            {
                m_Transform = newTransform;
            }
        }

        boLocked = true;
        
    }
}

public class Radar : MonoBehaviour
{
    private GameObject mainPlayer;
    readonly float mapScale = 0.5f;//2.0f;

    BlockRadarList blockRadarList;

    const int angleAccuracy = 10;


    private LockedNearObj lockedNearObj = new LockedNearObj();

    public void SetPlayer(GameObject player)
    {
        mainPlayer = player;
    }

    public ref LockedNearObj GetRefToLockedNearObj()
    {
        return ref lockedNearObj;
    }

    public bool CheckObjectIsInLineOfSight(ref RadarObject ro)
    {
        //now determine if there is a direct visibility between the object and the radar.
        //do NOT SHOW the object if there IS NOT direct visibility
        bool boOffsight = false;
        RaycastHit hit;
        bool boCheckLinecast = Physics.Linecast(mainPlayer.transform.position, ro.owner.transform.position, out hit);
        if (boCheckLinecast)
        {
            if (hit.collider)
            {
                if (null != blockRadarList)
                {
                    var list = blockRadarList.GetList();
                    foreach(var item in list)
                    {
                        if (hit.collider.tag == item)
                        {
                            boOffsight = true;
                            break;
                        }
                    }
                }
            }
        }

        return boOffsight 
            ? false : true;
    }

    public bool GetClosestLockedObject( out Transform targetTransforms)
    {
        targetTransforms = lockedNearObj.tRansform; 

        return lockedNearObj.IsLocked;
    }

    void Start()
    {
        lockedNearObj.Reset();
        blockRadarList = GetComponent<BlockRadarList>();
    }


}
