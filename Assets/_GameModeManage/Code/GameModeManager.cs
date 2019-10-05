using UnityEngine;
using UnityEngine.UI;

namespace MyTankGame
{
    public class GameModeManager : MonoBehaviour
    {

        /* This is not flexible at all, but should be fine as we'll have a limited set of cameras in general
         */

        #region Variables
        private Radar radar;
        private bool boRadarMode = false;
        public GameObject[] gunnerCamControls;

        //current state
        GameModeEnumerator.CameraMode _enCurrentCameraState = GameModeEnumerator.CameraMode.TopView;
        GameModeEnumerator.CameraMode _enNextCameraState = GameModeEnumerator.CameraMode.TopView;
        string _state_name;

        public Text _cameraText;
        #endregion

        #region Custom Public Methods
        public void Init(Radar rad, bool isRadarMode, Text text)
        {
            SetLinkDisplayGameModeOnButton(text);
            boRadarMode = isRadarMode;
            if (rad != null)
            {
                radar = rad;
                radar?.SetActive(boRadarMode);
            }

            TurnOffEverything();

            SetNextCameraState(GameModeEnumerator.CameraMode.TopView, "top view");
            ApplyCameraState(_enCurrentCameraState);

            //next state
            SetNextCameraState(GameModeEnumerator.CameraMode.SniperView, "sniper mode");
        }

        public void SetLinkDisplayGameModeOnButton(Text text)
        {
            _cameraText = text;
        }

        public void ChooseCamera()
        {
            TurnOffEverything();
            boRadarMode = false;

            switch (_enNextCameraState)
            {
                case GameModeEnumerator.CameraMode.TopView:
                    ApplyCameraState(_enNextCameraState);
                    SetNextCameraState(GameModeEnumerator.CameraMode.SniperView, "sniper mode");
                    break;

                case GameModeEnumerator.CameraMode.SniperView:
                    ApplyCameraState(_enNextCameraState);
                    if (null != gunnerCamControls)
                    {
                        foreach (GameObject gunnerCamControl in gunnerCamControls)
                        {
                            gunnerCamControl.SetActive(true);
                        }
                    }
                    SetNextCameraState(GameModeEnumerator.CameraMode.RadarView, "radar view");
                    break;

                case GameModeEnumerator.CameraMode.RadarView:
                    ApplyCameraState(_enNextCameraState);
                    boRadarMode = true;
                    SetNextCameraState(GameModeEnumerator.CameraMode.TopView, "top view");
                    break;

                default:
                    ApplyCameraState(_enCurrentCameraState);
                    break;
            }

            radar.SetActive(boRadarMode);
        }

        #endregion

        #region Custom Methods

        void TurnOffEverything()
        {
            if (gunnerCamControls != null)
            {
                foreach (var obj in gunnerCamControls)
                {
                    obj.SetActive(false);
                }
            }
        }

        void SetNextCameraState(GameModeEnumerator.CameraMode enCameraIdx, string state_name)
        {
            _enCurrentCameraState = _enNextCameraState;
            _enNextCameraState = enCameraIdx;
            _state_name = state_name;
        }

        void ApplyCameraState(GameModeEnumerator.CameraMode cameraMode)
        {
            _cameraText.text = (_state_name);
            SignalEventCameraChangedMode(cameraMode);
        }

        #endregion

        #region Events
        public const string event_name__change_camera_mode = "changeCameraMode";
        private void SignalEventCameraChangedMode(GameModeEnumerator.CameraMode cameraMode)
        {
            EventManager.TriggerEvent(event_name__change_camera_mode, cameraMode);
        }
        #endregion

    }
}

