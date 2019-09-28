using UnityEngine;

public class MakeRadarObject : MonoBehaviour
{
    private RadarListOfObjects radar;

    // Start is called before the first frame update
    void Start()
    {
        radar = FindObjectOfType<RadarListOfObjects>();

        radar.RegisterRadarObject(gameObject);
    }

    void OnEnable()
    {
        radar.RegisterRadarObject(gameObject);
    }

    private void OnDestroy()
    {
        radar.RemoveRadarObject(gameObject);
    }

    private void OnDisable()
    {
        radar.RemoveRadarObject(gameObject);
    }
}
