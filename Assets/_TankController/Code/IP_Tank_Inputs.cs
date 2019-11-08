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

        private Vector3 mouseTrackNormal;
        public Vector3 MouseTrackNormal
        {
            get { return mouseTrackNormal; }
        }

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
            EventSystem eventSystem = EventSystem.current;
            bool boAndroidOrIphone = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer);

            if (boAndroidOrIphone)
            {//Android smarphone
                //touch screen touching
                if (!eventSystem.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    if (Input.touchCount > 0 && Input.touchCount < 2)
                    {
                        if (Input.GetTouch(0).phase == TouchPhase.Began)
                        {
                            RaycastHit hit;
                            Ray ray = m_Camera.ScreenPointToRay(Input.GetTouch(0).position);
                            if (Physics.Raycast(ray, out hit))
                            {
                                mouseTrackPosition = hit.point;
                                mouseTrackNormal = hit.normal;
                                touchedScreenOrMouseClicked = true;
                            }
                        }
                    }
                }
            }
            else
            {//PC
                Ray screenRay = m_Camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(screenRay, out hit))
                {
                    mouseTrackPosition = hit.point;
                    mouseTrackNormal = hit.normal;
                }

                if (!EventSystem.current.IsPointerOverGameObject() /*non-android, mouse*/)
                {
                    if (Input.GetMouseButtonDown((int)EmMouseButton.enMouseButton__Primary))
                    {
                        touchedScreenOrMouseClicked = true;
                    }
                }


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
                else
                {
                    movementKeyDown = false;
                }
                forwardInput = Input.GetAxis("Vertical");
                rotationInput = Input.GetAxis("Horizontal");
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