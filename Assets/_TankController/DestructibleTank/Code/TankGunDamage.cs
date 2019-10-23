using UnityEngine;

namespace MyTankGame
{
    public class TankGunDamage : MonoBehaviour
    {
        #region Variables
        public GameObject SmokePrefab;
        GameObject smoke;

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
            smoke = Instantiate(SmokePrefab, this.transform);
            smoke.SetActive(false);
            rbArray = GetComponentsInChildren<Rigidbody>();
        }

        public void SetDestroyed()
        {
            foreach (var rb in rbArray)
            {
                rb.isKinematic = false;
            }
            smoke.SetActive(true);
        }
        #endregion

    }
}
