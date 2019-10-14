using UnityEngine;

namespace MyTankGame
{
    public class HomingMissileDamage : MonoBehaviour, IHomingMissileDamageable
    {
        Rigidbody[] rbArray;

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        void Awake()
        {
            Init();
        }

        private void Init()
        {
            rbArray = GetComponentsInChildren<Rigidbody>();
        }

        void BlowUp()
        {
            foreach (var rb in rbArray)
            {
                if (rb.name != "DestructibleTank")
                {
                    rb.isKinematic = false;
                }
            }
        }

        #region IHomingMissileDamage
        public bool IsHomingMissileDamageable()
        {
            return false;
        }
        public bool HomingMissileBlowUp()
        {
            BlowUp();
            return true;
        }
        #endregion
    }
}
