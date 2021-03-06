﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndiePixel.Cameras
{
    public class IP_Base_Camera : MonoBehaviour
    {
        #region Variables
        protected Transform m_Target;
        #endregion


        #region Main Methods
        // Start is called before the first frame update
        void Start()
        {
            HandleCamera();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            HandleCamera();
        }


        public void SetTarget(Transform targetTransform)
        {
            m_Target = targetTransform;
        }

        public bool GetTarget(out Transform transform)
        {
            transform = m_Target;
            return (null != m_Target) ? true : false;
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
