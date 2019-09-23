using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        public GameObject radar;

        public GameObject[] cameras;
        public GameObject[] gunnerCamControls;

        //current state
        EnGameModeIdx _enNextCameraState;
        string _state_name;

        public Text _cameraText;

        private bool boRadarMode = false;
        public bool BoRadarMode { get { return boRadarMode; } }
        #endregion

        #region Unity Methods
        private void Awake()
        {
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
        #endregion

        #region Custom Private Methods
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

