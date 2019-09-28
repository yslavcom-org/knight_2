using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndiePixel.Cameras
{
    public class IP_TopDown_Camera : IP_Base_Camera
    {
        #region Variables

        public float m_Height = 10f;
        public float m_Distance = 20f;
        public float m_Angle = 45f;
        public float m_SmoothSpeed = 0.5f;

        private Vector3 refVelocity;
        #endregion

        #region Main Methods
        #endregion

        #region Helper Methods

        protected override void HandleCamera()
        {
            base.HandleCamera();

            //Build world position vector
            Vector3 worldPosition = (Vector3.forward * -m_Distance) + (Vector3.up * m_Height);
            //Debug.DrawLine(m_Target.position, worldPosition, Color.red);

            //Build our Rotated vector
            Vector3 rotatedVector = Quaternion.AngleAxis(m_Angle, Vector3.up) * worldPosition;
            //Debug.DrawLine(m_Target.position, rotatedVector, Color.green);

            //Move our position
            Vector3 flatTargetPosition = m_Target.position;
            flatTargetPosition.y = 0f;
            Vector3 finalPosition = flatTargetPosition + rotatedVector;
            //Debug.DrawLine(m_Target.position, finalPosition, Color.blue);
            transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref refVelocity, m_SmoothSpeed);
            transform.LookAt(m_Target.position);
        }

#if false
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
            if (null != m_Target)
            {
                Gizmos.DrawLine(transform.position, m_Target.position);

                Gizmos.DrawSphere(m_Target.position, 1.5f);
            }

            Gizmos.DrawSphere(transform.position, 1.5f);
        }
#endif

#endregion
    }
}
