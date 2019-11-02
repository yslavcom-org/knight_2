using System;
using UnityEngine;

public abstract class Stamina  : MonoBehaviour
{
    public abstract event Action<float> OnPctChanged;// stamina changed
    public abstract void ModifyStamina(int amount);
}
