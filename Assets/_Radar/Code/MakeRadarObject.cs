using UnityEngine;

public class MakeRadarObject : MonoBehaviour
{
    private RadarListOfObjects radar;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Awake()
    {
        Init();
    }

    void OnEnable()
    {
        RegisterOnRadarAsTarget();
    }

    private void OnDestroy()
    {
        DeregisterFromRadarAsTarget();
    }

    private void OnDisable()
    {
        DeregisterFromRadarAsTarget();
    }


    private void Init()
    {
        radar = FindObjectOfType<RadarListOfObjects>();
        RegisterOnRadarAsTarget();
    }

    public void RegisterOnRadarAsTarget()
    {
        radar.RegisterRadarObject(gameObject);
    }

    public void DeregisterFromRadarAsTarget()
    {
        radar.RemoveRadarObject(gameObject);
    }
}
