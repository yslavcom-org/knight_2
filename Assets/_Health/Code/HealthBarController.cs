using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    private HealthBar healthBarPrefab;

    private Dictionary<Health, HealthBar> healthBars = new Dictionary<Health, HealthBar>();

    private void Awake()
    {
        Health.OnHealthAdded += AddHealthBar;
        Health.OnHealthRemoved += RemoveHealthBar;
    }

    public void SetPrefab(HealthBar healthBarPrefab)
    {
        this.healthBarPrefab = healthBarPrefab;
    }

    private void AddHealthBar(Health health)
    {
        if(healthBars.ContainsKey(health) == false)
        {
            var healthBar = Instantiate(healthBarPrefab, transform);
            healthBars.Add(health, healthBar);
            healthBar.SetHealth(health);
        }
    }

    private void RemoveHealthBar(Health health)
    {
        if (healthBars.ContainsKey(health) == true)
        {
            Destroy(healthBars[health].gameObject);
            healthBars.Remove(health);
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
