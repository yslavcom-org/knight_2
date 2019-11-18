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

        #region Variables
        private Camera m_Camera;
        private bool boPlayer;
        ToroidNavigator toroidNavigator;
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

        private int forwardInput;
        public int ForwardInput
        {
            get { return forwardInput; }
        }

        private int rotationInput;
        public int RotationInput
        {
            get { return rotationInput; }
        }

        private bool navigationControlActive = false;
        public bool NavigationControlActive
        {
            get { return navigationControlActive; }
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

            var obj = GameObject.Find(HardcodedValues.toroidalNavigationButton);
            if(obj)
            {
                toroidNavigator = obj.GetComponent<ToroidNavigator>();
            }
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
            int forward = 0;
            int rot = 0;
            bool bo_active = false;
            if(toroidNavigator)
            {
                bo_active = toroidNavigator.GetPressedDirection(out forward, out rot);
            }
            forwardInput = forward;
            rotationInput = rot;
            navigationControlActive = bo_active;
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