using UnityEngine.EventSystems;
using UnityEngine;

public class TouchOrMouseClick : MonoBehaviour
{
    enum EmMouseButton
    {
        enMouseButton__Primary = 0,
        enMouseButton__Secondary = 1,
        enMouseButton__Middle = 2,
    };

    private static bool MouseClick(Camera cam, out Vector3 outMouseTrackPosition)
    {
        bool touchedScreenOrMouseClicked = false;

        EventSystem eventSystem = EventSystem.current;
        Vector3 mouseTrackPosition = new Vector3(0, 0, 0);

#if false
        Ray screenRay = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(screenRay, out RaycastHit hit))
        {
            mouseTrackPosition = hit.point;
        }
#endif

        if (!EventSystem.current.IsPointerOverGameObject() /*non-android, mouse*/)
        {
            if (Input.GetMouseButtonDown((int)EmMouseButton.enMouseButton__Primary))
            {
                Ray screenRay = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(screenRay, out RaycastHit hit))
                {
                    mouseTrackPosition = hit.point;
                    touchedScreenOrMouseClicked = true;
                }
            }
        }

        outMouseTrackPosition = mouseTrackPosition;
        return touchedScreenOrMouseClicked;
    }

    private static bool Touch(Camera cam, out Vector3 outTouchPosition)
    {
        bool touchedScreenOrMouseClicked = false;

        EventSystem eventSystem = EventSystem.current;
        Vector3 touchPosition = new Vector3(0, 0, 0);

        if (!eventSystem.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
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

    public static bool GetMouseOrTouchCoord(Camera cam, 
        out Vector3 touchOrClickPosition)
    {
        if (HardcodedValues.boAndroidOrIphone)
        {//Android smarphone
         //touch screen touching
            return Touch(cam, out touchOrClickPosition);
        }
        else
        {//PC
            return MouseClick(cam, out touchOrClickPosition);
        }
    }
}
