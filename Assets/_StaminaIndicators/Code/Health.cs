using System;
using UnityEngine;

public class Health : Stamina
{
    public static event Action<StaminaBarController.BarType, Health> OnAdded = delegate { };
    public static event Action<StaminaBarController.BarType, Health> OnRemoved = delegate { };


    [SerializeField]
    protected int maxLevel = 100;
    public int CurrentLevel { get; protected set; }

    public override event Action<float> OnPctChanged = delegate { }; // health changed
    public static event Action<StaminaBarController.BarType, Health> OnBarZero = delegate { }; // no more health remaining


    protected void OnEnable()
    {
        CurrentLevel = maxLevel;
        OnAdded(StaminaBarController.BarType.Health, this);
    }

    protected void OnDisable()
    {
        OnRemoved(StaminaBarController.BarType.Health, this);
    }

    private void update_graphics()
    {
        float currentPct = (float)CurrentLevel / (float)maxLevel;
        OnPctChanged(currentPct);
    }

    public void SetStaminaToMaxLevel()
    {
        CurrentLevel = maxLevel;
        update_graphics();
    }

    public override void ModifyStamina(int amount)
    {
        if(CurrentLevel == 0)
        {
            //do nothing
        }
        else if (amount < 0
            && Math.Abs(CurrentLevel) < Math.Abs(amount))
        {
            CurrentLevel = 0;
            OnBarZero(StaminaBarController.BarType.Health, this);
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

            if(CurrentLevel==0)
            {
                OnBarZero(StaminaBarController.BarType.Health, this);
            }
        }

        float currentHealthPct = (float)CurrentLevel / (float)maxLevel;
        OnPctChanged(currentHealthPct);
    }
}
