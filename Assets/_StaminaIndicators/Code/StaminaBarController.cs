using System.Collections.Generic;
using UnityEngine;

public class StaminaBarController : MonoBehaviour
{
    public enum BarType
    {
        Health,
        Fuel,
        Ammunition,
    };

    private StaminaBar healthBarPrefab;
    private StaminaBar ammunitionBarPrefab;
    private StaminaBar fuelBarPrefab;

    private Dictionary<IStamina, StaminaBar> staminaBars = new Dictionary<IStamina, StaminaBar>();

    private void Awake()
    {
        Health.OnAdded += AddBar;
        Health.OnRemoved += RemoveBar;
    }

    public void SetStaminaBarPrefab(BarType type, StaminaBar barPrefab)
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
        if(staminaBars.ContainsKey(bar) == false)
        {
            StaminaBar temp = (BarType.Health == type)
                ? healthBarPrefab  : (BarType.Fuel == type)
                ? fuelBarPrefab  
                : ammunitionBarPrefab;

            var staminaBar = Instantiate(temp, transform);
            staminaBars.Add(bar, staminaBar);
            staminaBar.SetHealth(bar);
        }
    }

    private void RemoveBar(BarType type, Health bar)
    {
        if (staminaBars.ContainsKey(bar) == true)
        {
            if (null != staminaBars[bar].gameObject)
            {
                Destroy(staminaBars[bar].gameObject);
            }
            staminaBars.Remove(bar);
        }
    }

    public void SetTrackingCamera(Camera cam)
    {
        foreach(var bar in staminaBars)
        {
            bar.Value.SetTrackingCamera(cam);
        }
    }
}
