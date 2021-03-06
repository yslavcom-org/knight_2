﻿using UnityEngine;
using UnityEngine.UI;

public class CrossHairControl : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

    [SerializeField]
    private Image image;

    [SerializeField]
    private Sprite crossHairSprite_idle;
    [SerializeField]
    private Sprite crossHairSprite_enaged;
    
    Vector2 sizeDelta;
    Vector2 canvasScale;

    Vector3 centre;
    float realWidth, realHeight;
    float canvasWidth, canvasHeight; // they both must be equal , i.e. we deal with circle
    float x_high, x_low;
    float y_high, y_low;

    float radius;
    Canvas canvas;

    float canvasPlaceHolderWidth, canvasPlaceHolderHeight;

    void Start()
    {
        centre = this.transform.position;

        realWidth = image.sprite.rect.width;
        realHeight = image.sprite.rect.height;

        this.image.sprite = crossHairSprite_idle;

        canvas = GetComponentInParent<Canvas>();

        canvasWidth = canvas.scaleFactor * realWidth;
        canvasHeight = canvas.scaleFactor * realHeight;
        radius = canvasWidth / 2;
        x_high = centre.x + radius;
        x_low = centre.x - radius;
        y_high = centre.y + radius;
        y_low = centre.y - radius;
    }


    public void SetCamera(Camera camera)
    {
        this.camera = camera;
    }

    Vector3 screenPosition;
    [SerializeField]
    bool boPressed;
    float Angle;

    enum ClickCtrl
    {
        Idle,
        Active,
        JustLocked
    };
    ClickCtrl clickCtrl = ClickCtrl.Idle;

    float relative_distance = 0f;

    public void SetPressed()
    {
        boPressed = true;
    }

    public void ClearPressed()
    {
        boPressed = false;
    }

    public void SetEngaged(bool boEngaged)
    {
        if (false == boEngaged)
        {
            this.image.sprite = crossHairSprite_idle;
        }
        else
        {
            this.image.sprite = crossHairSprite_enaged;
            clickCtrl = ClickCtrl.JustLocked;
        }
    }

    private void Update()
    {
        float distance = 0f;

        bool is_toroid_clicked = TouchOrMouseClick.TrackMouseOrTouchCoordGUI(camera, out Vector3 position_toroid);
        bool is_anything_clicked = TouchOrMouseClick.TrackMouseOrTouchCoordGUIAndNotGUI(camera, out Vector3 position_anything);
        if (!is_toroid_clicked
            && !is_anything_clicked)
        {
            clickCtrl = ClickCtrl.Idle;
        }
        else
        {
            Vector3 position = (is_toroid_clicked)
                ? position_toroid : position_anything;

            screenPosition = camera.WorldToScreenPoint(position);

            Vector3 diff = screenPosition - centre;
            distance = diff.magnitude;

            if (is_toroid_clicked
                && ClickCtrl.Idle == clickCtrl)
            {
                if (distance <= radius)
                {
                    clickCtrl = ClickCtrl.Active;
                }
            }

            if (ClickCtrl.Active == clickCtrl)
            {
                var A = new Vector2(screenPosition.x, screenPosition.y);
                var B = new Vector2(centre.x, centre.y);
                var C = new Vector2(x_high, centre.y);
                Angle = Vector2.SignedAngle(A - B, C - B);

                relative_distance = distance / radius;
            }
            else
            {
                relative_distance = 0f;
            }
        }
    }

    public bool GetPressedDirection(out float navAngle, out float navRelativeDistance)
    {
        /*
         navAngle adjustment: clockwise, 0->90->180->270, 0 is the upper centre
         */

        if (ClickCtrl.Active != clickCtrl
            || !boPressed)
        {
            navAngle = 0;
        }
        else
        {
            if (Angle >= -90 && Angle < 0)
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

            //PrintDebugLog.PrintDebug(string.Format("navAngle = {0}, boPressed = {1}", navAngle, boPressed));
        }

        navRelativeDistance = relative_distance;

        return (ClickCtrl.Active == clickCtrl 
            && boPressed)
            ? true : false;
    }
}