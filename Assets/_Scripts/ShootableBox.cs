using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableBox : MonoBehaviour
{
    public int currentHealth = 3;

    public void Damage(int damageAmount)
    {
        Debug.Log("target = "+ this.gameObject.name);

        currentHealth -= damageAmount;
        if(currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
