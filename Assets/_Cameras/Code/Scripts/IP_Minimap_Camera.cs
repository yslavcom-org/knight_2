using System;
using UnityEngine;

namespace IndiePixel.Cameras
{
    public class IP_Minimap_Camera : IP_Base_Camera
    {
        Vector3 position;
        public void SetCameraPosition(Vector3 pos)
        {
            position = pos;
        }


        protected override void HandleCameraLateUpdate()
        {
            base.HandleCameraLateUpdate();

            transform.SetPositionAndRotation(position, Quaternion.identity);
            transform.LookAt(m_Target.position);
        }
    }
}
