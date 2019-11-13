using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace TankDemo
{
    public class IP_Tank_Inputs : MonoBehaviour
    {
        #region Event Listeners
        private UnityAction<object> someListener;
        private string event_name = HardcodedValues.tankShootEventString;
        #endregion

        #region Custom Enumerators
        enum EmMouseButton
        {
            enMouseButton__Primary = 0,
            enMouseButton__Secondary = 1,
            enMouseButton__Middle = 2,
        };
        #endregion

        #region Variables
        private Camera m_Camera;
        private bool boPlayer;
        #endregion

        #region Properties
        public void SetThisPlayerMode(bool isPlayer)
        {
            boPlayer = isPlayer;
            if (isPlayer)
            {
                RegisterPlayerShootEvent();
            }
        }

        public void SetTrackCamera(Camera cam)
        {
            m_Camera = cam;
        }

        private Vector3 mouseTrackPosition;
        public Vector3 MouseTrackPosition
        {
            get { return mouseTrackPosition; }
        }

        //private Vector3 mouseTrackNormal;
        //public Vector3 MouseTrackNormal
        //{
        //    get { return mouseTrackNormal; }
        //}

        private bool touchedScreenOrMouseClicked = false;
        public bool BoMouseClicked
        {
            get { return touchedScreenOrMouseClicked; }
        }

        public void MouseClickAck()
        {
            touchedScreenOrMouseClicked = false;
        }

        private float forwardInput;
        public float ForwardInput
        {
            get { return forwardInput; }
        }

        private float rotationInput;
        public float RotationInput
        {
            get { return rotationInput; }
        }

        private bool movementKeyDown = false;
        public bool MovementKeyDown
        {
            get { return movementKeyDown; }
        }

        private bool fireGun = false;
        public bool BoFireGun
        {
            get { return fireGun; }
        }
        public void FireGunAck()
        {
            fireGun = false;
        }

        private float fireGunFrequency = 0.25f;
        public void FireGunFrequency(float delay)
        {
            fireGunFrequency = delay;
        }

        #endregion

        #region Builtin Methods
        // Update is called once per frame
        void Update()
        {
            if (null == m_Camera) return;

            if (boPlayer)
            {
                HandleUserInputs();
            }
        }

#if false
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(reticlePosition, 0.1f);
        }
#endif
        #endregion

        #region Built-in Methods
        private void Awake()
        {
            someListener = new UnityAction<object>(DoFireGun);
            boPlayer = false;
        }

        void OnDisable()
        {
            if (boPlayer)
            {
                EventManager.StopListening(event_name, someListener);
            }
        }
        #endregion

        #region Custom Methods
        void RegisterPlayerShootEvent()
        {
            EventManager.StartListening(event_name, someListener);
        }

        protected virtual void HandleUserInputs()
        {
            if (HardcodedValues.boAndroidOrIphone)
            { 
                touchedScreenOrMouseClicked = TouchOrMouseClick.GetMouseOrTouchCoord(m_Camera, out mouseTrackPosition);
            }
            else
            {//PC
                touchedScreenOrMouseClicked = TouchOrMouseClick.GetMouseOrTouchCoord(m_Camera, out mouseTrackPosition);
                movementKeyDown = TouchOrMouseClick.KeyPress(out forwardInput, out rotationInput);
            }
        }

        private void DoFireGun(object arg)
        {
            StartCoroutine(DoFireGun__CoRoutine());
        }

        IEnumerator DoFireGun__CoRoutine()
        {
            fireGun = true;
            // Tells Unity to wait for an interval of time (in seconds)
            yield return new WaitForSeconds(fireGunFrequency);
        }
        #endregion
    }

}