using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExplosionDispatcher : MonoBehaviour
{
    public string event_name = "SphereBlowsUp";

    public Camera cam;
    public GameObject BlowUpAnimation;
    public GameObject SphereBlowUp; // it's an optional object

    public const int array_size = 5;
    private int array__idx;

    private GameObject[] blow_up_anim__array;
    private GameObject[] sphere_blow_up__array;

    private UnityAction<object> someListener;

    void Awake()
    {
        someListener = new UnityAction<object>(SphereBlowsUp);

        blow_up_anim__array = new GameObject[array_size];
        if(null != SphereBlowUp)
        {
            sphere_blow_up__array = new GameObject[array_size];
        }
    }

    private void Start()
    {
        for (int i = 0; i < array_size; i++)
        {
            if (blow_up_anim__array[i] == null)
            {
                blow_up_anim__array[i] = Instantiate(BlowUpAnimation, Vector3.zero, Quaternion.identity) as GameObject;
            }
            blow_up_anim__array[i].SetActive(false);

            if(null != sphere_blow_up__array)
            {
                if (sphere_blow_up__array[i] == null)
                {
                    sphere_blow_up__array[i] = Instantiate(SphereBlowUp, Vector3.zero, Quaternion.identity) as GameObject;
                }
                sphere_blow_up__array[i].SetActive(false);
            }
        }
        array__idx = 0;
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
        if(array__idx >= blow_up_anim__array.Length)
        {
            array__idx = 0;
        }

        if(blow_up_anim__array[array__idx] == null)
        {
            //Debug.Log("init SphereBlowsUp");
        }
        else
        {
            Vector3 position = (Vector3)arg;
            SetExplosionPosition(blow_up_anim__array, sphere_blow_up__array, position);
            array__idx++;
        }
    }

    public float shift = 1.5f;
    private void SetExplosionPosition(GameObject[] anim_array, GameObject[] blow_up_array, Vector3 position)
    {
        bool blow_up_array_idx_initialized = (null != blow_up_array) && (null != blow_up_array[array__idx]);

        if (blow_up_array_idx_initialized)
        {
            blow_up_array[array__idx].transform.position = new Vector3(position.x, position.y-shift, position.z);
            blow_up_array[array__idx].SetActive(true);
        }

        anim_array[array__idx].transform.position = position;
        Track(anim_array[array__idx]);
        if (anim_array[array__idx])
        {
            anim_array[array__idx].SetActive(false);
            var animated_texture_uv = anim_array[array__idx].GetComponent<AnimatedTextureUV>();
            if (animated_texture_uv)
            {
                animated_texture_uv.Init(false, blow_up_array_idx_initialized ? blow_up_array[array__idx]  : null);
            }
            anim_array[array__idx].SetActive(true);
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

    //private void LateUpdate()
    private void FixedUpdate()
    {
        for (int i = 0; i < array_size; i++)
        {
            if (blow_up_anim__array[i])
            {
                if (blow_up_anim__array[i].activeSelf)
                {
                    Track(blow_up_anim__array[i]);
                }
            }
        }
    }
}
