using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcHealth : MonoBehaviour
{
    public float MaxHealth;
    public float health = 100f;



    void Start()
    {
        MaxHealth = health;
    }

    public void TakeDamage(float damage)
    {
        if (health > 0f)
        {
            health -= damage;
        }
    }
}