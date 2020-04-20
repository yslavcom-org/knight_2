using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarControl : MonoBehaviour
{
    // Start is called before the first frame update

    public class RadarResource
    {
        public Radar radar;
        public RadarListOfObjects radarListOfObjects;
    }

    RadarResource radarResource = new RadarResource();

    void Start()
    {
        radarResource.radar = GetComponent<Radar>();
        radarResource.radarListOfObjects = GetComponent<RadarListOfObjects>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public RadarResource GetRadarResource()
    {
        return radarResource;
    }
}
