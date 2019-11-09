using UnityEditor;
using UnityEngine;

/*
HomingMissilePool which will dispatch the missile Launch.
Objects get created on start-up and are reused
*/

namespace MyTankGame
{
    using Item = GameInventory.Item;
    public class HomingMissilePool : MonoBehaviour
    {
        public const int homingMissileCount = 10;
        GameObject[] homingMissilePool;
        private int currentIdx = -1;
        bool enabled;

        // Start is called before the first frame update
        void Start()
        {
            var homingMissilePrefab = AssetDatabase.LoadAssetAtPath(HardcodedValues.StrPathToHomingMissilePrefab, typeof(GameObject));
            if (homingMissilePrefab == null) return;

            homingMissilePool = new GameObject[homingMissileCount];
            if (null == homingMissilePool) return;

            for (int i = 0; i < homingMissileCount; i++)
            {
                homingMissilePool[i] = Instantiate(homingMissilePrefab) as GameObject;
            }
            currentIdx = 0;
        }

        #region Custom methods
        int GetTotalObjects()
        {
            return (null != homingMissilePool) ? homingMissilePool.Length : 0;
        }

        int GetNextIdleObjectIdx()
        {
            int totalObj = GetTotalObjects();
            if (totalObj == 0) return -1;
            else
            {
                if(currentIdx >= totalObj)
                {
                    currentIdx = 0;
                }

                return currentIdx++;
            }
        }

        public void SetEnabled(bool enable)
        {
            enabled = enable;
        }

        public bool TryUseHomingMissile(out GameObject homingMissile)
        {
            if (enabled)
            {
                int idx = GetNextIdleObjectIdx();
                if (0 <= idx)
                {
                    var obj = homingMissilePool[idx];
                    homingMissile = obj;
                    return true;
                }
            }

            homingMissile = null;
            return false;
        }

        #endregion
    }
}
