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
        private GameObject radar;
        private bool boRadarMode = false;
        

        //current state
        GameModeEnumerator.CameraMode _enCurrentCameraState = GameModeEnumerator.CameraMode.RadarView;
        GameModeEnumerator.CameraMode _enNextCameraState = GameModeEnumerator.CameraMode.RadarView;

        private Text m_cameraText;
        private CrossHairControl crossHairControl;
        private GameObject crossHairObj;

        [SerializeField]
        private bool IsTankGunLockTarget;
        #endregion

        #region Custom Public Methods
        public void Init(GameObject rad, bool isRadarMode, Text text, GameObject crossHairObj)
        {
            SetLinkDisplayGameModeOnButton(text);
            boRadarMode = isRadarMode;
            if (rad != null)
            {
                radar = rad;
                radar?.SetActive(boRadarMode);
            }

            if (null != crossHairObj)
            {
                crossHairControl = crossHairObj.GetComponent<CrossHairControl>();
                this.crossHairObj = crossHairObj;
            }

            TurnOffEverything();

            SetNextCameraState(GameModeEnumerator.CameraMode.RadarView);
            ApplyCameraState(_enCurrentCameraState);

            //next state
            SetNextCameraState(GameModeEnumerator.CameraMode.RadarView);
        }

        public void SetIsTankGunLockTarget(bool IsTankGunLockTarget)
        {
            this.IsTankGunLockTarget = IsTankGunLockTarget;
        }

        public bool GetIsTankGunLockTarget()
        {
            return IsTankGunLockTarget;
        }

        public void SetLinkDisplayGameModeOnButton(Text text)
        {
            m_cameraText = text;
        }

        private void EnableCrossHairImage(CrossHairIdx crossHairIdx)
        {
            if (null != crossHairObj)
            {
                crossHairObj.SetActive(true);
                if (null != crossHairControl)
                {
                    crossHairControl.SetEngaged((crossHairIdx == CrossHairIdx.CrossHairLocked) ? true : false);
                }
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
                case GameModeEnumerator.CameraMode.SniperView:
                    ApplyCameraState(_enNextCameraState);
                    EnableCrossHairImage(IsTankGunLockTarget 
                        ? CrossHairIdx.CrossHairLocked : CrossHairIdx.CrossHair_NotLocked);
                    SetNextCameraState(GameModeEnumerator.CameraMode.RadarView);
                    break;

                case GameModeEnumerator.CameraMode.RadarView:
                    ApplyCameraState(_enNextCameraState);
                    boRadarMode = true;
                    SetNextCameraState(GameModeEnumerator.CameraMode.SniperView);
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
            if (crossHairObj != null)
            {
                crossHairObj.SetActive(false);
            }
        }

        void SetNextCameraState(GameModeEnumerator.CameraMode enCameraIdx)
        {
            _enCurrentCameraState = _enNextCameraState;
            _enNextCameraState = enCameraIdx;
        }

        void ApplyCameraState(GameModeEnumerator.CameraMode cameraMode)
        {
            SignalEventCameraChangedMode(cameraMode);
        }

        #endregion

        #region Events
        private void SignalEventCameraChangedMode(GameModeEnumerator.CameraMode cameraMode)
        {
            EventManager.TriggerEvent(HardcodedValues.evntName__change_camera_mode, cameraMode);
        }
        #endregion

    }
}

