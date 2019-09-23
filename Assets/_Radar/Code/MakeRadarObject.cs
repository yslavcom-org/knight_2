using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeRadarObject : MonoBehaviour
{
    private RadarListOfObjects radar;

    // Start is called before the first frame update
    void Start()
    {
        radar = FindObjectOfType<RadarListOfObjects>();

        radar.RegisterRadarObject(this.gameObject);
    }

    void OnEnable()
    {
        radar.RegisterRadarObject(this.gameObject);
    }

    private void OnDestroy()
    {
        radar.RemoveRadarObject(this.gameObject);
    }

    private void OnDisable()
    {
        radar.RemoveRadarObject(this.gameObject);
    }
}
