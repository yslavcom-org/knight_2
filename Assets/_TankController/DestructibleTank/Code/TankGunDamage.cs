using UnityEngine;

namespace MyTankGame
{
    public class TankGunDamage : MonoBehaviour
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

        public void SetDestroyed()
        {
            foreach (var rb in rbArray)
            {
                rb.isKinematic = false;
            }
        }
        #endregion

    }
}
