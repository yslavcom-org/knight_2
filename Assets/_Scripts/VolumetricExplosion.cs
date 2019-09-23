using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumetricExplosion : MonoBehaviour
{
    public float loopduration = 0.6f;

    Renderer _renderer;
    public float _min_transform = 0.01f;
    public float _max_transform = 3.0f;
    public float _step_transform = 0.15f;
    float _cur_transform = 1f;
    public float _transparency_threshold = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _cur_transform = _min_transform;
        SetRadiusAndTransparency();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(_cur_transform >= _max_transform)
        {
            _cur_transform = _min_transform;
        }
        else
        {
            _cur_transform += _step_transform;
        }
        SetRadiusAndTransparency();


#if true
        float r = Mathf.Sin((Time.time / loopduration) * (2 * Mathf.PI)) * 0.5f + 0.25f;
        float g = Mathf.Sin((Time.time / loopduration + 0.33333333f) * 2 * Mathf.PI) * 0.5f + 0.25f;
        float b = Mathf.Sin((Time.time / loopduration + 0.66666667f) * 2 * Mathf.PI) * 0.5f + 0.25f;
        float correction = 1 / (r + g + b);
        r *= correction;
        g *= correction;
        b *= correction;
        _renderer.material.SetVector("_ChannelFactor", new Vector4(r, g, b, 0));
#endif
    }

    void SetRadiusAndTransparency()
    {
        

        GetComponent<Transform>().transform.localScale = new Vector3(_cur_transform, _cur_transform, _cur_transform);

        if (_cur_transform > _max_transform) _cur_transform = _max_transform;
        if (_cur_transform >= (_transparency_threshold * _max_transform))
        {
            float transparency_coef = 1f - ((_max_transform - _cur_transform) / _max_transform);


            _renderer.material.SetFloat("_Transparency", transparency_coef);
            //Debug.Log("_Transparency = "+transparency_coef);
        }
        else
        {
            _renderer.material.SetFloat("_Transparency", 0);
        }
    }
}
