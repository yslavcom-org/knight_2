using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    public enum BarType
    {
        Health,
        Fuel,
        Ammunition,
    };

    private HealthBar healthBarPrefab;
    private HealthBar ammunitionBarPrefab;
    private HealthBar fuelBarPrefab;

    private Dictionary<Health, HealthBar> healthBars = new Dictionary<Health, HealthBar>();

    private void Awake()
    {
        Health.OnHealthAdded += AddBar;
        Health.OnHealthRemoved += RemoveBar;
    }

    public void SetHealthBarPrefab(BarType type, HealthBar barPrefab)
    {
        switch(type)
        {
            case BarType.Health:
                healthBarPrefab = barPrefab;
                break;

            case BarType.Fuel:
                fuelBarPrefab = barPrefab;
                break;

            case BarType.Ammunition:
                ammunitionBarPrefab = barPrefab;
                break;
        }
    }

    private void AddBar(BarType type, Health bar)
    {
        if(healthBars.ContainsKey(bar) == false)
        {
            HealthBar temp = (BarType.Health == type)
                ? healthBarPrefab  : (BarType.Fuel == type)
                ? fuelBarPrefab  
                : ammunitionBarPrefab;

            var healthBar = Instantiate(temp, transform);
            healthBars.Add(bar, healthBar);
            healthBar.SetHealth(bar);
        }
    }

    private void RemoveBar(BarType type, Health bar)
    {
        if (healthBars.ContainsKey(bar) == true)
        {
            if (null != healthBars[bar].gameObject)
            {
                Destroy(healthBars[bar].gameObject);
            }
            healthBars.Remove(bar);
        }
    }

    public void SetTrackingCamera(Camera cam)
    {
        foreach(var healthBar in healthBars)
        {
            healthBar.Value.SetTrackingCamera(cam);
        }
    }
}
