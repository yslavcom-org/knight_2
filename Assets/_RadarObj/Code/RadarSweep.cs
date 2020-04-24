using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyRadar
{
    public class RadarSweep : MonoBehaviour
    {
        public float RotationSpeed = 50f;

        private const int rotationDir = -1; // -1 or +1

        [SerializeField]
        private float rotationAngle;
        public float RotationAngle { get { return rotationAngle; } }

        [SerializeField]
        private Quaternion startRotationAngle;

        public float GetRotationAngle()
        {
            return (rotationDir == -1)
                ? (360 - transform.eulerAngles.z) : transform.eulerAngles.z;
        }

        public void ResetSweepLine()
        {
            rotationAngle = 0;
            this.transform.rotation = Quaternion.identity;
        }

        private void Start()
        {
            startRotationAngle = transform.rotation;
        }

        void FixedUpdate()
        {
            rotationAngle = rotationDir * (RotationSpeed * Time.deltaTime);
            this.transform.Rotate(0f, 0f, rotationAngle);
        }
    }
}