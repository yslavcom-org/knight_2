using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraPositionManager : MonoBehaviour
{
    /* This is not flexible at all, but should be fine as we'll have a limited set of cameras in general
     */

    #region Custom Enumerators
    enum EnCameraIdx
    {
       TopView = 0,
       SniperView,

       Count
    }
    #endregion

    #region Variables
    public GameObject[] cameras;
    public GameObject[] gunnerCamControls;

    //current state
    EnCameraIdx _enNextCameraState;
    string _state_name;

    public Text _cameraText;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        #region Log Errors
        if((int)EnCameraIdx.Count != cameras.Length)
        { 
            //Debug.LogError("ensure all cameras are enumerated");
        }
        #endregion

        TurnOffEverything();

        SetNextCameraState(EnCameraIdx.TopView, "top view");
        ApplyCameraState();

        //next state
        SetNextCameraState(EnCameraIdx.SniperView, "sniper mode");
    }
    #endregion

    #region Custom Public Methods
    public void ChooseCamera()
    {
        TurnOffEverything();

        switch(_enNextCameraState)
        {
            case EnCameraIdx.TopView:
                ApplyCameraState();
                SetNextCameraState(EnCameraIdx.SniperView, "sniper mode");
                break;

            case EnCameraIdx.SniperView:
                ApplyCameraState();
                foreach (GameObject gunnerCamControl in gunnerCamControls)
                {
                    gunnerCamControl.SetActive(true);
                }
                SetNextCameraState(EnCameraIdx.TopView, "top view");
                break;

            default:
                ApplyCameraState();
                break;
        }
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

    void SetNextCameraState(EnCameraIdx enCameraIdx, string state_name)
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
