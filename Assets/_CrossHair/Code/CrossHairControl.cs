using UnityEngine;
using UnityEngine.UI;

public class CrossHairControl : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

    [SerializeField]
    private Image image;

    Vector2 sizeDelta;
    Vector2 canvasScale;

    Vector3 centre;
    float realWidth, realHeight;
    float canvasWidth, canvasHeight; // they both must be equal , i.e. we deal with circle
    float x_high, x_low;
    float y_high, y_low;

    float radius;
    Canvas canvas;

    //int dir_x, dir_y;

    void Start()
    {
        centre = this.transform.position;

        realWidth = image.sprite.rect.width;
        realHeight = image.sprite.rect.height;

        canvas = GetComponentInParent<Canvas>();

        canvasWidth = canvas.scaleFactor * realWidth;
        canvasHeight = canvas.scaleFactor * realHeight;
        radius = canvasWidth / 2;
        x_high = centre.x + radius;
        x_low = centre.x - radius;
        y_high = centre.y + radius;
        y_low = centre.y - radius;
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


    Vector3 screenPosition;
    bool boPressed;
    float Angle;

    bool is_active_touch = false;
    int gearNumber;
    private void Update()
    {
        //Debug.Log(string.Format("centre = {0}, canvasWidth = {1}, screen_position = {2}, distance = {3}", centre, canvasWidth, screen_position, distance));

        bool is_toroid_clicked = TouchOrMouseClick.TrackMouseOrTouchCoordGUI(camera, out Vector3 position_toroid);
        bool is_anything_clicked = TouchOrMouseClick.TrackMouseOrTouchCoordGUIAndNotGUI(camera, out Vector3 position_anything);
        if (!is_toroid_clicked
            && !is_anything_clicked)
        {
            is_active_touch = false;
        }
        else
        {
            //Debug.Log(string.Format("centre = {0}, canvasWidth = {1}", centre, canvasWidth)); 
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
                if (distance <= radius)
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
            if (is_toroid_clicked)
            {
                gearNumber = 1;
            }
            else
            {
                gearNumber = 2;
            }

            Debug.Log(string.Format("Angle = {0}, gearNumber = {1}", Angle, gearNumber));
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
        }

        gearNumber = this.gearNumber;

        return boPressed;
    }

}