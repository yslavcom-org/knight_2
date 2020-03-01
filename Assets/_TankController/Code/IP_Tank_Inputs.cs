﻿using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace TankDemo
{
    public class IP_Tank_Inputs : MonoBehaviour
    {
        #region Event Listeners
        private UnityAction<object> someListener;
        private string event_name = HardcodedValues.evntName__tankShootEventString;
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

        private bool navigationKeyPressed = false;
        public bool NavigationKeyPressed
        {
            get { return navigationKeyPressed; }
        }

        private bool navigationToroidalControlActive = false;
        public bool NavigationToroidalControlActive
        {
            get { return navigationToroidalControlActive; }
        }
        private float navigationToroidalAngle;
        public float NavigationToroidalAngle
        {
            get { return navigationToroidalAngle; }
        }
        private float navigationToroidalGearNum;
        public float NavigationToroidalGearNum
        {
            get { return navigationToroidalGearNum; }
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

        private void HandleUserInputs()
        {
            DoUserInputs.HandleKeyboard(out forwardInput, out rotationInput, out navigationKeyPressed);
            DoUserInputs.HandleToroidNavigator(ref toroidNavigator, out navigationToroidalAngle, out navigationToroidalGearNum, out navigationToroidalControlActive);
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