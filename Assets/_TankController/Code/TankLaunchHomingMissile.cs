using UnityEngine;

namespace MyTankGame
{
    public class TankLaunchHomingMissile : MonoBehaviour
    {
        private HomingMissilePool homingMissilePool;
        private TankLaunchHomingMissile tankLaunchHomingMissile;

        #region Built-in Methods
        private void Start()
        {
            homingMissilePool = gameObject.GetComponent<MyTankGame.HomingMissilePool>();
            tankLaunchHomingMissile = gameObject.GetComponent<MyTankGame.TankLaunchHomingMissile>();
        }
        #endregion

        public bool Launch(Radar radar, ref IndiePixel.Cameras.IP_Minimap_Camera homingMissileTrackingCamera)
        {
            if (null == radar) return false;
            if (radar.GetClosestLockedObject(out Transform targetTransform))
            {
                if (null == homingMissilePool) return false;

                var startPosition = this.transform.position;

                return homingMissilePool.BoLaunchMissile(startPosition, targetTransform, ref homingMissileTrackingCamera);
            }

            return false;
        }
    }
}
