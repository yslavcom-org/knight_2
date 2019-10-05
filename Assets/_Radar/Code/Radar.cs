using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarObject
{
    public Image iconIdle { get; set; }
    public Image iconLocked { get; set; }
    public GameObject owner { get; set; }
}

class LockedNearObj
{// position of the locked object wich is the closest
    bool boLocked;
    public bool IsLocked { get { return boLocked; } }
    Vector3 position;
    public Vector3 Position{ get { return position; } }

    public LockedNearObj()
    {
        Reset();
    }

    public void Reset()
    {
        boLocked = false;
        position = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    }

    public void SetLockedNearObj(Vector3 radarCentre, Vector3 new_position)
    {
        if(!boLocked)
        {
            position = new_position;
        }
        else
        {
            if (Vector3.Distance(radarCentre, new_position) < Vector3.Distance(radarCentre, position))
            {
                position = new_position;
            }
        }

        boLocked = true;
        
    }
}

public class Radar : MonoBehaviour
{
    private GameObject mainPlayer;
    readonly float mapScale = 0.5f;//2.0f;
    private MyRadar.RadarSweep radarSweep;

    RadarListOfObjects radarListOfObjects;

    const int angleAccuracy = 5;


    private LockedNearObj lockedNearObj = new LockedNearObj();

    public void SetPlayer(GameObject player)
    {
        mainPlayer = player;
    }

    public void SetActive(bool boRadarMode)
    {
        gameObject.SetActive(boRadarMode);
    }

    void DrawRadarDots()
    {
        InitRadarAssets();

        ref List<RadarObject> radObjects = ref radarListOfObjects.GetReferenceToListOfObjects();

        float sweepLineAngle = radarSweep.GetRotationAngle();
        lockedNearObj.Reset();

        if (null == mainPlayer) return;

        for (int i = 0; i < radObjects.Count; i++)
        {
            var ro = radObjects[i];

            //now determine if there is a direct visibility between the object and the radar.
            //do NOT SHOW the object if there IS NOT direct visibility
            bool boOffsight = false;
            RaycastHit hit;
            bool boCheckLinecast = Physics.Linecast(mainPlayer.transform.position, ro.owner.transform.position, out hit);
#if false
            if (i == 0)
            {
                Debug.DrawLine(playerPos.position, ro.owner.transform.position,Color.red);
            }
            else if (i == 1)
            {
                Debug.DrawLine(playerPos.position, ro.owner.transform.position, Color.yellow);
            }
            else if (i == 2)
            {
                Debug.DrawLine(playerPos.position, ro.owner.transform.position, Color.blue);
            }
#endif
            if (boCheckLinecast)
            {
                if (hit.collider)
                {
                    if (hit.collider.tag == "ground")
                    {
                        boOffsight = true;
                    }
                }
            }

            if (boOffsight)
            {
                //do not show on radar
                ro.iconLocked.enabled = false;
                ro.iconIdle.enabled = false;
            }
            else
            {
                //show on radar
                Vector3 radarPos = (ro.owner.transform.position - mainPlayer.transform.position);
                float distToObject = Vector3.Distance(mainPlayer.transform.position, ro.owner.transform.position) * mapScale;
                float deltay = Mathf.Atan2(radarPos.x, radarPos.z) * Mathf.Rad2Deg - 270 - mainPlayer.transform.eulerAngles.y;
                radarPos.x = distToObject * Mathf.Cos(deltay * Mathf.Deg2Rad) * -1;
                radarPos.z = distToObject * Mathf.Sin(deltay * Mathf.Deg2Rad);

                Vector3 icon_position = new Vector3(radarPos.x, radarPos.z, 0) + this.transform.position;  // the position of the object onb the radar

                int objAngle = (int)(360 - AngleGetters.CalculateAngle(this.transform.position, icon_position));

                bool boLocked = false;

                int objectAngleLow = objAngle - angleAccuracy;
                if (objectAngleLow < 0)
                {
                    objectAngleLow = 360 - (0 - objectAngleLow);
                }
                int objectAngleHigh = objAngle + angleAccuracy;
                if (objectAngleHigh > 360)
                {
                    objectAngleHigh = objectAngleHigh - 360;
                }

                if (objectAngleLow <= (int)sweepLineAngle
                    && objectAngleHigh >= (int)sweepLineAngle)
                {
                    boLocked = true;
                }
                if (!boLocked)
                {//object not locked
                    ro.iconLocked.enabled = false;
                    ro.iconIdle.enabled = true;
                    ro.iconIdle.transform.SetParent(this.transform);
                    ro.iconIdle.transform.position = icon_position;
                }
                else
                {//object is locked
                    ro.iconIdle.enabled = false;
                    ro.iconLocked.enabled = true;
                    ro.iconLocked.transform.SetParent(this.transform);
                    ro.iconLocked.transform.position = icon_position;

                    lockedNearObj.SetLockedNearObj(mainPlayer.transform.position, ro.owner.transform.position);
                }
            }
        }
    }

    public bool GetClosestLockedObject( out Vector3 targetPosition)
    {
        targetPosition = lockedNearObj.Position; 

        return lockedNearObj.IsLocked;
    }

    private void InitRadarAssets()
    {
        if (null == radarSweep
            || null == radarListOfObjects)
        {
            lockedNearObj.Reset();

            radarSweep = FindObjectOfType<MyRadar.RadarSweep>();
            radarListOfObjects = FindObjectOfType<RadarListOfObjects>();
        }
    }

    void Start()
    {
        InitRadarAssets();
    }

    private void Update()
    {
        DrawRadarDots();
    }
}
