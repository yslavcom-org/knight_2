﻿using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;


#if true

public class ToroidNavigator : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

    [SerializeField]
    private Image image;

    [SerializeField]
    private Sprite touchPlaceholder;
    Texture2D touchPlaceholderTex;


    Vector2 sizeDelta;
    Vector2 canvasScale;


    Vector3 centre;
    float realWidth, realHeight;
    float canvasWidth, canvasHeight; // they both must be equal , i.e. we deal with circle
    float x_high, x_low;
    float y_high, y_low;

    float canvasPlaceHolderWidth, canvasPlaceHolderHeight;

    float radius;
    Canvas canvas;

    //int dir_x, dir_y;

    void Start()
    {
        centre = this.transform.position;

        realWidth = image.sprite.rect.width;
        realHeight = image.sprite.rect.height;

        canvas = GetComponentInParent<Canvas>();
        /*
         * https://forum.unity.com/threads/get-the-true-size-in-pixels-of-image-that-uses-canvas-scaler.487930/
        Vector2 sizeDelta = GetComponent<RectTransform>().sizeDelta;
        Vector2 canvasScale = new Vector2(canvas.transform.localScale.x, canvas.transform.localScale.y);
        Vector2 finalScale = new Vector2(sizeDelta.x * canvasScale.x, sizeDelta.y * canvasScale.y);
        */

        canvasWidth = canvas.scaleFactor * realWidth;
        canvasHeight = canvas.scaleFactor * realHeight;
        radius = canvasWidth / 2;
        x_high = centre.x + radius;
        x_low = centre.x - radius;
        y_high = centre.y + radius;
        y_low = centre.y - radius;

        touchPlaceholderTex = TextureFromSprite.Convert(touchPlaceholder);
        float realPlaceHolderWidth = touchPlaceholderTex.width;
        float realPlaceHolderHeight = touchPlaceholderTex.height;
        canvasPlaceHolderWidth = canvas.scaleFactor * realPlaceHolderWidth;
        canvasPlaceHolderHeight = canvas.scaleFactor * realPlaceHolderHeight;
    }


    /*
     * Use the center of the circle as the origin. 

dir = org - pos;
Then you would do something like arctan2(dir.y,dir.x) * RAD_2PI; 
     */

    public void SetCamera(Camera camera)
    {//this camera is used to track the position of the touch/mouse_click for the navigation to take place
        this.camera = camera;
    }

    Vector3 screenPosition;
    bool boPressed;
    float Angle;

    bool is_active_touch = false;
    int gearNumber;
    private void Update()
    {
        bool is_toroid_clicked = TouchOrMouseClick.TrackMouseOrTouchCoordGUI(camera, out Vector3 position_toroid);
        bool is_anything_clicked = TouchOrMouseClick.TrackMouseOrTouchCoordGUIAndNotGUI(camera, out Vector3 position_anything);

        //PrintDebugLog.PrintDebug(string.Format("is_toroid_clicked = {0}, is_anything_clicked = {1}", is_toroid_clicked, is_anything_clicked));

        if (!is_toroid_clicked
            && !is_anything_clicked)
        {
            is_active_touch = false;
        }
        else
        {
            //Debug.Log(string.Format("centre = {0}, canvasWidth = {1}, screen_position = {2}, distance = {3}", centre, canvasWidth, screen_position, distance)); 
            float distance;
            Vector3 position;
            if (is_toroid_clicked)
            {
                position = position_toroid;
            }
            else
            {
                position = position_anything;
            }

            screenPosition = camera.WorldToScreenPoint(position);

            Vector3 diff = screenPosition - centre;
            distance = diff.magnitude;

            if (is_toroid_clicked
                && !is_active_touch)
            {
                if (distance + canvasPlaceHolderWidth / 2 <= radius)
                {
                    is_active_touch = true;
                }
            }

            if (is_active_touch)
            {
                var A = new Vector2(screenPosition.x, screenPosition.y);
                var B = new Vector2(centre.x, centre.y);
                var C = new Vector2(x_high, centre.y);
                Angle = Vector2.SignedAngle(A - B, C - B);
            }
        }

        if (!is_active_touch) gearNumber = 0;
        else
        {
            if(is_toroid_clicked)
            {
                gearNumber = 1;
            }
            else
            {
                gearNumber = 2;
            }
        }


         boPressed = is_active_touch;
    }

    public bool GetPressedDirection(out float navAngle, out int gearNumber)
    {
        /*
         navAngle adjustment: clockwise, 0->90->180->270, 0 is the upper centre
         */
                 
        if (!boPressed)
        {
            navAngle = 0;
        }
        else
        {
            if(Angle >= -90 && Angle < 0)
            {//1st quarter, clockwise
                navAngle = Angle + 90;
            }
            else if (Angle >= 0 && Angle < 90)
            {//2nd quarter, clockwise
                navAngle = Angle + 90;
            }
            else if (Angle >= 90 && Angle < 180)
            {//3rd quarter, clockwise
                navAngle = Angle + 90;
            }
            else
            {// (Angle >= -180 && Angle < -90)
                //4th quarter, clockwise
                navAngle = Angle + 450;
            }
        }

        if (boPressed)
        {
            //Debug.Log(string.Format("Angle = {0}, {1}", Angle, navAngle));
        }
        gearNumber = this.gearNumber;

        return boPressed;
    }

    void OnGUI()
    {
        if (!boPressed) return;

        if (Event.current.type.Equals(EventType.Repaint))
        {
            //clip x & y
            var touch_x = screenPosition.x;
            var touch_y = screenPosition.y;

            float x = touch_x - canvasPlaceHolderWidth / 2;
            float y = Screen.height - touch_y - canvasPlaceHolderHeight / 2;

            Graphics.DrawTexture(new Rect(x, y, canvasPlaceHolderWidth, canvasPlaceHolderHeight), touchPlaceholderTex);
            //Graphics.DrawTexture(new Rect(centre.x, centre.y, 5, 5), touchPlaceholderTex); attempt to draw dot in the centre, buggy, sometimes it's positioned in order part of screen

        }  
    }
}



#else

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
    bool isOverlayActive = false;

    SpriteRenderer spriteRend;

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
        overlayObject.transform.parent = transform; // make 'TouchOverlay' as child of this object

        spriteRend = overlayObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
        spriteRend.drawMode = SpriteDrawMode.Sliced;
        spriteRend.sprite = touchPlaceholder;
        overlayObject.SetActive(isOverlayActive);

        //color cannot be changed directly
        var color = spriteRend.color;
        color.a = 0.333f;
        spriteRend.color = color;

        spriteRend.size = new Vector2(4, 4);
        spriteRend.sortingOrder = 0;
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

        Vector3 screen_position = new Vector3(0, 0, 0);

        bool clicked = TouchOrMouseClick.TrackMouseOrTouchCoordGUI(camera, out Vector3 position);
        if(clicked)
        {
            screen_position = camera.WorldToScreenPoint(position);

            Vector3 diff = screen_position - centre;
            var distance = diff.magnitude;

            //Debug.Log(string.Format("centre = {0}, canvasWidth = {1}, screen_position = {2}, distance = {3}", centre, canvasWidth, screen_position, distance)); 
            if (distance <= radius)
            {
                Debug.Log("inside circle");

                bo_active = true;

            }
        }

        switch(isOverlayActive)
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
            overlayObject.transform.position =  position;
            overlayObject.transform.LookAt(camera.transform);
        }
    }
}

#endif