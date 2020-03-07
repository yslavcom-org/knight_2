using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTankGame
{
    public class Tank_Navigation : MonoBehaviour
    {
        #region Custom Enumerators
        enum EnAngleDirection
        {
            EnAngleDirection__Centre,
            EnAngleDirection__Left,
            EnAngleDirection__Right
        };
        #endregion

        //[SerializeField]
        private float targetAngle;
        //[SerializeField]
        private EnAngleDirection left_right;

        private const float near_distance = 2f;
        EnAngleDirection prev_left_right;
        //[SerializeField]
        private float prev_distance;
        //[SerializeField]
        private float distance;

        Vector3 prevTargetPosition = Vector3.zero;

        enum EnMove
        {
            EnMove__Idle,
            EnMove__Rotate,
            EnMove__Move
        };
        EnMove enMoveSm;

#if false
        public void GraduallyMoveRigidBody(Transform transform, Rigidbody rigidBody, Vector3 targetPosition, float actualTankSpeed, float tankRotationSpeed)
        {
            Vector3 nextPosition = transform.position + (transform.forward * actualTankSpeed * Time.deltaTime);
            distance = Vector3.Distance(nextPosition, targetPosition);

            Vector3 vectorToTarget = targetPosition - transform.position;
            targetAngle = Vector3.Angle(vectorToTarget, transform.forward);

            if (prevTargetPosition != targetPosition)
            {//move to new position
                enMoveSm = EnMove.EnMove__Rotate; // first, rotate
                prev_left_right = EnAngleDirection.EnAngleDirection__Centre;

                prevTargetPosition = targetPosition;
                prev_distance = float.MaxValue;
            }

            if (enMoveSm == EnMove.EnMove__Rotate)
            {
                ////////////////////////////////////////
                //rotate tank

                left_right = get_left_or_right(vectorToTarget);

                if (left_right == EnAngleDirection.EnAngleDirection__Right)
                {
                    Quaternion wantedRotation = transform.rotation * Quaternion.Euler(Vector3.up * tankRotationSpeed * Time.deltaTime);
                    rigidBody.MoveRotation(wantedRotation);
                }
                else if (left_right == EnAngleDirection.EnAngleDirection__Left)
                {
                    Quaternion wantedRotation = transform.rotation * Quaternion.Euler(Vector3.up * (-tankRotationSpeed) * Time.deltaTime);
                    rigidBody.MoveRotation(wantedRotation);
                }
                else
                {
                    enMoveSm = EnMove.EnMove__Move;
                }

                if (enMoveSm != EnMove.EnMove__Move)
                {
                    if ((left_right != prev_left_right && prev_left_right != EnAngleDirection.EnAngleDirection__Centre)
                        || left_right == EnAngleDirection.EnAngleDirection__Centre)
                    {
                        enMoveSm = EnMove.EnMove__Move;
                    }
                }

                prev_left_right = left_right;
            }

            if (enMoveSm == EnMove.EnMove__Move
                || enMoveSm == EnMove.EnMove__Rotate)
            {
                ////////////////////////////////////////
                //move tank

                if (enMoveSm == EnMove.EnMove__Move
                        && prev_distance < distance)
                {
                    enMoveSm = EnMove.EnMove__Idle;
                }
                else if (distance >= near_distance)
                {
                    rigidBody.MovePosition(nextPosition);
                    prev_distance = distance;
                }
                else
                {
                    enMoveSm = EnMove.EnMove__Idle;
                }
            }

        }
#endif
        private EnAngleDirection get_left_or_right(Vector3 vectorToTarget)
        {
            var perp = Vector3.Cross(transform.forward, vectorToTarget);
            var dir = Vector3.Dot(perp, Vector3.up);

            EnAngleDirection enEnAngleDirection = EnAngleDirection.EnAngleDirection__Centre;
            if (dir > 0.0)
            {
                enEnAngleDirection = EnAngleDirection.EnAngleDirection__Right;
                //Debug.Log("right");
            }
            else if (dir < 0.0)
            {
                enEnAngleDirection = EnAngleDirection.EnAngleDirection__Left;
                //Debug.Log("left");
            }
            else
            {
                //Debug.Log("centre");
            }

            return enEnAngleDirection;
        }
    }
}
