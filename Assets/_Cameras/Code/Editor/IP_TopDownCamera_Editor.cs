using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace IndiePixel.Cameras
{
    [CustomEditor(typeof(IP_TopDown_Camera))]
    public class IP_TopDownCamera_Editor : Editor
    {
#region Variables
        private IP_TopDown_Camera targetCamera;
        private Transform m_Target;
        private bool isPosition;
#endregion

#region Main Methods
        private void OnEnable()
        {
            targetCamera = (IP_TopDown_Camera)target;
            isPosition = targetCamera.GetTarget(out m_Target);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
        void OnSceneGUI()
        {
            if (!isPosition) return;

            //Storing target reference
            Transform camTarget = m_Target;

            //draw distance
            Handles.color = new Color(0f, 0f, 1f, 0.15f);
            Handles.DrawSolidDisc(m_Target.position, Vector3.up, targetCamera.m_Distance);
            Handles.color = new Color(0f, 1f, 0f, 0.75f);
            Handles.DrawWireDisc(m_Target.position, Vector3.up, targetCamera.m_Distance);

            //slider reference
            Handles.color = new Color(1f, 0f, 0f, 0.75f);
            targetCamera.m_Distance = Handles.ScaleSlider(targetCamera.m_Distance, camTarget.position, -camTarget.forward, Quaternion.identity, targetCamera.m_Distance, 1f);
            targetCamera.m_Distance = Mathf.Clamp(targetCamera.m_Distance, 5f, float.MaxValue);

            Handles.color = new Color(0f, 0f, 1f, 0.75f);
            targetCamera.m_Height = Handles.ScaleSlider(targetCamera.m_Height, camTarget.position, camTarget.up, Quaternion.identity, targetCamera.m_Height, 1f);
            targetCamera.m_Height = Mathf.Clamp(targetCamera.m_Height, 5f, float.MaxValue);

            //create labels
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.white;
            labelStyle.alignment = TextAnchor.UpperCenter;
            Handles.Label(camTarget.position + (-camTarget.forward * targetCamera.m_Distance), "Distance", labelStyle);
            labelStyle.alignment = TextAnchor.MiddleRight;
            Handles.Label(camTarget.position + (Vector3.up * targetCamera.m_Height), "Height", labelStyle);

        }

#endregion
    }
}

