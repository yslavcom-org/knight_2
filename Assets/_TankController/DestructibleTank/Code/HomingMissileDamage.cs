using UnityEngine;

namespace MyTankGame
{
    public class HomingMissileDamage : MonoBehaviour, IHomingMissileDamageable, ITankGunDamageable
    {
        #region Variables
        Rigidbody[] rbArray;
        #endregion

        #region Built-in methods
        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        void Awake()
        {
            Init();
        }
        #endregion

        #region Custom methods
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
        #endregion

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

        #region ITankGunDamageable
        public bool GunPointsThisObject(Vector3 distance, object obj)
        {
            //highlight the object icon
            Debug.Log("GunPointsThisObject");
            return true;
        }

        public bool GunShootsThisObject(Vector3 distance, object obj)
        {
            //highlight the object icon
            Debug.Log("GunShootsThisObject");
            return true;
        }
        #endregion
    }
}
