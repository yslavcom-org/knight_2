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
        private Radar radar;
        private bool boRadarMode = false;
        public GameObject[] gunnerCamControls;

        //current state
        EnGameModeIdx _enCurrentCameraState;
        EnGameModeIdx _enNextCameraState;
        string _state_name;

        public Text _cameraText;
#endregion

#region Unity Methods
        private void Awake()
        {
            TurnOffEverything();

            SetNextCameraState(EnGameModeIdx.TopView, "top view");
            ApplyCameraState();

            //next state
            SetNextCameraState(EnGameModeIdx.SniperView, "sniper mode");

            radar.SetActive(boRadarMode);
        }
#endregion

#region Custom Public Methods
        public void SetRadar(Radar rad)
        {
            radar = rad;
        }

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

#region Custom Methods

        void TurnOffEverything()
        {
            foreach (var obj in gunnerCamControls)
            {
                obj.SetActive(false);
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
            _cameraText.text = (_state_name);
        }

#endregion


    }
}

