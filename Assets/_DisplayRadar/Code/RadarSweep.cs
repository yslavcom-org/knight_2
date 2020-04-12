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
        [SerializeField]
        private Quaternion startRotationAngle;
        Vector3 right;
        Vector3 up;

        public float GetRotationAngle()
        {
            //float angle = Quaternion.Angle(startRotationAngle, transform.rotation);
            //return angle;

            return (rotationDir == -1)
                ? (360 - transform.eulerAngles.z) : transform.eulerAngles.z;
        }

        /*
        float SignedAngleBetween(Vector3 a, Vector3 b, bool clockwise)
        {
            float angle = Vector3.Angle(a, b);

            //clockwise
            if (Mathf.Sign(angle) == -1 && clockwise)
                angle = 360 + angle;

            //counter clockwise
            else if (Mathf.Sign(angle) == 1 && !clockwise)
                angle = -angle;
            return angle;
        }

        float CalculateAngle(Vector3 from, Vector3 to)
        {
            return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
        }
        */

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