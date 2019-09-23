using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndiePixel.Cameras
{
    public class IP_Base_Camera : MonoBehaviour
    {
        #region Variables
        private GameObject m_TargetObject;
        public Transform m_Target;
        public string TargetObjectName = "TankHolder";
        #endregion


        #region Main Methods
        // Start is called before the first frame update
        void Start()
        {
            var obj = GameObject.Find(TargetObjectName);
            if (obj)
            {
                m_Target = obj.transform;
            }
            HandleCamera();
        }

        // Update is called once per frame
        void Update()
        {
            HandleCamera();
        }
        #endregion

        #region Helper Methods
        protected virtual void HandleCamera()
        {
            if (null == m_Target) return;
        }
        #endregion
    }
}
