using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace IndiePixel.Cameras
{

    public class IP_Camera_Menu : Editor
    {
        [MenuItem("Indie-Pixel/Cameras/Top Down Camera")]
        public static void CreateTopDownCamera()
        {
            GameObject[] selectedGO = Selection.gameObjects;
            //foreach(var selected in selectedGO)
            //{
            //    Debug.Log(selected.name);
            //}

            if (selectedGO.Length > 0
                && selectedGO[0].GetComponent<Camera>())
            {
                if (selectedGO.Length < 2)
                {
                    AttachTopDownScript(selectedGO[0].gameObject, null);
                }
                else if (selectedGO.Length == 2)
                {
                    AttachTopDownScript(selectedGO[0].gameObject, selectedGO[1].gameObject.transform);
                }
                else if (selectedGO.Length > 2)
                {
                    EditorUtility.DisplayDialog("Camera Tools", "You can only select 2 objects in the scene "
                        + "and the first selection needs to be a camera", "Ok");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Camera Tools", "You need to select a game object in the scene "
                    + "that has a camera component assigned to it", "Ok");
            }
        }

        static void AttachTopDownScript(GameObject aCamera, Transform aTarget)
        {
            //assign the top down script to camera
            IP_TopDown_Camera cameraScript = null;
            if (aCamera)
            {
                cameraScript = aCamera.AddComponent<IP_TopDown_Camera>();

                //check to see if we have the target and we have a script reference
                if (cameraScript && aTarget)
                {
                    //cameraScript.m_Target = aTarget;
                    cameraScript.SetTarget(aTarget);
                }

                Selection.activeGameObject = aCamera;
            }
        }
    }

}