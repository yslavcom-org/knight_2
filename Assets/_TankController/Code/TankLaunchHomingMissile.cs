using UnityEngine;

namespace MyTankGame
{
    public class TankLaunchHomingMissile : MonoBehaviour
    {
        private TankLaunchHomingMissile tankLaunchHomingMissile;
        MyTankGame.IObjectId launcherObjId;

        #region Built-in Methods
        private void Start()
        {
            var id = gameObject.GetComponentsInParent<MyTankGame.IObjectId>();
            if (null != id)
            {
                launcherObjId = id[0];
            }

            tankLaunchHomingMissile = gameObject.GetComponent<MyTankGame.TankLaunchHomingMissile>();
        }
        #endregion

        public bool Launch(Radar radar, ref MyTankGame.HomingMissilePool homingMissilePool,  ref IndiePixel.Cameras.IP_Minimap_Camera homingMissileTrackingCamera)
        {
            if (null == radar) return false;
            if (radar.GetClosestLockedObject(out Transform targetTransform))
            {
                if (null == homingMissilePool) return false;
                var startPosition = this.transform.position;

                return BoLaunchMissile(startPosition, targetTransform, ref homingMissilePool, ref homingMissileTrackingCamera);
            }

            return false;
        }

        bool BoLaunchMissile(Vector3 startPosition, Transform targetTransform, ref MyTankGame.HomingMissilePool homingMissilePool, ref IndiePixel.Cameras.IP_Minimap_Camera homingMissileTrackingCamera)
        {
            bool available = homingMissilePool.TryUseHomingMissile(out GameObject homingMissile);
            if (!available) return false;


            var handle = homingMissile.GetComponent<MyTankGame.HomingMissileController>();
            if (handle == null) return false;

            handle.Launch(launcherObjId, startPosition, targetTransform, homingMissileTrackingCamera);

            return true;
        }
    }
}
