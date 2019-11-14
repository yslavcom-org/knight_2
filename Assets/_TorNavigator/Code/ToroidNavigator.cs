using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToroidNavigator : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

    [SerializeField]
    private Image image;

    [SerializeField]
    private Sprite touchPlaceholder;


    Vector2 sizeDelta;
    Vector2 canvasScale;


    Vector3 centre;
    float realWidth, realHeight;
    float canvasWidth, canvasHeight; // they both must be equal , i.e. we deal with circle

    float radius;

    GameObject overlayObject;
    bool is_overlay_active = false;

    void Start()
    {
        centre = this.transform.position;

        realWidth = image.sprite.rect.width;
        realHeight = image.sprite.rect.height;

        var canvas = GetComponentInParent<Canvas>();
        /*
         * https://forum.unity.com/threads/get-the-true-size-in-pixels-of-image-that-uses-canvas-scaler.487930/
        Vector2 sizeDelta = GetComponent<RectTransform>().sizeDelta;
        Vector2 canvasScale = new Vector2(canvas.transform.localScale.x, canvas.transform.localScale.y);
        Vector2 finalScale = new Vector2(sizeDelta.x * canvasScale.x, sizeDelta.y * canvasScale.y);
        */

        canvasWidth = canvas.scaleFactor * realWidth;
        canvasHeight = canvas.scaleFactor * realHeight;

        radius = canvasWidth / 2;

        overlayObject = new GameObject("TouchOverlay");
        var sprite_rend = overlayObject.AddComponent<SpriteRenderer>();
        sprite_rend.sprite = touchPlaceholder;
        overlayObject.SetActive(is_overlay_active);

        //color cannot be changed directly
        var color = overlayObject.GetComponent<SpriteRenderer>().color;
        color.a = 0.333f;
        overlayObject.GetComponent<SpriteRenderer>().color = color;
    }

    /*
     * Use the center of the circle as the origin. 

dir = org - pos;
Then you would do something like arctan2(dir.y,dir.x) * RAD_2PI; 
     */

    public void SetCamera(Camera camera)
    {
        this.camera = camera;
    }

    
    private void Update()
    {
        bool bo_active = false;

        bool clicked = TouchOrMouseClick.TrackMouseOrTouchCoordGUI(camera, out Vector3 position);
        if(clicked)
        {
            var screen_position = camera.WorldToScreenPoint(position);

            Vector3 diff = screen_position - centre;
            var distance = diff.magnitude;

            //Debug.Log(string.Format("centre = {0}, canvasWidth = {1}, screen_position = {2}, distance = {3}", centre, canvasWidth, screen_position, distance));
            if (distance <= radius)
            {
                Debug.Log("inside circle");

                bo_active = true;

            }
        }

        switch(is_overlay_active)
        {
            case true:
                if(!bo_active)
                {
                    overlayObject.SetActive(false);
                }
                break;

            case false:
                if (bo_active)
                {
                    overlayObject.SetActive(true);
                }
                break;
        }

        if(bo_active)
        {
            overlayObject.transform.position = position;
            overlayObject.transform.LookAt(camera.transform);
        }

        
    }
}
