using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MissileHitTerrain : MonoBehaviour
{
    public string event_name = HardcodedValues.evntName__missileBlowsUp; 
    public Shader _drawShader;
    //public Camera _camera;

    public string _mapName = "_BaseMap";

    public Color _hitColor;

    private RenderTexture _splatMap;
    private Material _surfaceMaterial, _drawMaterial;

    private Texture _baseMapTex;

    //private const int tex_size_x = 1024;
    //private const int tex_size_y = 1024;
    private const RenderTextureFormat _renderTextureFormat = RenderTextureFormat.ARGBFloat;

    private UnityAction<object> someListener;

    void Awake()
    {
        someListener = new UnityAction<object>(SphereBlowsUp);
    }

    // Start is called before the first frame update
    void Start()
    {
        _surfaceMaterial = GetComponent<MeshRenderer>().material;
        _baseMapTex = _surfaceMaterial.GetTexture(_mapName);

        //_splatMap = new RenderTexturetex_size_x, tex_size_y, 0, _renderTextureFormat);
        _splatMap = new RenderTexture(_baseMapTex.width, _baseMapTex.height, 0, _renderTextureFormat);
        Graphics.Blit(_baseMapTex, _splatMap);
        _surfaceMaterial.SetTexture("_Splat", _splatMap);

        _drawMaterial = new Material(_drawShader);

        if (null == _hitColor)
        {
            _hitColor = Color.red;
        }
        _drawMaterial.SetVector("_Color", _hitColor);
    }

    void SphereBlowsUp(object arg)
    {
        if (null == arg) return;

        Vector3 worldCollidePoint = ((Transform)arg).position;

        RaycastHit hit;
        //if (Physics.Raycast(_camera.ScreenPointToRay(worldCollidePoint), out hit))
        if (Physics.Raycast(worldCollidePoint, -Vector3.up, out hit))
        {
            Vector4 collide_coord = new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0);
            _drawMaterial.SetVector("_Coordinate", collide_coord);
            RenderTexture tempTexture = RenderTexture.GetTemporary(_splatMap.width, _splatMap.height, 0, _renderTextureFormat);
            Graphics.Blit(_splatMap, tempTexture);
            Graphics.Blit(tempTexture, _splatMap, _drawMaterial);
            RenderTexture.ReleaseTemporary(tempTexture);

            //assign the modified tecture to the surface material's texture
            _surfaceMaterial.SetTexture(_mapName, _splatMap);
        }
    }


    void OnEnable()
    {
        EventManager.StartListening(event_name, someListener);
    }

    void OnDisable()
    {
        EventManager.StopListening(event_name, someListener);
    }

#if false
    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, 256, 256), _splatMap, ScaleMode.ScaleToFit, false, 1f);
    }
#endif
}
 