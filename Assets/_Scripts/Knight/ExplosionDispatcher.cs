using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExplosionDispatcher : MonoBehaviour
{
    public string event_name = "SphereBlowsUp";

    public Camera cam;
    public GameObject Sphere_blow_up;
    public const int sphere_blow_up__max = 5;
    private int sphere_blow_up__idx;
    private GameObject[] sphere_blow_up__array;


    private UnityAction<object> someListener;

    void Awake()
    {
        someListener = new UnityAction<object>(SphereBlowsUp);

        sphere_blow_up__array = new GameObject[sphere_blow_up__max];
    }

    private void Start()
    {
        for (int i = 0; i < sphere_blow_up__max; i++)
        {
            if (sphere_blow_up__array[i] == null)
            {
                sphere_blow_up__array[i] = Instantiate(Sphere_blow_up, Vector3.zero, Quaternion.identity) as GameObject;
            }
            sphere_blow_up__array[i].SetActive(false);
        }
        sphere_blow_up__idx = 0;
    }

    void OnEnable()
    {
        EventManager.StartListening(event_name, someListener);
    }

    void OnDisable()
    {
        EventManager.StopListening(event_name, someListener);
    }

    void SphereBlowsUp(object arg)
    {
        if(sphere_blow_up__idx >= sphere_blow_up__array.Length)
        {
            sphere_blow_up__idx = 0;
        }

        if(sphere_blow_up__array[sphere_blow_up__idx] == null)
        {
            Debug.Log("init SphereBlowsUp");
        }
        else
        {
            Vector3 position = (Vector3)arg;

            sphere_blow_up__array[sphere_blow_up__idx].transform.position = position;
            Track(sphere_blow_up__array[sphere_blow_up__idx]);
            if (sphere_blow_up__array[sphere_blow_up__idx])
            {
                var animated_texture_uv = sphere_blow_up__array[sphere_blow_up__idx].GetComponent<AnimatedTextureUV>();
                if (animated_texture_uv)
                {
                    animated_texture_uv.Init(false);
                }
            }
            sphere_blow_up__array[sphere_blow_up__idx].SetActive(true);

            sphere_blow_up__idx++;
        }
    }

    private void Track(GameObject gameObject)
    {
        var component = gameObject.GetComponent<AnimatedTextureUV>();
        if (component)
        {
            component.TrackCamera(cam);
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < sphere_blow_up__max; i++)
        {
            if (sphere_blow_up__array[i].activeSelf)
            {
                Track(sphere_blow_up__array[i]);
            }
        }
    }
}
