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

        bool boStayBehind = false;
        #endregion



        #region Main Methods

        public float cameraDistance = 10.0f;
#if false
        private Vector3 offset;

        [SerializeField]
        float offset_z = 5;
        [SerializeField]
        float offset_y = -5;

        float distance;
        Vector3 playerPrevPos, playerMoveDir;


        [SerializeField]
        private Vector3 offsetPosition;

        [SerializeField]
        private Space offsetPositionSpace = Space.Self;

        [SerializeField]
        private bool lookAt = true;
#endif

        private void Start()
        {
#if false
            offset = m_Target.position - transform.position;
#endif

#if false
            offset = transform.position - m_Target.position;

            distance = offset.magnitude;
            playerPrevPos = m_Target.position;
#endif
        }
#endregion

#region Helper Methods

        protected override void HandleCameraLateUpdate()
        {
            base.HandleCameraLateUpdate();

            if (!boStayBehind)
            {
                //Build world position vector
                Vector3 worldPosition = (Vector3.forward * -m_Distance) + (Vector3.up * m_Height);
                //Debug.DrawLine(m_Target.position, worldPosition, Color.red);

                //Build our Rotated vector
                Vector3 rotatedVector = Quaternion.AngleAxis(m_Angle, Vector3.up) * worldPosition;
                //Debug.DrawLine(m_Target.position, rotatedVector, Color.green);

                //Move our position
                Vector3 position = m_Target == null ? new Vector3(0, 0, 0) : m_Target.position;
                Vector3 flatTargetPosition = position;
                flatTargetPosition.y = 0f;
                Vector3 finalPosition = flatTargetPosition + rotatedVector;
                //Debug.DrawLine(m_Target.position, finalPosition, Color.blue);
                transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref refVelocity, m_SmoothSpeed);
                transform.LookAt(position);
            }
            else
            {
#if true

                transform.position = m_Target.position - m_Target.forward * cameraDistance;
                transform.LookAt(m_Target.position);
                transform.position = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);

#endif

#if false
                float speed = 5f;


                // Look
                var newRotation = Quaternion.LookRotation(m_Target.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, speed * Time.deltaTime);

                // Move
                Vector3 newPosition = m_Target.position - m_Target.forward * offset_z/*offset.z*/ - m_Target.up * offset_y/*offset.y*/;
                transform.position = Vector3.Slerp(transform.position, newPosition, Time.deltaTime * speed);
#endif
            }
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
