using UnityEngine.EventSystems;
using UnityEngine;

public class TouchOrMouseClick : MonoBehaviour
{
    enum EmMouseButton
    {
        Primary = 0,
        Secondary = 1,
        Middle = 2,
    };

    private static bool MouseClick(Camera cam, out Vector3 outMouseTrackPosition)
    {
        bool touchedScreenOrMouseClicked = false;

        Vector3 mouseTrackPosition = new Vector3(0, 0, 0);

        if (Input.GetMouseButtonDown((int)EmMouseButton.Primary))
        {
            Ray screenRay = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(screenRay, out RaycastHit hit))
            {
                mouseTrackPosition = hit.point;
                touchedScreenOrMouseClicked = true;
            }
        }

        outMouseTrackPosition = mouseTrackPosition;
        return touchedScreenOrMouseClicked;
    }

    private static bool Touch(Camera cam, out Vector3 outTouchPosition)
    {
        bool touchedScreenOrMouseClicked = false;

        Vector3 touchPosition = new Vector3(0, 0, 0);

        if (Input.touchCount > 0 && Input.touchCount < 2)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Ray ray = cam.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    touchPosition = hit.point;
                    touchedScreenOrMouseClicked = true;
                }
            }
        }

        outTouchPosition = touchPosition;
        return touchedScreenOrMouseClicked;
    }

    public static bool KeyPress(out float outForwardInput,
        out float outRotationInput)
    {
        bool movementKeyDown = false;

        if (Input.GetKeyDown(KeyCode.UpArrow)
    || Input.GetKey(KeyCode.UpArrow)

    || Input.GetKeyDown(KeyCode.DownArrow)
    || Input.GetKey(KeyCode.DownArrow)

    || Input.GetKeyDown(KeyCode.LeftArrow)
    || Input.GetKey(KeyCode.LeftArrow)

    || Input.GetKeyDown(KeyCode.RightArrow)
    || Input.GetKey(KeyCode.RightArrow))
        {
            movementKeyDown = true;
        }

        outForwardInput = Input.GetAxis("Vertical");
        outRotationInput = Input.GetAxis("Horizontal");

        return movementKeyDown;
    }

    enum enClick
    {
        NotGui = 0,
        GuiOnly = 1,
        Both = 2,
    };

    private static bool IsTouchedOrClicked(enClick click, Camera cam,
        out Vector3 touchOrClickPosition)
    {
        if (HardcodedValues.boAndroidOrIphone)
        {//Android smarphone
         //touch screen touching
            bool proceed = true;
            switch(click)
            {
                case enClick.GuiOnly:
                    proceed = EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
                    break;

                case enClick.NotGui:
                    proceed = !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
                    break;

                default:
                    proceed = true;
                    break;
            }

            if (proceed)
            {
                return Touch(cam, out touchOrClickPosition);
            }
            else
            {
                touchOrClickPosition = new Vector3(0, 0, 0);
                return false;
            }
        }
        else
        {
         /*non-android, mouse*/
            bool proceed = true;
            switch (click)
            {
                case enClick.GuiOnly:
                    proceed = EventSystem.current.IsPointerOverGameObject();
                    break;

                case enClick.NotGui:
                    proceed = !EventSystem.current.IsPointerOverGameObject();
                    break;

                default:
                    proceed = true;
                    break;
            }

            if (proceed)
            {
                return MouseClick(cam, out touchOrClickPosition);
            }
            else
            {
                touchOrClickPosition = new Vector3(0, 0, 0);
                return false;
            }
        }
    }

    public static bool GetMouseOrTouchCoord(Camera cam, 
        out Vector3 touchOrClickPosition)
    {
        return IsTouchedOrClicked(enClick.NotGui, cam, out touchOrClickPosition);
    }

    public static bool GetMouseOrTouchCoordGUI(Camera cam,
       out Vector3 touchOrClickPosition)
    {
        return IsTouchedOrClicked(enClick.GuiOnly, cam, out touchOrClickPosition);
    }
}
