using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public static event Action<Health> OnHealthAdded = delegate { };
    public static event Action<Health> OnHealthRemoved = delegate { };


    [SerializeField]
    private int maxHealth = 100;
    public int CurrentHealth { get; private set; }

    public event Action<float> OnHealthPctChanged = delegate { }; // health changed
    public event Action<bool> OnHealthZero = delegate { }; // no more health remaining


    private void OnEnable()
    {
        CurrentHealth = maxHealth;
        OnHealthAdded(this);
    }

    private void OnDisable()
    {
        OnHealthRemoved(this);
    }

    public void ModifyHealth(int amount)
    {
        if (amount < 0
            && Math.Abs(CurrentHealth) < Math.Abs(amount))
        {
            CurrentHealth = 0;
            OnHealthZero(true);
        }
        else
        {
            if (amount > 0
                && (maxHealth - CurrentHealth) <= amount)
            {
                CurrentHealth = maxHealth;
            }
            else
            {
                CurrentHealth += amount;
            }
        }

        float currentHealthPct = (float)CurrentHealth / (float)maxHealth;
        OnHealthPctChanged(currentHealthPct);
    }
}
