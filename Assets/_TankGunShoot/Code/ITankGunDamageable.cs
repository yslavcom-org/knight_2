using UnityEngine;

public interface ITankGunDamageable
{
    bool GunPointsThisObject(Vector3 distance, object obj);
    bool GunShootsThisObject(Vector3 distance, object obj);
}
