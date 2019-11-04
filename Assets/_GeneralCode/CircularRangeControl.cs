using UnityEngine;
using UnityEngine.UI;

    /// <summary>
    /// A circular range / slider control for Unity UI
    /// By Pietro Polsinelli https://twitter.com/ppolsinelli
    /// </summary>
    public class CircularRangeControl : MonoBehaviour
    {
        public Transform Origin; //center of rotation
        public Image ImageSelected; //drag here the image of type filled radial top
        public Text Angle; //value textual feedback
        public Text DetectedValue; //value textual feedback

        public Toggle DeltaChanges; // if on the value can only change by a delta

        public int Scale = 360; //value scale to use

        private int CurrentValue;
        public State CircularButtonState = State.NOT_DRAGGING;

        const float full_perimeter = 360f;

        public enum State
        {
            NOT_DRAGGING,
            DRAGGING,
        }

        public void DragOnCircularArea(bool isClick)
        {
            //we ignore the click event due to dragging in order to 
            //ignore beyond max set with drag and enable it on click / touch
            if (isClick && CircularButtonState == State.DRAGGING)         {
                CircularButtonState = State.NOT_DRAGGING;
                return;
            }
            
            if (isClick)
                CircularButtonState = State.NOT_DRAGGING;
            else
            {
                CircularButtonState = State.DRAGGING;
            }

            float f = Vector3.Angle(Vector3.up, Input.mousePosition - Origin.position);
            bool onTheRight = Input.mousePosition.x > Origin.position.x;
            int detectedValue = onTheRight ? (int)f : 180 + (180 - (int)f);

        //if (onTheRight)
          //  Debug.Log("click on right");
        //else
          //  Debug.Log("click on left");

        if (detectedValue > 350)
                detectedValue = 360;
            else if (CurrentValue == 360 && detectedValue < 10)
                detectedValue = 360;
            else if (CurrentValue == 0 && detectedValue > 350)
                detectedValue = 0;
            else if (detectedValue < 10)
                detectedValue = 0;

            if (!isClick && DeltaChanges != null && DeltaChanges.isOn)
            {
                if (detectedValue <= CurrentValue && Mathf.Abs(CurrentValue - detectedValue) > 180)
                    detectedValue = CurrentValue;
                else if (CurrentValue == 0 && detectedValue > 270)
                    detectedValue = CurrentValue;
            }

            CurrentValue = detectedValue;
            Angle.text = "" + (int)(CurrentValue * Scale / full_perimeter);
            ImageSelected.fillAmount = CurrentValue / full_perimeter;
        }
    }

