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

    private int IndicatorBarLayerId = 0;

    private Dictionary<Stamina, StaminaBar> staminaBars = new Dictionary<Stamina, StaminaBar>();

    private void Awake()
    {
        Health.OnAdded += AddBar;
        Health.OnRemoved += RemoveBar;

        Fuel.OnAdded += AddBar;
        Fuel.OnRemoved += RemoveBar;
        
        Ammunition.OnAdded += AddBar;
        Ammunition.OnRemoved += RemoveBar;

        IndicatorBarLayerId = LayerMask.NameToLayer("IndicatorBar");
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

    private void AddBar(BarType type, Stamina bar)
    {
        if(staminaBars.ContainsKey(bar) == false)
        {
            StaminaBar temp = (BarType.Health == type)
                ? healthBarPrefab  : (BarType.Fuel == type)
                ? fuelBarPrefab  
                : ammunitionBarPrefab;
            temp.gameObject.layer = IndicatorBarLayerId;

            var staminaBar = Instantiate(temp, transform);
            staminaBars.Add(bar, staminaBar);
            staminaBar.SetAmount(bar);
        }
    }

    private void RemoveBar(BarType type, Stamina bar)
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
