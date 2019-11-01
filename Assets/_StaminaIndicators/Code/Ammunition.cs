using System;
using UnityEngine;

public class Ammunition : MonoBehaviour, IStamina
{
    public static event Action<StaminaBarController.BarType, Ammunition> OnAdded = delegate { };
    public static event Action<StaminaBarController.BarType, Ammunition> OnRemoved = delegate { };


    [SerializeField]
    protected int maxLevel = 100;
    public int CurrentLevel { get; protected set; }

    public event Action<float> OnPctChanged = delegate { }; // amunition changed
    public static event Action<StaminaBarController.BarType, Ammunition> OnBarZero = delegate { }; // no more amunition remaining


    protected void OnEnable()
    {
        CurrentLevel = maxLevel;
        OnAdded(StaminaBarController.BarType.Ammunition, this);
    }

    protected void OnDisable()
    {
        OnRemoved(StaminaBarController.BarType.Ammunition, this);
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
            OnBarZero(StaminaBarController.BarType.Ammunition, this);
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
                OnBarZero(StaminaBarController.BarType.Ammunition, this);
            }
        }

        float currentPct = (float)CurrentLevel / (float)maxLevel;
        OnPctChanged(currentPct);
    }
}