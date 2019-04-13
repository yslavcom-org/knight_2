using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMouseLook : MonoBehaviour
{
    Vector2 mouseLook;
    Vector2 smoothV;
    public float sensitivity = 5f;
    public float smoothing = 2f;

    GameObject character;

    // Start is called before the first frame update
    void Start()
    {
        character = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        smoothV.x = Mathf.Lerp(smoothV.x, mouseDelta.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, mouseDelta.y, 1f / smoothing);
        mouseLook += smoothV;

#if true
        // both camera & body rotate x & y
        //clip angles
        mouseLook.y = Mathf.Clamp(mouseLook.y, -45f, 45f);
        mouseLook.x = Mathf.Clamp(mouseLook.x, -45f, 45f);

        var rotY = Quaternion.AngleAxis(-mouseLook.y, character.transform.right);
        var rotX = Quaternion.AngleAxis(mouseLook.x, character.transform.up);

        character.transform.localRotation = rotX * rotY;
#else
        // camera rotates up down, body only left right
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right); // rotate camera, up/down
        character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);  // rotate parent object left/right
#endif
    }
}
