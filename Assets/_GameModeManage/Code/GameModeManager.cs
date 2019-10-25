using UnityEngine;
using UnityEngine.UI;

namespace MyTankGame
{
    public class GameModeManager : MonoBehaviour
    {
        /* This is not flexible at all, but should be fine as we'll have a limited set of cameras in general
         */

        #region enums
        enum CrossHairIdx{
            CrossHair_NotLocked = 0,
            CrossHairLocked = 1,
        };
        #endregion

        #region Variables
        private Radar radar;
        private bool boRadarMode = false;
        

        //current state
        GameModeEnumerator.CameraMode _enCurrentCameraState = GameModeEnumerator.CameraMode.TopView;
        GameModeEnumerator.CameraMode _enNextCameraState = GameModeEnumerator.CameraMode.TopView;
        string _state_name;

        private Text m_cameraText;
        private GameObject[] m_gunnerCamControls;

        public bool IsTankGunLockTarget;
        #endregion

        #region Custom Public Methods
        public void Init(Radar rad, bool isRadarMode, Text text, GameObject[] gunnerCamControls)
        {
            SetLinkDisplayGameModeOnButton(text);
            boRadarMode = isRadarMode;
            if (rad != null)
            {
                radar = rad;
                radar?.SetActive(boRadarMode);
            }

            m_gunnerCamControls = gunnerCamControls;

            TurnOffEverything();

            SetNextCameraState(GameModeEnumerator.CameraMode.TopView, "top view");
            ApplyCameraState(_enCurrentCameraState);

            //next state
            SetNextCameraState(GameModeEnumerator.CameraMode.SniperView, "sniper mode");
        }

        public void SetLinkDisplayGameModeOnButton(Text text)
        {
            m_cameraText = text;
        }

        private void EnableCrossHairImage(CrossHairIdx crossHairIdx)
        {
            if (null != m_gunnerCamControls)
            {
                foreach (GameObject gunnerCamControl in m_gunnerCamControls)
                {
                    gunnerCamControl.SetActive(false);
                }
                m_gunnerCamControls[(int)crossHairIdx]?.SetActive(true);
            }
        }

        public void SetGunCrossHairIfAny()
        {
            if(_enCurrentCameraState == GameModeEnumerator.CameraMode.SniperView)
            {
                EnableCrossHairImage(IsTankGunLockTarget
                    ? CrossHairIdx.CrossHairLocked : CrossHairIdx.CrossHair_NotLocked);
            }
        }


        public void ChooseCamera()
        {
            TurnOffEverything();
            boRadarMode = false;
            IsTankGunLockTarget = false; // it will be updated if a target is locked

            switch (_enNextCameraState)
            {
                case GameModeEnumerator.CameraMode.TopView:
                    ApplyCameraState(_enNextCameraState);
                    SetNextCameraState(GameModeEnumerator.CameraMode.SniperView, "sniper mode");
                    break;

                case GameModeEnumerator.CameraMode.SniperView:
                    ApplyCameraState(_enNextCameraState);
                    EnableCrossHairImage(IsTankGunLockTarget 
                        ? CrossHairIdx.CrossHairLocked : CrossHairIdx.CrossHair_NotLocked);
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
            if (m_gunnerCamControls != null)
            {
                foreach (var obj in m_gunnerCamControls)
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
            m_cameraText.text = (_state_name);
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

