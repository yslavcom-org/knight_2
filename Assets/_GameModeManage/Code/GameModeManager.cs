using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MyTankGame
{
    public class GameModeManager : MonoBehaviour
    {
        /* This is not flexible at all, but should be fine as we'll have a limited set of cameras in general
         */

        #region Custom Enumerators
        enum EnGameModeIdx
        {
            TopView = 0,
            SniperView,
            RadarView,
            Count
        }
        #endregion

        #region Variables
        public Transform targetTopDownByDefault;
        public GameObject radar;

        public GameObject[] cameras;
        public GameObject[] gunnerCamControls;

        //current state
        EnGameModeIdx _enCurrentCameraState;
        EnGameModeIdx _enNextCameraState;
        string _state_name;

        public Text _cameraText;

        private bool boRadarMode = false;
        public bool BoRadarMode { get { return boRadarMode; } }

        private UnityAction<object> listenerHomingMissileLaunched;
        private UnityAction<object> listenerHomingMissileDestroyed;

        private IndiePixel.Cameras.IP_TopDown_Camera topDownCameraComponent;

        #endregion

        #region Unity Methods

        private void Start()
        {
            topDownCameraComponent = cameras[(int)EnGameModeIdx.TopView].GetComponent<IndiePixel.Cameras.IP_TopDown_Camera>();
            if (null != topDownCameraComponent)
            {
                SetTargetToTopDownCamera(targetTopDownByDefault);
            }
        }

        private void Awake()
        {
            topDownCameraComponent = cameras[(int)EnGameModeIdx.TopView].GetComponent<IndiePixel.Cameras.IP_TopDown_Camera>();

            #region Log Errors
            if ((int)EnGameModeIdx.Count != cameras.Length)
            {
                //Debug.LogError("ensure all cameras are enumerated");
            }
            #endregion

            TurnOffEverything();

            SetNextCameraState(EnGameModeIdx.TopView, "top view");
            ApplyCameraState();

            //next state
            SetNextCameraState(EnGameModeIdx.SniperView, "sniper mode");

            radar.SetActive(boRadarMode);

            listenerHomingMissileLaunched = new UnityAction<object>(HomingMissilwWasLaunched);
            listenerHomingMissileDestroyed = new UnityAction<object>(HomingMissilwWasDestroyed);

        }
        #endregion

        #region Custom Public Methods
        public void ChooseCamera()
        {
            TurnOffEverything();
            boRadarMode = false;

            switch (_enNextCameraState)
            {
                case EnGameModeIdx.TopView:
                    ApplyCameraState();
                    SetNextCameraState(EnGameModeIdx.SniperView, "sniper mode");
                    break;

                case EnGameModeIdx.SniperView:
                    ApplyCameraState();
                    foreach (GameObject gunnerCamControl in gunnerCamControls)
                    {
                        gunnerCamControl.SetActive(true);
                    }
                    SetNextCameraState(EnGameModeIdx.RadarView, "radar view");
                    break;

                case EnGameModeIdx.RadarView:
                    ApplyCameraState();
                    boRadarMode = true;
                    SetNextCameraState(EnGameModeIdx.TopView, "top view");
                    break;

                default:
                    ApplyCameraState();
                    break;
            }

            radar.SetActive(boRadarMode);
        }

        public const string event_name__homing_missile_launched = "missileLaunch";
        public const string event_name__homing_missile_destroyed = "missileDestroy";
        void OnEnable()
        {
            EventManager.StartListening(event_name__homing_missile_launched, listenerHomingMissileLaunched);
            EventManager.StartListening(event_name__homing_missile_destroyed, listenerHomingMissileDestroyed);
        }

        void OnDisable()
        {
            EventManager.StopListening(event_name__homing_missile_launched, listenerHomingMissileLaunched);
            EventManager.StopListening(event_name__homing_missile_destroyed, listenerHomingMissileDestroyed);
        }

        #endregion

        #region Custom Private Methods

        private void SetTargetToTopDownCamera(Transform target)
        {
            topDownCameraComponent.SetTarget(target);
        }

        private void HomingMissilwWasLaunched(object arg)
        {
            //set the homing missile as the target to the top down camera
            SetTargetToTopDownCamera((Transform)arg);

            //enable the top down camera and disable the others
            for (int iter=0; iter < cameras.Length; iter++)
            {
                cameras[iter].SetActive(false);
            }
            cameras[(int)EnGameModeIdx.TopView].SetActive(true);
        }

        private void HomingMissilwWasDestroyed(object arg)
        {
            //enable the camera corresponding to the state
            SetTargetToTopDownCamera(targetTopDownByDefault);
            cameras[(int)EnGameModeIdx.TopView].SetActive(false);
            cameras[(int)_enCurrentCameraState].SetActive(true);
        }

        void TurnOffEverything()
        {
            foreach (GameObject camera in cameras)
            {
                camera.SetActive(false);
            }

            foreach (GameObject gunnerCamControl in gunnerCamControls)
            {
                gunnerCamControl.SetActive(false);
            }
        }

        void SetNextCameraState(EnGameModeIdx enCameraIdx, string state_name)
        {
            _enCurrentCameraState = _enNextCameraState;
            _enNextCameraState = enCameraIdx;
            _state_name = state_name;
        }

        void ApplyCameraState()
        {
            cameras[(int)_enNextCameraState].SetActive(true);
            _cameraText.text = (_state_name);
        }

        #endregion
    }
}

