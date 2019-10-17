using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;
    private int currentHealth;

    public event Action<float> OnHealthPctChanged = delegate { };


    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public void ModifyHealth(int amount)
    {
        if (amount < 0
            && Math.Abs(currentHealth) < Math.Abs(amount))
        {
            currentHealth = 0;
        }
        else
        {
            currentHealth += amount;
        }

        float currentHealthPct = (float)currentHealth / (float)maxHealth;
        OnHealthPctChanged(currentHealthPct);
    }
}
