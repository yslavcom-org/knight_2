using System;
using UnityEngine;

public class Fuel : MonoBehaviour, IStamina
{
    public static event Action<StaminaBarController.BarType, Fuel> OnAdded = delegate { };
    public static event Action<StaminaBarController.BarType, Fuel> OnRemoved = delegate { };


    [SerializeField]
    protected int maxLevel = 100;
    public int CurrentLevel { get; protected set; }

    public event Action<float> OnPctChanged = delegate { }; // fuel changed
    public static event Action<StaminaBarController.BarType, Fuel> OnBarZero = delegate { }; // no more fuel remaining


    protected void OnEnable()
    {
        CurrentLevel = maxLevel;
        OnAdded(StaminaBarController.BarType.Fuel, this);
    }

    protected void OnDisable()
    {
        OnRemoved(StaminaBarController.BarType.Fuel, this);
    }

    public void ModifyStamina(int amount)
    {
        if (CurrentLevel == 0)
        {
            //do nothing
        }
        else if (amount < 0
            && Math.Abs(CurrentLevel) < Math.Abs(amount))
        {
            CurrentLevel = 0;
            OnBarZero(StaminaBarController.BarType.Fuel, this);
        }
        else
        {
            if (amount > 0
                && (maxLevel - CurrentLevel) <= amount)
            {
                CurrentLevel = maxLevel;
            }
            else
            {
                CurrentLevel += amount;
            }

            if (CurrentLevel == 0)
            {
                OnBarZero(StaminaBarController.BarType.Fuel, this);
            }
        }

        float currentPct = (float)CurrentLevel / (float)maxLevel;
        OnPctChanged(currentPct);
    }
}