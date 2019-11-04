using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWithMouse : MonoBehaviour
{
    public Camera _camera;
    public Shader _drawShader;

    private RenderTexture _splatMap;
    private Material _snowMaterial, _drawMaterial;
    private RaycastHit _hit;

    private const int tex_size_x = 1024;
    private const int tex_size_y = 1024;
    private const RenderTextureFormat _renderTextureFormat = RenderTextureFormat.ARGBFloat;


    // Start is called before the first frame update
    void Start()
    {
        _drawMaterial = new Material(_drawShader);
        _drawMaterial.SetVector("_Color", Color.red);

        _snowMaterial = GetComponent<MeshRenderer>().material;
        _splatMap = new RenderTexture(tex_size_x, tex_size_y, 0, _renderTextureFormat);
        _snowMaterial.SetTexture("_Splat", _splatMap);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.Mouse0))
        {
            if(Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out _hit))
            {
                _drawMaterial.SetVector("_Coordinate", new Vector4(_hit.textureCoord.x, _hit.textureCoord.y, 0, 0));
                RenderTexture temp = RenderTexture.GetTemporary(_splatMap.width, _splatMap.height, 0, _renderTextureFormat);
                Graphics.Blit(_splatMap, temp);
                Graphics.Blit(temp, _splatMap, _drawMaterial);
                RenderTexture.ReleaseTemporary(temp);
            }
        }
    }


    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, 256, 256), _splatMap, ScaleMode.ScaleToFit, false, 1f);
    }
}
