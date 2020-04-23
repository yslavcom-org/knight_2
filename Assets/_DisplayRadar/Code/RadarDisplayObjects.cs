using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarDisplayObjects : MonoBehaviour
{
    [SerializeField]
    private GameObject mainPlayer;
    Radar playerRadar;

    RadarListOfObjects radarListOfObjects;
    private MyRadar.RadarSweep radarSweep;

    readonly float mapScale = 0.5f;//2.0f;
    const int angleAccuracy = 10;

    [SerializeField]
    private int listCount = 0;

    private LockedNearObj lockedNearObj;

    public void SetMainPlayer(GameObject go)
    {
        mainPlayer = go;
        playerRadar = mainPlayer.GetComponentInChildren<Radar>();
    }

    void DrawRadarDots()
    {
        InitRadarAssets();

        ref List<RadarObject> radObjects = ref radarListOfObjects.GetReferenceToListOfObjects();
        listCount = radObjects.Count;

        float sweepLineAngle = radarSweep.GetRotationAngle();
        lockedNearObj.Reset();

        if (null == mainPlayer) return;

        for (int i = 0; i < radObjects.Count; i++)
        {
            var ro = radObjects[i];

            bool boOffsight = !playerRadar.CheckObjectIsInLineOfSight( ro.owner.transform);

            if (boOffsight)
            {
                //do not show on radar
                ro.iconLocked.enabled = false;
                ro.iconIdle.enabled = false;
            }
            else
            {
                bool boLocked = playerRadar.RadarLockObjects((int)sweepLineAngle, this.transform, ro.owner.transform, out Vector3 icon_position);
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
                }
            }
        }
    }

    private void InitRadarAssets()
    {
        if ( null == radarListOfObjects
            || null == radarSweep)
        {
            radarListOfObjects = playerRadar.GetComponent<RadarListOfObjects>();
            lockedNearObj = playerRadar.GetComponent<Radar>().GetRefToLockedNearObj();

            radarSweep = playerRadar.GetComponentInChildren<MyRadar.RadarSweep>();
            DisplaySweepLine displaySweepLine = FindObjectOfType<DisplaySweepLine>();
            if (null != radarSweep
                && null != displaySweepLine)
            {
                displaySweepLine.SetRadarSweep(radarSweep);
            }
        }
    }

    private void Update()
    {
        DrawRadarDots();
    }

    bool boRadarMode_prev = false;

    public void SetEnabled(bool boRadarMode)
    {
        gameObject.SetActive(boRadarMode);
    }
}
