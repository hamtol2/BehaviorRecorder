using System.Collections;
using UnityEngine;

namespace REEL.PoseAnimation
{
    public enum JointAxis
    {
        RotFX, RotFY, RotFZ, RotRX, RotRY, RotRZ
    }

    [System.Serializable]
    public class JointSet
    {
        public Transform joint;
        public Vector3 baseRotation;
        public JointAxis jointRotAxis;
        public bool isFixed = false;
        private float angle;

        public void SetAngle(float angle)
        {
            this.angle = angle;
            Vector3 rot = isFixed ? GetFixedEulerAngle(angle) : GetEulerAngle(angle);
            joint.localRotation = Quaternion.Euler(rot);
        }

        public IEnumerator SetAngleLerp(float angle, float duration)
        {
            float elapsedTime = 0f;
            this.angle = angle;
            Vector3 rot = isFixed ? GetFixedEulerAngle(angle) : GetEulerAngle(angle);
            Quaternion targetRot = Quaternion.Euler(rot);

            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;
                joint.localRotation = Quaternion.Lerp(joint.localRotation, targetRot, elapsedTime);

                yield return null;
            }
        }

        public float GetAngle()
        {
            return angle;
        }

        public void SetBaseAngle()
        {
            joint.localRotation = Quaternion.Euler(baseRotation);
        }

        Vector3 GetEulerAngle(float angle)
        {
            Vector3 rot = joint.localEulerAngles;

            switch (jointRotAxis)
            {
                case JointAxis.RotFX: rot.x = baseRotation.x + angle; break;
                case JointAxis.RotRX: rot.x = baseRotation.x - angle; break;
                case JointAxis.RotFY: rot.y = baseRotation.y + angle; break;
                case JointAxis.RotRY: rot.y = baseRotation.y - angle; break;
                case JointAxis.RotFZ: rot.z = baseRotation.z + angle; break;
                case JointAxis.RotRZ: rot.z = baseRotation.z - angle; break;

                default: break;
            }

            return rot;
        }

        Vector3 GetFixedEulerAngle(float angle)
        {
            Vector3 rot = baseRotation;

            switch (jointRotAxis)
            {
                case JointAxis.RotFX: rot.x = baseRotation.x + angle; break;
                case JointAxis.RotRX: rot.x = baseRotation.x - angle; break;
                case JointAxis.RotFY: rot.y = baseRotation.y + angle; break;
                case JointAxis.RotRY: rot.y = baseRotation.y - angle; break;
                case JointAxis.RotFZ: rot.z = baseRotation.z + angle; break;
                case JointAxis.RotRZ: rot.z = baseRotation.z - angle; break;

                default: break;
            }

            return rot;
        }
    }
}