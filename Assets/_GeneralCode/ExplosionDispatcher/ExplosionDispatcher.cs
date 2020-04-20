using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExplosionDispatcher : MonoBehaviour
{
    private int array__idx;
    public const int array_size = 5;
    private string m_event_name = HardcodedValues.evntName__missileBlowsUp; 
    private Camera m_trackCamera;
    private GameObject m_BlowUpAnimation;
    private GameObject m_SphereBlowUp;
    private float m_shift = 1.5f;

    private GameObject[] blow_up_anim__array;
    private GameObject[] sphere_blow_up__array;

    private UnityAction<object> someListener;

    void Awake()
    {
        someListener = new UnityAction<object>(SphereBlowsUp);

        InitExplosionArrays();
    }

    private void Start()
    {
        InitExplosionArrays();
    }

    private void InitExplosionArrays()
    {
        blow_up_anim__array = new GameObject[array_size];
        if (null != m_SphereBlowUp)
        {
            sphere_blow_up__array = new GameObject[array_size];
        }

        for (int i = 0; i < array_size; i++)
        {
            if (null != m_BlowUpAnimation)
            {
                if (blow_up_anim__array[i] == null)
                {
                    blow_up_anim__array[i] = Instantiate(m_BlowUpAnimation, Vector3.zero, Quaternion.identity) as GameObject;
                }
                blow_up_anim__array[i].SetActive(false);
            }

            if (null != m_SphereBlowUp
                && null != sphere_blow_up__array)
            {
                if (sphere_blow_up__array[i] == null)
                {
                    sphere_blow_up__array[i] = Instantiate(m_SphereBlowUp, Vector3.zero, Quaternion.identity) as GameObject;
                }
                sphere_blow_up__array[i].SetActive(false);
            }
        }
        array__idx = 0;
    }

    void OnEnable()
    {
        EventManager.StartListening(m_event_name, someListener);
    }

    void OnDisable()
    {
        EventManager.StopListening(m_event_name, someListener);
    }


    public void Init(string event_name, Camera trackCamera, GameObject BlowUpAnimation, GameObject SphereBlowUp, float shift)
    {
        m_event_name = event_name;
        m_trackCamera = trackCamera;
        m_BlowUpAnimation = BlowUpAnimation;
        m_SphereBlowUp = SphereBlowUp;
        m_shift = shift;

        InitExplosionArrays();
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
            Transform trnsfrm = (Transform)arg;
            SetExplosionPosition(blow_up_anim__array, sphere_blow_up__array, trnsfrm.position);
            array__idx++;
        }
    }

    private void SetExplosionPosition(GameObject[] anim_array, GameObject[] blow_up_array, Vector3 position)
    {
        bool blow_up_array_idx_initialized = (null != blow_up_array) && (null != blow_up_array[array__idx]);

        if (blow_up_array_idx_initialized)
        {
            blow_up_array[array__idx].transform.position = new Vector3(position.x, position.y-m_shift, position.z);
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
            component.TrackCamera(m_trackCamera);
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
